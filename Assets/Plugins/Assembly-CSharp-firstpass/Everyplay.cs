using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using EveryplayMiniJSON;
using UnityEngine;

public class Everyplay : MonoBehaviour
{
	public enum FaceCamPreviewOrigin
	{
		TopLeft = 0,
		TopRight = 1,
		BottomLeft = 2,
		BottomRight = 3
	}

	public enum UserInterfaceIdiom
	{
		Phone = 0,
		Tablet = 1,
		iPhone = 0,
		iPad = 1
	}

	public delegate void WasClosedDelegate();

	public delegate void ReadyForRecordingDelegate(bool enabled);

	public delegate void RecordingStartedDelegate();

	public delegate void RecordingStoppedDelegate();

	public delegate void FaceCamSessionStartedDelegate();

	public delegate void FaceCamRecordingPermissionDelegate(bool granted);

	public delegate void FaceCamSessionStoppedDelegate();

	public delegate void ThumbnailReadyAtFilePathDelegate(string filePath);

	public delegate void ThumbnailReadyAtTextureIdDelegate(int textureId, bool portrait);

	public delegate void UploadDidStartDelegate(int videoId);

	public delegate void UploadDidProgressDelegate(int videoId, float progress);

	public delegate void UploadDidCompleteDelegate(int videoId);

	public delegate void ThumbnailLoadReadyDelegate(Texture2D texture);

	public delegate void ThumbnailLoadFailedDelegate(string error);

	public delegate void RequestReadyDelegate(string response);

	public delegate void RequestFailedDelegate(string error);

	private static string clientId;

	private static bool appIsClosing;

	private static EveryplayLegacy everyplayLegacy;

	private static Everyplay everyplayInstance;

	[Obsolete("Calling Everyplay with SharedInstance is deprecated, you may remove SharedInstance.")]
	public static EveryplayLegacy SharedInstance
	{
		get
		{
			if (EveryplayInstance != null && everyplayLegacy == null)
			{
				everyplayLegacy = everyplayInstance.gameObject.AddComponent<EveryplayLegacy>();
			}
			return everyplayLegacy;
		}
	}

	private static Everyplay EveryplayInstance
	{
		get
		{
			if (everyplayInstance == null && !appIsClosing)
			{
				EveryplaySettings everyplaySettings = (EveryplaySettings)Resources.Load("EveryplaySettings");
				if (everyplaySettings != null && everyplaySettings.IsEnabled)
				{
					GameObject gameObject = new GameObject("Everyplay");
					if (gameObject != null)
					{
						everyplayInstance = gameObject.AddComponent<Everyplay>();
						if (everyplayInstance != null)
						{
							clientId = everyplaySettings.clientId;
							if (everyplaySettings.testButtonsEnabled)
							{
								AddTestButtons(gameObject);
							}
							UnityEngine.Object.DontDestroyOnLoad(gameObject);
						}
					}
				}
			}
			return everyplayInstance;
		}
	}

	[method: MethodImpl(32)]
	public static event WasClosedDelegate WasClosed;

	[method: MethodImpl(32)]
	public static event ReadyForRecordingDelegate ReadyForRecording;

	[method: MethodImpl(32)]
	public static event RecordingStartedDelegate RecordingStarted;

	[method: MethodImpl(32)]
	public static event RecordingStoppedDelegate RecordingStopped;

	[method: MethodImpl(32)]
	public static event FaceCamSessionStartedDelegate FaceCamSessionStarted;

	[method: MethodImpl(32)]
	public static event FaceCamRecordingPermissionDelegate FaceCamRecordingPermission;

	[method: MethodImpl(32)]
	public static event FaceCamSessionStoppedDelegate FaceCamSessionStopped;

	[method: MethodImpl(32)]
	public static event ThumbnailReadyAtFilePathDelegate ThumbnailReadyAtFilePath;

	[method: MethodImpl(32)]
	public static event ThumbnailReadyAtTextureIdDelegate ThumbnailReadyAtTextureId;

	[method: MethodImpl(32)]
	public static event UploadDidStartDelegate UploadDidStart;

