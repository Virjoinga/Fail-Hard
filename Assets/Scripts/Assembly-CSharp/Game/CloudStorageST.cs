using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Game
{
	public class CloudStorageST : IStorage
	{
		private class ResponseObject
		{
			public ManualResetEvent link;

			public VMessage response;

			public DateTime sentAt;

			public string queryId;

			public int MessageNumber;

			public IMessageConsumer consumer;

			public void SetBlocking()
			{
				link = new ManualResetEvent(false);
			}

			public bool IsBlocking()
			{
				return link != null;
			}

			public bool WaitForResponse()
			{
				if (link != null)
				{
					return link.WaitOne(MESSAGE_WAIT_PERIOD_MS);
				}
				return true;
			}
		}

		private enum MessageType
		{
			Client = 0,
			Global = 1,
			Counter = 2,
			Handshake = 3
		}

		public static bool GenerateId = false;

		public static bool ServerAvailable = true;

		public static int[] ServerVersion;

		private static readonly int MESSAGE_WAIT_PERIOD = 8;

		private TimeSpan MESSAGE_WAIT_PERIOD_TS = TimeSpan.FromSeconds(MESSAGE_WAIT_PERIOD);

		private static int MESSAGE_WAIT_PERIOD_MS = MESSAGE_WAIT_PERIOD * 1000;

		private TimeSpan OFFLINE_STAY_PERIOD = TimeSpan.FromMinutes(15.0);

		public ManualResetEvent connectionEvent;

		public ManualResetEvent handshakeEvent;

		private Timer responseChecker;

		private Timer onlineChecker;

		private Dictionary<int, ResponseObject> waitResponse;

		private int nextMessageNumber;

		private static CloudStorageST s_instance;

		private Queue<VMessage> requestQueue;

		private Queue<VMessage> responseQueue;

		private string globalId = "devel1";

		private string clientId;

		private Socket clientSocket;

		private Thread requestThread;

		private Thread responseThread;

		private volatile bool shouldRequest;

		private volatile bool shouldResponse;

		private bool initialized;

		private bool blockUpdates;

		private bool preventServerUpdates;

		private bool localNewer = true;

		private long session;

		public static CloudStorageST Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = new CloudStorageST();
				}
				return s_instance;
			}
		}

		public VMessage IncomingMessage
		{
			get
			{
				if (responseQueue.Count == 0)
				{
					return null;
				}
				VMessage vMessage = null;
				lock (responseQueue)
				{
					return responseQueue.Dequeue();
				}
			}
			private set
			{
				lock (responseQueue)
				{
					responseQueue.Enqueue(value);
				}
			}
		}

		public int SavedAt
		{
			get
			{
				return PlayerPrefs.GetInt("cloudSaveAt");
			}
			private set
			{
				PlayerPrefs.SetInt("cloudSaveAt", value);
				PlayerPrefs.Save();
			}
		}

		public CloudStorageST()
		{
			requestQueue = new Queue<VMessage>();
			responseQueue = new Queue<VMessage>();
			nextMessageNumber = 1;
			waitResponse = new Dictionary<int, ResponseObject>();
			responseChecker = new Timer(VerifyResponses, null, -1, -1);
			onlineChecker = new Timer(RestoreOnlineState, null, -1, -1);
			connectionEvent = new ManualResetEvent(true);
			handshakeEvent = new ManualResetEvent(false);
		}

		private ResponseObject SynchronizedGetResponse(int key)
		{
			ResponseObject result = null;
			lock (waitResponse)
			{
				if (waitResponse.ContainsKey(key))
				{
					result = waitResponse[key];
				}
			}
			return result;
		}

		private void SynchronizedAddResponse(int key, ResponseObject v)
		{
			lock (waitResponse)
			{
				waitResponse.Add(key, v);
			}
		}

		private void SynchronizedRemoveResponse(int key)
		{
			lock (waitResponse)
			{
				if (waitResponse.ContainsKey(key))
				{
					waitResponse.Remove(key);
				}
			}
		}

		public void Init()
		{
			if (initialized)
			{
				return;
			}
			LocalStorage.Init();
			if (!GenerateId && PlayerPrefs.HasKey("clientId"))
			{
				clientId = PlayerPrefs.GetString("clientId");
			}
			else
			{
				clientId = Guid.NewGuid().ToString();
				if (!GenerateId)
				{
					PlayerPrefs.SetString("clientId", clientId);
					PlayerPrefs.Save();
				}
			}
			GA.SettingsGA.SetCustomUserID(clientId);
			initialized = true;
			if (Application.internetReachability != 0)
			{
				Restore();
			}
			else
			{
				ServerAvailable = false;
			}
		}

		private void MarkSaved()
		{
			SavedAt = Storage.UnixTimeNow;
		}

		public void Shutdown()
		{
			Shutdown(false);
		}

		public void ShutdownError()
		{
			Shutdown(true);
		}

		public void Shutdown(bool error)
		{
			if (!ServerAvailable)
			{
				return;
			}
			if (error)
			{
				ServerAvailable = false;
			}
			try
			{
				if (!error)
				{
					MarkSaved();
				}
				LocalStorage.Instance.Shutdown();
				shouldRequest = false;
				shouldResponse = false;
				if (Monitor.TryEnter(requestQueue))
				{
					try
					{
						Monitor.Pulse(requestQueue);
					}
					finally
					{
						Monitor.Exit(requestQueue);
					}
				}
				try
				{
					clientSocket.Shutdown(SocketShutdown.Both);
					clientSocket.Close();
					if (!requestThread.Join(500))
					{
						Debug.LogWarning("Request thread didn't terminate gracefully!");
					}
					if (!responseThread.Join(500))
					{
						Debug.LogWarning("Response thread didn't terminate gracefully!");
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning(ex.ToString());
				}
				responseChecker.Change(-1, -1);
			}
			finally
			{
				connectionEvent.Set();
			}
		}

		public void Restore()
		{
			if (connectionEvent.WaitOne(0))
			{
				ServerAvailable = true;
				connectionEvent.Reset();
				clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				StartThreads();
				responseChecker.Change(MESSAGE_WAIT_PERIOD_TS, MESSAGE_WAIT_PERIOD_TS);
				onlineChecker.Change(-1, -1);
				if (SavedAt < LocalStorage.Instance.SavedAt)
				{
					localNewer = true;
				}
			}
		}

		private void StartThreads()
		{
			try
			{
				if (requestThread == null || !shouldRequest)
				{
					shouldRequest = true;
					requestThread = new Thread(StartSending);
					requestThread.Start();
				}
			}
			catch (ThreadStateException)
			{
			}
			try
			{
				if (responseThread == null || !shouldResponse)
				{
					shouldResponse = true;
					responseThread = new Thread(StartReceiving);
					responseThread.Start();
				}
			}
			catch (ThreadStateException)
			{
			}
		}

		private bool HandleWait(ResponseObject ro)
		{
			bool flag = ro.WaitForResponse();
			if (!flag)
			{
				if (!ServerAvailable)
				{
					Debug.LogWarning("We are already offline, error handlers already called.");
				}
				else
				{
					Debug.LogWarning("Server is not responding in time, going to offline mode.");
					ShutdownError();
					onlineChecker.Change(OFFLINE_STAY_PERIOD, TimeSpan.FromMilliseconds(-1.0));
				}
			}
			SynchronizedRemoveResponse(ro.MessageNumber);
			return flag;
		}

		private void QueueMessage(VMessage msg)
		{
			if (!initialized)
			{
				Debug.LogWarning("CloudStorage must be initialized before it is used!");
			}
			if (requestThread == null || !requestThread.IsAlive)
			{
				Debug.LogWarning("Queuing messages without running request thread.");
			}
			lock (requestQueue)
			{
				requestQueue.Enqueue(msg);
				Monitor.Pulse(requestQueue);
			}
		}

		private void StartSending()
		{
			try
			{
				clientSocket.Connect("cm.viimagames.com", 22991);
				connectionEvent.Set();
				AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
				ulong num = 0uL;
				bool flag = false;
				handshakeEvent.Reset();
				while (shouldRequest)
				{
					int num2 = 0;
					lock (requestQueue)
					{
						num2 = requestQueue.Count;
					}
					if (num2 > 0 || !flag)
					{
						using (MemoryStream memoryStream = new MemoryStream(200))
						{
							memoryStream.Position = 4L;
							VMessage vMessage = null;
							if (flag)
							{
								lock (requestQueue)
								{
									vMessage = requestQueue.Dequeue();
									vMessage.Session = session;
								}
							}
							else
							{
								vMessage = ConstructMessage(MessageType.Handshake);
								vMessage.SecondaryId = "handshake";
								flag = true;
							}
							if (vMessage == null)
							{
								Debug.LogError("GPF, requestQueue should not be empty here!");
							}
							aJProtoBufSerializer.Serialize(memoryStream, vMessage);
							byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)memoryStream.Position - 4));
							long position = memoryStream.Position;
							memoryStream.Position = 0L;
							memoryStream.Write(bytes, 0, 4);
							byte[] buffer = memoryStream.GetBuffer();
							for (int i = 0; i < position; i++)
							{
								EData.ecdc(ref buffer[i], num);
								num++;
							}
							clientSocket.Send(buffer, (int)position, SocketFlags.None);
						}
						if (!handshakeEvent.WaitOne(MESSAGE_WAIT_PERIOD_MS))
						{
							Debug.LogWarning("Handshake with server failed! (timeout)");
							ApplicationFocusController.ExecuteLater(ShutdownError);
							shouldRequest = false;
						}
					}
					else
					{
						lock (requestQueue)
						{
							Monitor.Wait(requestQueue);
						}
					}
				}
			}
			catch (SocketException ex)
			{
				Debug.LogWarning(ex.ToString());
			}
			connectionEvent.Set();
		}

		private void StartReceiving()
		{
			byte[] array = new byte[4096];
			ulong num = 0uL;
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			connectionEvent.WaitOne();
			connectionEvent.Reset();
			try
			{
				while (shouldResponse)
				{
					int i = 0;
					int num2 = 0;
					for (; i < 4; i += clientSocket.Receive(array, 4 - i, SocketFlags.None))
					{
					}
					if (i != 4)
					{
						Debug.LogError("Could not read message length prefix!");
					}
					for (int j = 0; j < 4; j++)
					{
						EData.ecdc(ref array[j], num);
						num++;
					}
					num2 = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(array, 0));
					i = 0;
					int num3 = 0;
					VMessage vMessage;
					using (MemoryStream memoryStream = new MemoryStream(4096))
					{
						while (i < num2)
						{
							num3 = ((num2 - i <= array.Length) ? clientSocket.Receive(array, num2 - i, SocketFlags.None) : clientSocket.Receive(array));
							i += num3;
							for (int k = 0; k < num3; k++)
							{
								EData.ecdc(ref array[k], num);
								num++;
							}
							memoryStream.Write(array, 0, num3);
						}
						if (i != num2)
						{
							Debug.LogError("Could not read message!");
						}
						memoryStream.Position = 0L;
						vMessage = aJProtoBufSerializer.Deserialize(memoryStream, null, typeof(VMessage)) as VMessage;
					}
					int messageNumber = vMessage.MessageNumber;
					lock (waitResponse)
					{
						if (waitResponse.ContainsKey(messageNumber))
						{
							ResponseObject responseObject = waitResponse[messageNumber];
							if (responseObject.IsBlocking())
							{
								responseObject.response = vMessage;
								responseObject.link.Set();
							}
							else if (responseObject.consumer != null)
							{
								responseObject.consumer.Consume(vMessage);
							}
							else
							{
								switch (vMessage.ResponseCode)
								{
								case ResponseCode.OK_HANDSHAKE:
									session = vMessage.Session;
									handshakeEvent.Set();
									ServerVersion = vMessage.Version;
									break;
								case ResponseCode.NEW_VERSION_AVAILABLE:
									session = vMessage.Session;
									handshakeEvent.Set();
									ServerVersion = vMessage.Version;
									IncomingMessage = vMessage;
									break;
								default:
									Debug.LogWarning("Going offline because of " + vMessage.ResponseCode);
									ApplicationFocusController.ExecuteLater(ShutdownError);
									handshakeEvent.Set();
									IncomingMessage = vMessage;
									break;
								case ResponseCode.OK:
									break;
								}
							}
							waitResponse.Remove(messageNumber);
							ApplicationFocusController.ExecuteLater(MarkSaved);
						}
						else
						{
							Debug.LogWarning("Unexpected message number received!");
						}
					}
				}
			}
			catch (SocketException)
			{
			}
		}

		public void ResetClientId()
		{
			clientId = Guid.NewGuid().ToString();
			PlayerPrefs.SetString("clientId", clientId);
			LocalStorage.Instance.Clear();
			LocalStorage.Instance.Shutdown();
		}

		private VMessage ConstructMessage(MessageType msgType, params string[] requests)
		{
			VMessage vMessage = new VMessage();
			ResponseObject responseObject = new ResponseObject();
			switch (msgType)
			{
			case MessageType.Global:
				vMessage.GlobalId = globalId;
				break;
			case MessageType.Handshake:
				vMessage.ClientId = clientId;
				vMessage.Version = Version.PROTOCOL_A;
				break;
			default:
				vMessage.ClientId = clientId;
				break;
			case MessageType.Counter:
				break;
			}
			vMessage.Request.AddRange(requests);
			vMessage.MessageNumber = nextMessageNumber;
			responseObject.MessageNumber = nextMessageNumber;
			responseObject.sentAt = DateTime.UtcNow;
			SynchronizedAddResponse(nextMessageNumber++, responseObject);
			return vMessage;
		}

		private void RestoreOnlineState(object o)
		{
			ApplicationFocusController.ExecuteLater(Restore);
		}

		private void VerifyResponses(object o)
		{
			lock (waitResponse)
			{
				bool flag = false;
				foreach (ResponseObject value in waitResponse.Values)
				{
					if (DateTime.UtcNow - value.sentAt > MESSAGE_WAIT_PERIOD_TS)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return;
				}
				Debug.LogWarning("Cloud storage is not responding!");
				ApplicationFocusController.ExecuteLater(ShutdownError);
				onlineChecker.Change(OFFLINE_STAY_PERIOD, TimeSpan.FromMilliseconds(-1.0));
				List<int> list = new List<int>();
				foreach (int key in waitResponse.Keys)
				{
					ResponseObject responseObject = waitResponse[key];
					if (responseObject.consumer != null)
					{
						list.Add(key);
						VMessage vMessage = new VMessage();
						vMessage.LevelInfo = LocalStorage.Instance.LoadLevelInfo(responseObject.queryId);
						responseObject.consumer.Consume(vMessage);
					}
				}
				foreach (int item in list)
				{
					waitResponse.Remove(item);
				}
			}
		}

		public CharacterInfo LoadCharacterInfo()
		{
			if (localNewer)
			{
				CharacterInfo characterInfo = LocalStorage.Instance.LoadCharacterInfo();
				UpdateCharacterInfo(characterInfo);
				return characterInfo;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "CharacterInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			CharacterInfo characterInfo2;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					characterInfo2 = ((response.CharacterInfo != null) ? response.CharacterInfo : LocalStorage.Instance.LoadCharacterInfo());
				}
				else
				{
					characterInfo2 = LocalStorage.Instance.LoadCharacterInfo();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				characterInfo2 = LocalStorage.Instance.LoadCharacterInfo();
			}
			LocalStorage.Instance.UpdateCharacterInfo(characterInfo2);
			return characterInfo2;
		}

		public void UpdateCharacterInfo(CharacterInfo info)
		{
			if (!preventServerUpdates)
			{
				VMessage vMessage = ConstructMessage(MessageType.Client);
				vMessage.CharacterInfo = info;
				ResponseObject responseObject = null;
				if (blockUpdates)
				{
					responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
					responseObject.SetBlocking();
				}
				QueueMessage(vMessage);
				if (blockUpdates && !HandleWait(responseObject))
				{
					Debug.LogWarning("Blocking update failed!");
				}
			}
		}

		public LevelParameters LoadLevelParameters(string levelid)
		{
			return null;
		}

		public LevelInfo LoadLevelInfo(string levelid)
		{
			if (localNewer)
			{
				LevelInfo levelInfo = LocalStorage.Instance.LoadLevelInfo(levelid);
				UpdateLevelInfo(levelInfo, levelid);
				return levelInfo;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "LevelInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			vMessage.SecondaryId = levelid;
			responseObject.SetBlocking();
			responseObject.queryId = levelid;
			LevelInfo levelInfo2;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					if (response.LevelInfo == null)
					{
						levelInfo2 = LocalStorage.Instance.LoadLevelInfo(levelid);
					}
					levelInfo2 = response.LevelInfo;
				}
				else
				{
					levelInfo2 = LocalStorage.Instance.LoadLevelInfo(levelid);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				levelInfo2 = LocalStorage.Instance.LoadLevelInfo(levelid);
			}
			LocalStorage.Instance.UpdateLevelInfo(levelInfo2, levelid);
			return levelInfo2;
		}

		public void LoadLevelInfo(string levelid, IMessageConsumer consumer)
		{
			if (localNewer)
			{
				VMessage vMessage = new VMessage();
				vMessage.LevelInfo = LocalStorage.Instance.LoadLevelInfo(levelid);
				consumer.Consume(vMessage);
				UpdateLevelInfo(vMessage.LevelInfo, levelid);
				return;
			}
			VMessage vMessage2 = ConstructMessage(MessageType.Client, "LevelInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage2.MessageNumber);
			vMessage2.SecondaryId = levelid;
			responseObject.consumer = consumer;
			try
			{
				QueueMessage(vMessage2);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				consumer.Consume(new VMessage());
			}
		}

		public Counter LoadCounter(string id)
		{
			VMessage vMessage = ConstructMessage(MessageType.Counter, "Counter");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			Counter counter = null;
			vMessage.CounterId = id;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					return response.Counter;
				}
				return LocalStorage.Instance.LoadCounter(id);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				return LocalStorage.Instance.LoadCounter(id);
			}
		}

		public List<VehiclePartParameters> LoadVehiclePartParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "VehiclePartParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			List<VehiclePartParameters> list = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					list = response.VehiclePartParameters;
				}
				else
				{
					list = LocalStorage.Instance.LoadVehiclePartParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				list = LocalStorage.Instance.LoadVehiclePartParameters();
			}
			LocalStorage.Instance.UpdateVehiclePartParameters(list);
			return list;
		}

		public CharacterParameters LoadCharacterParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "CharacterParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			CharacterParameters characterParameters = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					characterParameters = response.CharacterParameters;
				}
				else
				{
					characterParameters = LocalStorage.Instance.LoadCharacterParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				characterParameters = LocalStorage.Instance.LoadCharacterParameters();
			}
			LocalStorage.Instance.UpdateCharacterParameters(characterParameters);
			return characterParameters;
		}

		public IAPParameters LoadIAPParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "IAPParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			IAPParameters iAPParameters = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					iAPParameters = response.IAPParameters;
				}
				else
				{
					iAPParameters = LocalStorage.Instance.LoadIAPParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				iAPParameters = LocalStorage.Instance.LoadIAPParameters();
			}
			LocalStorage.Instance.UpdateIAPParameters(iAPParameters);
			return iAPParameters;
		}

		public List<VehicleParameters> LoadVehicleParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "VehicleParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			List<VehicleParameters> list = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					list = response.VehicleParameters;
				}
				else
				{
					list = LocalStorage.Instance.LoadVehicleParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				list = LocalStorage.Instance.LoadVehicleParameters();
			}
			LocalStorage.Instance.UpdateVehicleParameters(list);
			return list;
		}

		public List<CutsceneParameters> LoadCutsceneParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "CutsceneParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			List<CutsceneParameters> list = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					list = response.CutsceneParameters;
				}
				else
				{
					list = LocalStorage.Instance.LoadCutsceneParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				list = LocalStorage.Instance.LoadCutsceneParameters();
			}
			LocalStorage.Instance.UpdateCutsceneParameters(list);
			return list;
		}

		public void UpdateVehiclePartParameters(List<VehiclePartParameters> parameters)
		{
		}

		public VehiclePartInfo LoadVehiclePartInfo(string id)
		{
			if (localNewer)
			{
				VehiclePartInfo vehiclePartInfo = LocalStorage.Instance.LoadVehiclePartInfo(id);
				UpdateVehiclePartInfo(vehiclePartInfo, id);
				return vehiclePartInfo;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "VehiclePartInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			vMessage.SecondaryId = id;
			VehiclePartInfo vehiclePartInfo2 = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					vehiclePartInfo2 = response.VehiclePartInfo;
				}
				else
				{
					vehiclePartInfo2 = LocalStorage.Instance.LoadVehiclePartInfo(id);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				vehiclePartInfo2 = LocalStorage.Instance.LoadVehiclePartInfo(id);
			}
			LocalStorage.Instance.UpdateVehiclePartInfo(vehiclePartInfo2, id);
			return vehiclePartInfo2;
		}

		public void UpdateCounter(string id)
		{
			VMessage vMessage = ConstructMessage(MessageType.Counter);
			vMessage.CounterId = id;
			ResponseObject responseObject = null;
			if (blockUpdates)
			{
				responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
				responseObject.SetBlocking();
			}
			QueueMessage(vMessage);
			if (blockUpdates && !HandleWait(responseObject))
			{
				Debug.LogWarning("Blocking update failed!");
			}
			LocalStorage.Instance.UpdateCounter(id);
		}

		public void UpdateVehiclePartInfo(VehiclePartInfo info, string id)
		{
			if (1 == 0)
			{
				VMessage vMessage = ConstructMessage(MessageType.Client);
				vMessage.SecondaryId = id;
				vMessage.VehiclePartInfo = info;
				ResponseObject responseObject = null;
				if (blockUpdates)
				{
					responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
					responseObject.SetBlocking();
				}
				QueueMessage(vMessage);
				if (blockUpdates && !HandleWait(responseObject))
				{
					Debug.LogWarning("Blocking update failed!");
				}
			}
		}

		public VehicleInfo LoadVehicleInfo(string id)
		{
			if (localNewer)
			{
				VehicleInfo vehicleInfo = LocalStorage.Instance.LoadVehicleInfo(id);
				UpdateVehicleInfo(vehicleInfo, id);
				return vehicleInfo;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "VehicleInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			vMessage.SecondaryId = id;
			VehicleInfo vehicleInfo2 = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					if (response.VehicleInfo == null)
					{
						vehicleInfo2 = new VehicleInfo();
						vehicleInfo2.State = ItemState.Unlocked;
					}
					else
					{
						vehicleInfo2 = response.VehicleInfo;
					}
				}
				else
				{
					vehicleInfo2 = LocalStorage.Instance.LoadVehicleInfo(id);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				vehicleInfo2 = LocalStorage.Instance.LoadVehicleInfo(id);
			}
			LocalStorage.Instance.UpdateVehicleInfo(vehicleInfo2, id);
			return vehicleInfo2;
		}

		public void UpdateVehicleInfo(VehicleInfo info, string id)
		{
			if (!preventServerUpdates)
			{
				VMessage vMessage = ConstructMessage(MessageType.Client);
				vMessage.SecondaryId = id;
				vMessage.VehicleInfo = info;
				ResponseObject responseObject = null;
				if (blockUpdates)
				{
					responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
					responseObject.SetBlocking();
				}
				QueueMessage(vMessage);
				if (blockUpdates && !HandleWait(responseObject))
				{
					Debug.LogWarning("Blocking update failed!");
				}
			}
		}

		public void UpdateLevelInfo(LevelInfo stats, string levelid)
		{
			if (1 == 0)
			{
				VMessage vMessage = ConstructMessage(MessageType.Client);
				vMessage.SecondaryId = levelid;
				vMessage.LevelInfo = stats;
				ResponseObject responseObject = null;
				if (blockUpdates)
				{
					responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
					responseObject.SetBlocking();
				}
				QueueMessage(vMessage);
				if (blockUpdates && !HandleWait(responseObject))
				{
					Debug.LogWarning("Blocking update failed!");
				}
			}
		}

		public List<BundleParameters> LoadBundleParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "BundleParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			List<BundleParameters> list = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					list = response.BundleParameters;
				}
				else
				{
					list = LocalStorage.Instance.LoadBundleParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				list = LocalStorage.Instance.LoadBundleParameters();
			}
			LocalStorage.Instance.UpdateBundleParameters(list);
			return list;
		}

		public void UpdateBundleParameters(List<BundleParameters> parameters)
		{
		}

		public BundleInfo LoadBundleInfo(string id)
		{
			if (localNewer)
			{
				BundleInfo bundleInfo = LocalStorage.Instance.LoadBundleInfo(id);
				UpdateBundleInfo(bundleInfo, id);
				return bundleInfo;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "BundleInfo");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			vMessage.SecondaryId = id;
			BundleInfo bundleInfo2 = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					bundleInfo2 = response.BundleInfo;
					if (bundleInfo2 == null)
					{
						bundleInfo2 = new BundleInfo();
					}
				}
				else
				{
					bundleInfo2 = LocalStorage.Instance.LoadBundleInfo(id);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				bundleInfo2 = LocalStorage.Instance.LoadBundleInfo(id);
			}
			LocalStorage.Instance.UpdateBundleInfo(bundleInfo2, id);
			return bundleInfo2;
		}

		public void UpdateBundleInfo(BundleInfo info, string id)
		{
			if (!preventServerUpdates)
			{
				VMessage vMessage = ConstructMessage(MessageType.Client);
				vMessage.SecondaryId = id;
				vMessage.BundleInfo = info;
				ResponseObject responseObject = null;
				if (blockUpdates)
				{
					responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
					responseObject.SetBlocking();
				}
				QueueMessage(vMessage);
				if (blockUpdates && !HandleWait(responseObject))
				{
					Debug.LogWarning("Blocking update failed!");
				}
			}
		}

		public ScoringParameters LoadScoringParameters()
		{
			VMessage vMessage = ConstructMessage(MessageType.Global, "ScoringParameters");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			ScoringParameters scoringParameters = null;
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					scoringParameters = response.ScoringParameters;
				}
				else
				{
					scoringParameters = LocalStorage.Instance.LoadScoringParameters();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				scoringParameters = LocalStorage.Instance.LoadScoringParameters();
			}
			LocalStorage.Instance.UpdateScoringParameters(scoringParameters);
			return scoringParameters;
		}

		public void UpdateScoringParameters(ScoringParameters parameters)
		{
		}

		public TransactionHistory LoadTransactionHistory()
		{
			if (localNewer)
			{
				TransactionHistory transactionHistory = LocalStorage.Instance.LoadTransactionHistory();
				UpdateTransactionHistory(transactionHistory);
				return transactionHistory;
			}
			VMessage vMessage = ConstructMessage(MessageType.Client, "TransactionHistory");
			ResponseObject responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
			responseObject.SetBlocking();
			TransactionHistory transactionHistory2 = null;
			vMessage.SecondaryId = "transactionhistory";
			try
			{
				QueueMessage(vMessage);
				if (HandleWait(responseObject))
				{
					VMessage response = responseObject.response;
					transactionHistory2 = response.TransactionHistory;
				}
				else
				{
					transactionHistory2 = LocalStorage.Instance.LoadTransactionHistory();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				transactionHistory2 = LocalStorage.Instance.LoadTransactionHistory();
			}
			LocalStorage.Instance.UpdateTransactionHistory(transactionHistory2);
			return transactionHistory2;
		}

		public void UpdateTransactionHistory(TransactionHistory history)
		{
			VMessage vMessage = ConstructMessage(MessageType.Client);
			vMessage.TransactionHistory = history;
			vMessage.SecondaryId = "transactionhistory";
			ResponseObject responseObject = null;
			if (blockUpdates)
			{
				responseObject = SynchronizedGetResponse(vMessage.MessageNumber);
				responseObject.SetBlocking();
			}
			QueueMessage(vMessage);
			if (blockUpdates && !HandleWait(responseObject))
			{
				Debug.LogWarning("Blocking update failed!");
			}
		}
	}
}
