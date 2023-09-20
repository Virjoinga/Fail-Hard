using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

namespace Game
{
	public class LocalStorage : IStorage
	{
		private class WriteObject
		{
			public string path;

			public byte[] data;

			public WriteObject(string p, byte[] d)
			{
				path = p;
				data = d;
			}
		}

		private const string nameCharacterInfo = "saved_characterinfo";

		private const string nameVehiclePartInfo = "saved_vehiclepartinfo_";

		private const string nameVehicleInfo = "saved_vehicleinfo_";

		private const string nameLevelInfo = "saved_levelinfo_";

		private const string nameCounter = "saved_counter_";

		private const string nameBundleInfo = "saved_bundleinfo_";

		private const string nameVehiclePartParameters = "saved_vehiclepartparameters";

		private const string nameVehicleParameters = "saved_vehicleparameters";

		private const string nameCharacterParameters = "saved_characterparameters";

		private const string nameIAPParameters = "saved_iapparameters";

		private const string nameCutsceneParameters = "saved_cutsceneparameters";

		private const string nameBundleParameters = "saved_bundleparameters";

		private const string nameTransactionHistory = "saved_transactionhistory";

		private const string nameScoringParameters = "saved_scoringparameters";

		private const string pathPrefix = "PreInstalledGameData";

		private string pathCharacterInfo = "/saved_characterinfo.bytes";

		private string pathVehiclePartInfo = "/saved_vehiclepartinfo_";

		private string pathVehicleInfo = "/saved_vehicleinfo_";

		private string pathLevelInfo = "/saved_levelinfo_";

		private string pathCounter = "/saved_counter_";

		private string pathBundleInfo = "/saved_bundleinfo_";

		private string pathVehiclePartParameters = "/saved_vehiclepartparameters.bytes";

		private string pathVehicleParameters = "/saved_vehicleparameters.bytes";

		private string pathCharacterParameters = "/saved_characterparameters.bytes";

		private string pathIAPParameters = "/saved_iapparameters.bytes";

		private string pathCutsceneParameters = "/saved_cutsceneparameters.bytes";

		private string pathBundleParameters = "/saved_bundleparameters.bytes";

		private string pathTransactionHistory = "/saved_transactionhistory.bytes";

		private string pathScoringParameters = "/saved_scoringparameters.bytes";

		private CharacterInfo characterInfo;

		private List<VehiclePartParameters> vehiclePartParameters;

		private List<VehicleParameters> vehicleParameters;

		private CharacterParameters characterParameters;

		private IAPParameters iapParameters;

		private List<CutsceneParameters> cutsceneParameters;

		private List<BundleParameters> bundleParameters;

		private ScoringParameters scoringParameters;

		private Dictionary<string, VehiclePartInfo> vehiclePartInfos = new Dictionary<string, VehiclePartInfo>();

		private Dictionary<string, VehicleInfo> vehicleInfos = new Dictionary<string, VehicleInfo>();

		private Dictionary<string, LevelInfo> levelInfos = new Dictionary<string, LevelInfo>();

		private Dictionary<string, Counter> counters = new Dictionary<string, Counter>();

		private Dictionary<string, BundleInfo> bundleInfos = new Dictionary<string, BundleInfo>();

		private TransactionHistory transactionHistory;

		private bool clearOnShutdown;

		private bool writing;

		private float writeWait;

		private Queue<WriteObject> WriteQueue = new Queue<WriteObject>();

		private Dictionary<string, object> dirtyObjects = new Dictionary<string, object>();

		public static LocalStorage Instance { get; private set; }

		public bool Disabled { get; set; }

		public int SavedAt
		{
			get
			{
				return PlayerPrefs.GetInt("localSaveAt");
			}
			private set
			{
				PlayerPrefs.SetInt("localSaveAt", value);
				PlayerPrefs.Save();
			}
		}