	[method: MethodImpl(32)]
	public static event UploadDidProgressDelegate UploadDidProgress;

	[method: MethodImpl(32)]
	public static event UploadDidCompleteDelegate UploadDidComplete;

	public static void Initialize()
	{
		if (EveryplayInstance == null)
		{
			Debug.Log("Unable to initialize Everyplay. Everyplay might be disabled for this platform or the app is closing.");
		}
	}

	public static void Show()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void ShowWithPath(string path)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void PlayVideoWithURL(string url)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void PlayVideoWithDictionary(Dictionary<string, object> dict)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void MakeRequest(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
	{
		if (EveryplayInstance != null)
		{
			EveryplayInstance.AsyncMakeRequest(method, url, data, readyDelegate, failedDelegate);
		}
	}

	public static string AccessToken()
	{
		if (EveryplayInstance != null)
		{
		}
		return null;
	}

	public static void ShowSharingModal()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void StartRecording()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void StopRecording()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void PauseRecording()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void ResumeRecording()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static bool IsRecording()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool IsRecordingSupported()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool IsPaused()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool SnapshotRenderbuffer()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool IsSupported()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool IsSingleCoreDevice()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static int GetUserInterfaceIdiom()
	{
		if (EveryplayInstance != null)
		{
		}
		return 0;
	}

	public static void PlayLastRecording()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetMetadata(string key, object val)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetMetadata(Dictionary<string, object> dict)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetTargetFPS(int fps)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetMotionFactor(int factor)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetMaxRecordingMinutesLength(int minutes)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetLowMemoryDevice(bool state)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetDisableSingleCoreDevices(bool state)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void LoadThumbnailFromFilePath(string filePath, ThumbnailLoadReadyDelegate readyDelegate, ThumbnailLoadFailedDelegate failedDelegate)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static bool FaceCamIsVideoRecordingSupported()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool FaceCamIsAudioRecordingSupported()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool FaceCamIsHeadphonesPluggedIn()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool FaceCamIsSessionRunning()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static bool FaceCamIsRecordingPermissionGranted()
	{
		if (EveryplayInstance != null)
		{
		}
		return false;
	}

	public static float FaceCamAudioPeakLevel()
	{
		if (EveryplayInstance != null)
		{
		}
		return 0f;
	}

	public static float FaceCamAudioPowerLevel()
	{
		if (EveryplayInstance != null)
		{
		}
		return 0f;
	}

	public static void FaceCamSetMonitorAudioLevels(bool enabled)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetAudioOnly(bool audioOnly)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewVisible(bool visible)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewScaleRetina(bool autoScale)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewSideWidth(int width)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewBorderWidth(int width)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewPositionX(int x)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewPositionY(int y)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewBorderColor(float r, float g, float b, float a)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetPreviewOrigin(FaceCamPreviewOrigin origin)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetThumbnailWidth(int thumbnailWidth)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetTargetTextureId(int textureId)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetTargetTextureWidth(int textureWidth)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamSetTargetTextureHeight(int textureHeight)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamStartSession()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamRequestRecordingPermission()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void FaceCamStopSession()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetThumbnailTargetTextureId(int textureId)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetThumbnailTargetTextureWidth(int textureWidth)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void SetThumbnailTargetTextureHeight(int textureHeight)
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	public static void TakeThumbnail()
	{
		if (!(EveryplayInstance != null))
		{
		}
	}

	private static void RemoveAllEventHandlers()
	{
		if (Everyplay.WasClosed != null)
		{
			Delegate[] invocationList = Everyplay.WasClosed.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				WasClosedDelegate value = (WasClosedDelegate)invocationList[i];
				Everyplay.WasClosed = (WasClosedDelegate)Delegate.Remove(Everyplay.WasClosed, value);
			}
		}
		if (Everyplay.ReadyForRecording != null)
		{
			Delegate[] invocationList2 = Everyplay.ReadyForRecording.GetInvocationList();
			for (int j = 0; j < invocationList2.Length; j++)
			{
				ReadyForRecordingDelegate value2 = (ReadyForRecordingDelegate)invocationList2[j];
				Everyplay.ReadyForRecording = (ReadyForRecordingDelegate)Delegate.Remove(Everyplay.ReadyForRecording, value2);
			}
		}
		if (Everyplay.RecordingStarted != null)
		{
			Delegate[] invocationList3 = Everyplay.RecordingStarted.GetInvocationList();
			for (int k = 0; k < invocationList3.Length; k++)
			{
				RecordingStartedDelegate value3 = (RecordingStartedDelegate)invocationList3[k];
				Everyplay.RecordingStarted = (RecordingStartedDelegate)Delegate.Remove(Everyplay.RecordingStarted, value3);
			}
		}
		if (Everyplay.RecordingStopped != null)
		{
			Delegate[] invocationList4 = Everyplay.RecordingStopped.GetInvocationList();
			for (int l = 0; l < invocationList4.Length; l++)
			{
				RecordingStoppedDelegate value4 = (RecordingStoppedDelegate)invocationList4[l];
				Everyplay.RecordingStopped = (RecordingStoppedDelegate)Delegate.Remove(Everyplay.RecordingStopped, value4);
			}
		}
		if (Everyplay.FaceCamSessionStarted != null)
		{
			Delegate[] invocationList5 = Everyplay.FaceCamSessionStarted.GetInvocationList();
			for (int m = 0; m < invocationList5.Length; m++)
			{
				FaceCamSessionStartedDelegate value5 = (FaceCamSessionStartedDelegate)invocationList5[m];
				Everyplay.FaceCamSessionStarted = (FaceCamSessionStartedDelegate)Delegate.Remove(Everyplay.FaceCamSessionStarted, value5);
			}
		}
		if (Everyplay.FaceCamRecordingPermission != null)
		{
			Delegate[] invocationList6 = Everyplay.FaceCamRecordingPermission.GetInvocationList();
			for (int n = 0; n < invocationList6.Length; n++)
			{
				FaceCamRecordingPermissionDelegate value6 = (FaceCamRecordingPermissionDelegate)invocationList6[n];
				Everyplay.FaceCamRecordingPermission = (FaceCamRecordingPermissionDelegate)Delegate.Remove(Everyplay.FaceCamRecordingPermission, value6);
			}
		}
		if (Everyplay.FaceCamSessionStopped != null)
		{
			Delegate[] invocationList7 = Everyplay.FaceCamSessionStopped.GetInvocationList();
			for (int num = 0; num < invocationList7.Length; num++)
			{
				FaceCamSessionStoppedDelegate value7 = (FaceCamSessionStoppedDelegate)invocationList7[num];
				Everyplay.FaceCamSessionStopped = (FaceCamSessionStoppedDelegate)Delegate.Remove(Everyplay.FaceCamSessionStopped, value7);
			}
		}
		if (Everyplay.ThumbnailReadyAtFilePath != null)
		{
			Delegate[] invocationList8 = Everyplay.ThumbnailReadyAtFilePath.GetInvocationList();
			for (int num2 = 0; num2 < invocationList8.Length; num2++)
			{
				ThumbnailReadyAtFilePathDelegate value8 = (ThumbnailReadyAtFilePathDelegate)invocationList8[num2];
				Everyplay.ThumbnailReadyAtFilePath = (ThumbnailReadyAtFilePathDelegate)Delegate.Remove(Everyplay.ThumbnailReadyAtFilePath, value8);
			}
		}
		if (Everyplay.ThumbnailReadyAtTextureId != null)
		{
			Delegate[] invocationList9 = Everyplay.ThumbnailReadyAtTextureId.GetInvocationList();
			for (int num3 = 0; num3 < invocationList9.Length; num3++)
			{
				ThumbnailReadyAtTextureIdDelegate value9 = (ThumbnailReadyAtTextureIdDelegate)invocationList9[num3];
				Everyplay.ThumbnailReadyAtTextureId = (ThumbnailReadyAtTextureIdDelegate)Delegate.Remove(Everyplay.ThumbnailReadyAtTextureId, value9);
			}
		}
		if (Everyplay.UploadDidStart != null)
		{
			Delegate[] invocationList10 = Everyplay.UploadDidStart.GetInvocationList();
			for (int num4 = 0; num4 < invocationList10.Length; num4++)
			{
				UploadDidStartDelegate value10 = (UploadDidStartDelegate)invocationList10[num4];
				Everyplay.UploadDidStart = (UploadDidStartDelegate)Delegate.Remove(Everyplay.UploadDidStart, value10);
			}
		}
		if (Everyplay.UploadDidProgress != null)
		{
			Delegate[] invocationList11 = Everyplay.UploadDidProgress.GetInvocationList();
			for (int num5 = 0; num5 < invocationList11.Length; num5++)
			{
				UploadDidProgressDelegate value11 = (UploadDidProgressDelegate)invocationList11[num5];
				Everyplay.UploadDidProgress = (UploadDidProgressDelegate)Delegate.Remove(Everyplay.UploadDidProgress, value11);
			}
		}
		if (Everyplay.UploadDidComplete != null)
		{
			Delegate[] invocationList12 = Everyplay.UploadDidComplete.GetInvocationList();
			for (int num6 = 0; num6 < invocationList12.Length; num6++)
			{
				UploadDidCompleteDelegate value12 = (UploadDidCompleteDelegate)invocationList12[num6];
				Everyplay.UploadDidComplete = (UploadDidCompleteDelegate)Delegate.Remove(Everyplay.UploadDidComplete, value12);
			}
		}
	}