		public static void Init()
		{
			if (Instance == null)
			{
				Instance = new LocalStorage();
				Instance.MoveToInternal();
				string persistentDataPath = Instance.GetPersistentDataPath();
				Instance.pathCharacterInfo = persistentDataPath + Instance.pathCharacterInfo;
				Instance.pathVehiclePartInfo = persistentDataPath + Instance.pathVehiclePartInfo;
				Instance.pathVehicleInfo = persistentDataPath + Instance.pathVehicleInfo;
				Instance.pathLevelInfo = persistentDataPath + Instance.pathLevelInfo;
				Instance.pathCounter = persistentDataPath + Instance.pathCounter;
				Instance.pathVehiclePartParameters = persistentDataPath + Instance.pathVehiclePartParameters;
				Instance.pathVehicleParameters = persistentDataPath + Instance.pathVehicleParameters;
				Instance.pathCharacterParameters = persistentDataPath + Instance.pathCharacterParameters;
				Instance.pathIAPParameters = persistentDataPath + Instance.pathIAPParameters;
				Instance.pathCutsceneParameters = persistentDataPath + Instance.pathCutsceneParameters;
				Instance.pathBundleParameters = persistentDataPath + Instance.pathBundleParameters;
				Instance.pathBundleInfo = persistentDataPath + Instance.pathBundleInfo;
				Instance.pathTransactionHistory = persistentDataPath + Instance.pathTransactionHistory;
				Instance.pathScoringParameters = persistentDataPath + Instance.pathScoringParameters;
				Instance.Setup(false);
			}
		}

		private string GetPersistentDataPath()
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.viimagames.utilities.StorageLocator"))
			{
				return androidJavaObject.CallStatic<string>("getInternal", new object[0]);
			}
		}

		private string GetExternalDataPath()
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.viimagames.utilities.StorageLocator"))
			{
				return androidJavaObject.CallStatic<string>("getExternal", new object[0]);
			}
		}

		public void SaveLocalProtocolVersion(int[] version)
		{
			PlayerPrefs.SetInt("ProtocolVersionMajor", version[0]);
			PlayerPrefs.SetInt("ProtocolVersionMinor", version[1]);
			PlayerPrefs.Save();
		}

		public void Setup(bool forceWrite)
		{
			if (PlayerPrefs.HasKey("ProtocolVersionMajor") && PlayerPrefs.HasKey("ProtocolVersionMinor"))
			{
				int @int = PlayerPrefs.GetInt("ProtocolVersionMajor");
				int int2 = PlayerPrefs.GetInt("ProtocolVersionMinor");
				if (Version.PROTOCOL_A[0] > int2 || Version.PROTOCOL_A[1] > @int)
				{
					forceWrite = true;
				}
			}
			else
			{
				forceWrite = true;
			}
			if (!File.Exists(pathVehiclePartParameters) || forceWrite)
			{
				File.WriteAllBytes(pathVehiclePartParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_vehiclepartparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathVehicleParameters) || forceWrite)
			{
				File.WriteAllBytes(pathVehicleParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_vehicleparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathCharacterParameters) || forceWrite)
			{
				File.WriteAllBytes(pathCharacterParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_characterparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathIAPParameters) || forceWrite)
			{
				File.WriteAllBytes(pathIAPParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_iapparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathCutsceneParameters) || forceWrite)
			{
				File.WriteAllBytes(pathCutsceneParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_cutsceneparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathBundleParameters) || forceWrite)
			{
				File.WriteAllBytes(pathBundleParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_bundleparameters", typeof(TextAsset))).bytes);
			}
			if (!File.Exists(pathScoringParameters) || forceWrite)
			{
				File.WriteAllBytes(pathScoringParameters, ((TextAsset)Resources.Load("PreInstalledGameData/saved_scoringparameters", typeof(TextAsset))).bytes);
			}
			SaveLocalProtocolVersion(Version.PROTOCOL_A);
		}

		private void MoveToInternal()
		{
			string path = GetPersistentDataPath() + "/saved_characterinfo.bytes";
			if (File.Exists(path))
			{
				return;
			}
			string path2 = GetExternalDataPath() + "/saved_characterinfo.bytes";
			if (File.Exists(path2))
			{
				FileInfo[] files = new DirectoryInfo(GetExternalDataPath()).GetFiles("saved_*.bytes");
				foreach (FileInfo fileInfo in files)
				{
					string path3 = GetPersistentDataPath() + "/" + fileInfo.Name;
					byte[] bytes = File.ReadAllBytes(fileInfo.FullName);
					File.WriteAllBytes(path3, bytes);
				}
			}
		}

		public void Clear()
		{
			clearOnShutdown = true;
		}

		public void Shutdown()
		{
			if (clearOnShutdown)
			{
				FileInfo[] files = new DirectoryInfo(Application.persistentDataPath).GetFiles("saved_*.bytes");
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.Delete();
				}
				return;
			}
			writeWait = 0f;
			WriteQueue.Clear();
			IEnumerator enumerator = ParallelSync();
			while (enumerator.MoveNext())
			{
			}
			enumerator = StartWriting();
			while (enumerator.MoveNext())
			{
			}
		}

		public IEnumerator ParallelSync()
		{
			if (characterInfo != null)
			{
				SerializeObject(characterInfo, pathCharacterInfo);
			}
			yield return true;
			if (vehiclePartParameters != null)
			{
				SerializeObject(vehiclePartParameters, pathVehiclePartParameters);
			}
			yield return true;
			if (vehicleParameters != null)
			{
				SerializeObject(vehicleParameters, pathVehicleParameters);
			}
			yield return true;
			if (characterParameters != null)
			{
				SerializeObject(characterParameters, pathCharacterParameters);
			}
			yield return true;
			if (iapParameters != null)
			{
				SerializeObject(iapParameters, pathIAPParameters);
			}
			yield return true;
			if (cutsceneParameters != null)
			{
				SerializeObject(cutsceneParameters, pathCutsceneParameters);
			}
			yield return true;
			if (bundleParameters != null)
			{
				SerializeObject(bundleParameters, pathBundleParameters);
			}
			yield return true;
			if (scoringParameters != null)
			{
				SerializeObject(scoringParameters, pathScoringParameters);
			}
			yield return true;
			if (transactionHistory != null)
			{
				SerializeObject(transactionHistory, pathTransactionHistory);
			}
			yield return true;
			lock (vehiclePartInfos)
			{
				foreach (string k in vehiclePartInfos.Keys)
				{
					SerializeObject(vehiclePartInfos[k], pathVehiclePartInfo + k + ".bytes");
				}
			}
			yield return true;
			lock (vehicleInfos)
			{
				foreach (string m in vehicleInfos.Keys)
				{
					SerializeObject(vehicleInfos[m], pathVehicleInfo + m + ".bytes");
				}
			}
			yield return true;
			lock (levelInfos)
			{
				foreach (string l in levelInfos.Keys)
				{
					SerializeObject(levelInfos[l], pathLevelInfo + l + ".bytes");
				}
			}
			yield return true;
			lock (counters)
			{
				foreach (string j in counters.Keys)
				{
					SerializeObject(counters[j], pathCounter + j + ".bytes");
				}
			}
			yield return true;
			lock (bundleInfos)
			{
				foreach (string i in bundleInfos.Keys)
				{
					SerializeObject(bundleInfos[i], pathBundleInfo + i + ".bytes");
				}
			}
			yield return true;
			SavedAt = Storage.UnixTimeNow;
		}

		public void SyncToDisk()
		{
			if (!Disabled)
			{
				writeWait = 0.06f;
				WriteQueue.Clear();
				ApplicationFocusController.BeginCoroutine(ParallelSync());
			}
		}

		public void ClearDirty()
		{
			dirtyObjects.Clear();
		}

		public void SyncDirtyToDisk()
		{
			if (Disabled)
			{
				return;
			}
			writeWait = 0.06f;
			foreach (KeyValuePair<string, object> dirtyObject in dirtyObjects)
			{
				SerializeObject(dirtyObject.Value, dirtyObject.Key);
			}
			dirtyObjects.Clear();
		}

		public void Restore()
		{
		}

		public CharacterInfo LoadCharacterInfo()
		{
			CharacterInfo characterInfo = null;
			if (File.Exists(pathCharacterInfo))
			{
				characterInfo = ReadFile<CharacterInfo>(pathCharacterInfo);
			}
			else
			{
				characterInfo = new CharacterInfo();
				CharacterParameters characterParameters = CloudStorageST.Instance.LoadCharacterParameters();
				characterInfo.Coins = characterParameters.Coins;
			}
			UpdateCharacterInfo(characterInfo);
			return characterInfo;
		}

		public LevelParameters LoadLevelParameters(string levelid)
		{
			Debug.LogWarning("Should not be used (yet)!");
			return null;
		}

		public LevelInfo LoadLevelInfo(string levelid)
		{
			LevelInfo levelInfo = null;
			if (File.Exists(pathLevelInfo + levelid + ".bytes"))
			{
				levelInfo = ReadFile<LevelInfo>(pathLevelInfo + levelid + ".bytes");
			}
			else
			{
				levelInfo = new LevelInfo();
				levelInfo.IsLocked = true;
				levelInfo.IsCompleted = false;
				levelInfo.UnlockingIndex = 9999;
			}
			UpdateLevelInfo(levelInfo, levelid);
			return levelInfo;
		}

		public void LoadLevelInfo(string levelid, IMessageConsumer consumer)
		{
			VMessage vMessage = new VMessage();
			vMessage.LevelInfo = LoadLevelInfo(levelid);
			consumer.Consume(vMessage);
		}

		public List<VehiclePartParameters> LoadVehiclePartParameters()
		{
			List<VehiclePartParameters> list = null;
			if (File.Exists(pathVehiclePartParameters))
			{
				list = ReadFile<List<VehiclePartParameters>>(pathVehiclePartParameters);
			}
			else
			{
				Debug.LogWarning("List of vehicle part parameters should always be present!");
				Setup(false);
				list = ReadFile<List<VehiclePartParameters>>(pathVehiclePartParameters);
			}
			UpdateVehiclePartParameters(list);
			return list;
		}

		public List<VehicleParameters> LoadVehicleParameters()
		{
			List<VehicleParameters> list = null;
			if (File.Exists(pathVehicleParameters))
			{
				list = ReadFile<List<VehicleParameters>>(pathVehicleParameters);
			}
			else
			{
				Debug.LogWarning("List of vehicle parameters should always be present!");
				Setup(false);
				list = ReadFile<List<VehicleParameters>>(pathVehicleParameters);
			}
			UpdateVehicleParameters(list);
			return list;
		}

		public CharacterParameters LoadCharacterParameters()
		{
			CharacterParameters characterParameters = null;
			if (File.Exists(pathCharacterParameters))
			{
				characterParameters = ReadFile<CharacterParameters>(pathCharacterParameters);
			}
			else
			{
				Debug.LogWarning("Character parameters should always be present!");
				Setup(false);
				characterParameters = ReadFile<CharacterParameters>(pathCharacterParameters);
			}
			UpdateCharacterParameters(characterParameters);
			return characterParameters;
		}

		public IAPParameters LoadIAPParameters()
		{
			IAPParameters iAPParameters = null;
			if (File.Exists(pathIAPParameters))
			{
				iAPParameters = ReadFile<IAPParameters>(pathIAPParameters);
			}
			else
			{
				Debug.LogWarning("IAP parameters should always be present!");
				Setup(false);
				iAPParameters = ReadFile<IAPParameters>(pathIAPParameters);
			}
			UpdateIAPParameters(iAPParameters);
			return iAPParameters;
		}

		public VehiclePartInfo LoadVehiclePartInfo(string id)
		{
			VehiclePartInfo vehiclePartInfo = null;
			vehiclePartInfo = ((!File.Exists(pathVehiclePartInfo + id + ".bytes")) ? new VehiclePartInfo() : ReadFile<VehiclePartInfo>(pathVehiclePartInfo + id + ".bytes"));
			UpdateVehiclePartInfo(vehiclePartInfo, id);
			return vehiclePartInfo;
		}

		public VehicleInfo LoadVehicleInfo(string id)
		{
			VehicleInfo vehicleInfo = null;
			if (File.Exists(pathVehicleInfo + id + ".bytes"))
			{
				vehicleInfo = ReadFile<VehicleInfo>(pathVehicleInfo + id + ".bytes");
			}
			else
			{
				vehicleInfo = new VehicleInfo();
				vehicleInfo.State = ItemState.Unlocked;
			}
			UpdateVehicleInfo(vehicleInfo, id);
			return vehicleInfo;
		}

		public BundleInfo LoadBundleInfo(string id)
		{
			BundleInfo bundleInfo = null;
			bundleInfo = ((!File.Exists(pathBundleInfo + id + ".bytes")) ? new BundleInfo() : ReadFile<BundleInfo>(pathBundleInfo + id + ".bytes"));
			UpdateBundleInfo(bundleInfo, id);
			return bundleInfo;
		}

		public List<CutsceneParameters> LoadCutsceneParameters()
		{
			List<CutsceneParameters> list = null;
			if (File.Exists(pathCutsceneParameters))
			{
				list = ReadFile<List<CutsceneParameters>>(pathCutsceneParameters);
			}
			else
			{
				Debug.LogWarning("List of cut scene parameters should always be present!");
				Setup(false);
				list = ReadFile<List<CutsceneParameters>>(pathCutsceneParameters);
			}
			UpdateCutsceneParameters(list);
			return list;
		}

		public List<BundleParameters> LoadBundleParameters()
		{
			List<BundleParameters> list = null;
			if (File.Exists(pathBundleParameters))
			{
				list = ReadFile<List<BundleParameters>>(pathBundleParameters);
			}
			else
			{
				Debug.LogWarning("List of level bundle parameters should always be present!");
				Setup(false);
				list = ReadFile<List<BundleParameters>>(pathBundleParameters);
			}
			UpdateBundleParameters(list);
			return list;
		}

		public TransactionHistory LoadTransactionHistory()
		{
			if (transactionHistory == null)
			{
				if (File.Exists(pathTransactionHistory))
				{
					transactionHistory = ReadFile<TransactionHistory>(pathTransactionHistory);
				}
				else
				{
					transactionHistory = new TransactionHistory();
				}
			}
			return transactionHistory;
		}

		public Counter LoadCounter(string id)
		{
			Counter counter = null;
			if (File.Exists(pathCounter + id + ".bytes"))
			{
				return ReadFile<Counter>(pathCounter + id + ".bytes");
			}
			return new Counter();
		}

		private void AddDirty(string key, object o)
		{
			if (!dirtyObjects.ContainsKey(key))
			{
				dirtyObjects.Add(key, o);
			}
		}

		public void UpdateCounter(string id)
		{
			lock (counters)
			{
				if (counters.ContainsKey(id))
				{
					counters[id].Value++;
				}
				else
				{
					counters.Add(id, new Counter());
				}
				AddDirty(pathCounter + id + ".bytes", counters[id]);
			}
		}

		public void UpdateBundleParameters(List<BundleParameters> parameters)
		{
			if (bundleParameters != null)
			{
				if (!object.ReferenceEquals(parameters, bundleParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					bundleParameters = parameters;
				}
			}
			else
			{
				bundleParameters = parameters;
			}
		}

		public void UpdateVehiclePartParameters(List<VehiclePartParameters> parameters)
		{
			if (vehiclePartParameters != null)
			{
				if (!object.ReferenceEquals(parameters, vehiclePartParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					vehiclePartParameters = parameters;
				}
			}
			else
			{
				vehiclePartParameters = parameters;
			}
		}

		public void UpdateVehicleParameters(List<VehicleParameters> parameters)
		{
			if (vehicleParameters != null)
			{
				if (!object.ReferenceEquals(parameters, vehicleParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					vehicleParameters = parameters;
				}
			}
			else
			{
				vehicleParameters = parameters;
			}
		}

		public void UpdateCharacterParameters(CharacterParameters parameters)
		{
			if (characterParameters != null)
			{
				if (!object.ReferenceEquals(parameters, characterParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					characterParameters = parameters;
				}
			}
			else
			{
				characterParameters = parameters;
			}
		}

		public void UpdateIAPParameters(IAPParameters parameters)
		{
			if (iapParameters != null)
			{
				if (!object.ReferenceEquals(parameters, iapParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					iapParameters = parameters;
				}
			}
			else
			{
				iapParameters = parameters;
			}
		}

		public void UpdateCutsceneParameters(List<CutsceneParameters> parameters)
		{
			if (cutsceneParameters != null)
			{
				if (!object.ReferenceEquals(parameters, cutsceneParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					cutsceneParameters = parameters;
				}
			}
			else
			{
				cutsceneParameters = parameters;
			}
		}

		public void UpdateVehiclePartInfo(VehiclePartInfo info, string id)
		{
			if (info == null)
			{
				return;
			}
			lock (vehiclePartInfos)
			{
				if (vehiclePartInfos.ContainsKey(id))
				{
					if (!object.ReferenceEquals(info, vehiclePartInfos[id]))
					{
						Debug.LogWarning("Overriding existing instance!");
						vehiclePartInfos.Remove(id);
						vehiclePartInfos.Add(id, info);
					}
				}
				else
				{
					vehiclePartInfos.Add(id, info);
				}
				AddDirty(pathVehiclePartInfo + id + ".bytes", info);
			}
		}

		public void UpdateVehicleInfo(VehicleInfo info, string id)
		{
			if (info == null)
			{
				return;
			}
			lock (vehicleInfos)
			{
				if (vehicleInfos.ContainsKey(id))
				{
					if (!object.ReferenceEquals(info, vehicleInfos[id]))
					{
						Debug.LogWarning("Overriding existing instance!");
						vehicleInfos.Remove(id);
						vehicleInfos.Add(id, info);
					}
				}
				else
				{
					vehicleInfos.Add(id, info);
				}
				AddDirty(pathVehicleInfo + id + ".bytes", info);
			}
		}

		public void UpdateBundleInfo(BundleInfo info, string id)
		{
			if (info == null)
			{
				return;
			}
			lock (bundleInfos)
			{
				if (bundleInfos.ContainsKey(id))
				{
					if (!object.ReferenceEquals(info, bundleInfos[id]))
					{
						Debug.LogWarning("Overriding existing instance!");
						bundleInfos.Remove(id);
						bundleInfos.Add(id, info);
					}
				}
				else
				{
					bundleInfos.Add(id, info);
				}
				AddDirty(pathBundleInfo + id + ".bytes", info);
			}
		}

		public void UpdateLevelInfo(LevelInfo info, string id)
		{
			if (info == null)
			{
				return;
			}
			lock (levelInfos)
			{
				if (levelInfos.ContainsKey(id))
				{
					if (!object.ReferenceEquals(info, levelInfos[id]))
					{
						Debug.LogWarning("Overriding existing instance!");
						levelInfos.Remove(id);
						levelInfos.Add(id, info);
					}
				}
				else
				{
					levelInfos.Add(id, info);
				}
				AddDirty(pathLevelInfo + id + ".bytes", info);
			}
		}

		public void UpdateCharacterInfo(CharacterInfo info)
		{
			if (characterInfo != null)
			{
				if (!object.ReferenceEquals(info, characterInfo))
				{
					Debug.LogWarning("Overriding existing instance!");
					characterInfo = info;
				}
			}
			else
			{
				characterInfo = info;
			}
			AddDirty(pathCharacterInfo, info);
		}

		public void UpdateTransactionHistory(TransactionHistory history)
		{
			if (transactionHistory != null)
			{
				if (!object.ReferenceEquals(history, transactionHistory))
				{
					transactionHistory = history;
				}
			}
			else
			{
				transactionHistory = history;
			}
			AddDirty(pathTransactionHistory, transactionHistory);
		}

		public ScoringParameters LoadScoringParameters()
		{
			ScoringParameters scoringParameters = null;
			if (File.Exists(pathScoringParameters))
			{
				scoringParameters = ReadFile<ScoringParameters>(pathScoringParameters);
			}
			else
			{
				Debug.LogWarning("Scoring parameters should always be present!");
			}
			UpdateScoringParameters(scoringParameters);
			return scoringParameters;
		}

		public void UpdateScoringParameters(ScoringParameters parameters)
		{
			if (scoringParameters != null)
			{
				if (!object.ReferenceEquals(parameters, scoringParameters))
				{
					Debug.LogWarning("Overriding existing instance!");
					scoringParameters = parameters;
				}
			}
			else
			{
				scoringParameters = parameters;
			}
		}

		private T ReadFile<T>(string path)
		{
			T val = default(T);
			byte[] array = File.ReadAllBytes(path);
			ulong num = 0uL;
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				EData.ecdc(ref array[i], num);
				num++;
			}
			MemoryStream memoryStream = new MemoryStream(array);
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			try
			{
				val = (T)aJProtoBufSerializer.Deserialize(memoryStream, null, typeof(T));
				if (val == null)
				{
					flag = true;
				}
			}
			catch (ProtoException ex)
			{
				Debug.LogWarning(ex.ToString());
				flag = true;
			}
			if (flag)
			{
				memoryStream.Close();
				Setup(true);
				array = File.ReadAllBytes(path);
				num = 0uL;
				for (int j = 0; j < array.Length; j++)
				{
					EData.ecdc(ref array[j], num);
					num++;
				}
				memoryStream = new MemoryStream(array);
				val = (T)aJProtoBufSerializer.Deserialize(memoryStream, null, typeof(T));
			}
			memoryStream.Close();
			return val;
		}

		private void SerializeObject(object obj, string path)
		{
			MemoryStream memoryStream = new MemoryStream(2048);
			AJProtoBufSerializer aJProtoBufSerializer = new AJProtoBufSerializer();
			ulong num = 0uL;
			aJProtoBufSerializer.Serialize(memoryStream, obj);
			byte[] array = memoryStream.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				EData.ecdc(ref array[i], num);
				num++;
			}
			WriteQueue.Enqueue(new WriteObject(path, array));
			if (!writing && writeWait > 0f)
			{
				ApplicationFocusController.BeginCoroutine(StartWriting());
			}
		}

		private IEnumerator StartWriting()
		{
			writing = true;
			while (WriteQueue.Count > 0)
			{
				WriteObject temp = WriteQueue.Dequeue();
				File.WriteAllBytes(temp.path + ".writing", temp.data);
				if (File.Exists(temp.path))
				{
					File.Delete(temp.path);
				}
				File.Move(temp.path + ".writing", temp.path);
				if (writeWait > 0f)
				{
					yield return new WaitForSeconds(writeWait);
				}
			}
			writing = false;
		}
	}
}