	private static void AddTestButtons(GameObject gameObject)
	{
		Texture2D texture2D = (Texture2D)Resources.Load("everyplay-test-buttons", typeof(Texture2D));
		if (texture2D != null)
		{
			EveryplayRecButtons everyplayRecButtons = gameObject.AddComponent<EveryplayRecButtons>();
			if (everyplayRecButtons != null)
			{
				everyplayRecButtons.atlasTexture = texture2D;
			}
		}
	}

	private void AsyncLoadThumbnailFromFilePath(string filePath, ThumbnailLoadReadyDelegate readyDelegateMethod, ThumbnailLoadFailedDelegate failedDelegateMethod)
	{
		if (filePath != null)
		{
			StartCoroutine(LoadThumbnailEnumerator(filePath, readyDelegateMethod, failedDelegateMethod));
		}
		else
		{
			failedDelegateMethod("Everyplay error: Thumbnail is not ready.");
		}
	}

	private IEnumerator LoadThumbnailEnumerator(string fileName, ThumbnailLoadReadyDelegate readyDelegateMethod, ThumbnailLoadFailedDelegate failedDelegateMethod)
	{
		WWW www = new WWW("file://" + fileName);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			failedDelegateMethod("Everyplay error: " + www.error);
		}
		else if ((bool)www.texture)
		{
			readyDelegateMethod(www.texture);
		}
		else
		{
			failedDelegateMethod("Everyplay error: Loading thumbnail failed.");
		}
	}

	private void AsyncMakeRequest(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
	{
		StartCoroutine(MakeRequestEnumerator(method, url, data, readyDelegate, failedDelegate));
	}

	private IEnumerator MakeRequestEnumerator(string method, string url, Dictionary<string, object> data, RequestReadyDelegate readyDelegate, RequestFailedDelegate failedDelegate)
	{
		if (data == null)
		{
			data = new Dictionary<string, object>();
		}
		if (url.IndexOf("http") != 0)
		{
			if (url.IndexOf("/") != 0)
			{
				url = "/" + url;
			}
			url = "https://api.everyplay.com" + url;
		}
		method = method.ToLower();
		Dictionary<string, string> headers = new Dictionary<string, string>();
		string accessToken = AccessToken();
		if (accessToken != null)
		{
			headers["Authorization"] = "Bearer " + accessToken;
		}
		else if (url.IndexOf("client_id") == -1)
		{
			url = ((url.IndexOf("?") != -1) ? (url + "&") : (url + "?"));
			url = url + "client_id=" + clientId;
		}
		data.Add("_method", method);
		string dataString = Json.Serialize(data);
		byte[] dataArray = Encoding.UTF8.GetBytes(dataString);
		headers["Accept"] = "application/json";
		headers["Content-Type"] = "application/json";
		headers["Data-Type"] = "json";
		headers["Content-Length"] = dataArray.Length.ToString();
		WWW www = new WWW(url, dataArray, headers);
		yield return www;
		if (!string.IsNullOrEmpty(www.error) && failedDelegate != null)
		{
			failedDelegate("Everyplay error: " + www.error);
		}
		else if (string.IsNullOrEmpty(www.error) && readyDelegate != null)
		{
			readyDelegate(www.text);
		}
	}

	private void OnApplicationQuit()
	{
		RemoveAllEventHandlers();
		appIsClosing = true;
		everyplayInstance = null;
	}

	private void EveryplayHidden(string msg)
	{
		if (Everyplay.WasClosed != null)
		{
			Everyplay.WasClosed();
		}
	}

	private void EveryplayReadyForRecording(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("enabled"))
		{
			int num = Convert.ToInt32(dictionary["enabled"]);
			if (Everyplay.ReadyForRecording != null)
			{
				Everyplay.ReadyForRecording(num == 1);
			}
		}
	}

	private void EveryplayRecordingStarted(string msg)
	{
		if (Everyplay.RecordingStarted != null)
		{
			Everyplay.RecordingStarted();
		}
	}

	private void EveryplayRecordingStopped(string msg)
	{
		if (Everyplay.RecordingStopped != null)
		{
			Everyplay.RecordingStopped();
		}
	}

	private void EveryplayFaceCamSessionStarted(string msg)
	{
		if (Everyplay.FaceCamSessionStarted != null)
		{
			Everyplay.FaceCamSessionStarted();
		}
	}

	private void EveryplayFaceCamRecordingPermission(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("granted"))
		{
			int num = Convert.ToInt32(dictionary["granted"]);
			if (Everyplay.FaceCamRecordingPermission != null)
			{
				Everyplay.FaceCamRecordingPermission(num == 1);
			}
		}
	}

	private void EveryplayFaceCamSessionStopped(string msg)
	{
		if (Everyplay.FaceCamSessionStopped != null)
		{
			Everyplay.FaceCamSessionStopped();
		}
	}

	private void EveryplayThumbnailReadyAtFilePath(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("thumbnailFilePath"))
		{
			string text = (string)dictionary["thumbnailFilePath"];
			if (text != null && Everyplay.ThumbnailReadyAtFilePath != null)
			{
				Everyplay.ThumbnailReadyAtFilePath(text);
			}
		}
	}

	private void EveryplayThumbnailReadyAtTextureId(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("textureId") && dictionary.ContainsKey("portrait"))
		{
			int textureId = Convert.ToInt32(dictionary["textureId"]);
			bool portrait = Convert.ToInt32(dictionary["portrait"]) > 0;
			if (Everyplay.ThumbnailReadyAtTextureId != null)
			{
				Everyplay.ThumbnailReadyAtTextureId(textureId, portrait);
			}
		}
	}

	private void EveryplayUploadDidStart(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("videoId"))
		{
			int videoId = Convert.ToInt32(dictionary["videoId"]);
			if (Everyplay.UploadDidStart != null)
			{
				Everyplay.UploadDidStart(videoId);
			}
		}
	}

	private void EveryplayUploadDidProgress(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("videoId") && dictionary.ContainsKey("progress"))
		{
			int videoId = Convert.ToInt32(dictionary["videoId"]);
			double num = Convert.ToDouble(dictionary["progress"]);
			if (Everyplay.UploadDidProgress != null)
			{
				Everyplay.UploadDidProgress(videoId, (float)num);
			}
		}
	}

	private void EveryplayUploadDidComplete(string jsonMsg)
	{
		if (jsonMsg == null || jsonMsg.Length <= 0)
		{
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(jsonMsg) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("videoId"))
		{
			int videoId = Convert.ToInt32(dictionary["videoId"]);
			if (Everyplay.UploadDidComplete != null)
			{
				Everyplay.UploadDidComplete(videoId);
			}
		}
	}
}
