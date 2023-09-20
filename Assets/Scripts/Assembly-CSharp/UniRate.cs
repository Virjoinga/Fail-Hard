using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRateMiniJSON;
using UnityEngine;

public class UniRate : MonoBehaviour
{
	public enum Error
	{
		BundleIdDoesNotMatchAppStore = 0,
		AppNotFoundOnAppStore = 1,
		NotTheLatestVersion = 2,
		NetworkError = 3
	}

	public delegate void OnUniRateFaildDelegate(Error error);

	public delegate void OnDetectAppUpdatedDelegate();

	public delegate bool ShouldUniRatePromptForRatingDelegate();

	public delegate void OnPromptedForRatingDelegate();

	public delegate void OnUserAttemptToRateDelegate();

	public delegate void OnUserDeclinedToRateDelegate();

	public delegate void OnUserWantReminderToRateDelegate();

	public delegate bool ShouldUniRateOpenRatePageDelegate();

	private const string kUniRateRatedVersionKey = "UniRateRatedVersionChecked";

	private const string kUniRateDeclinedVersionKey = "UniRateDeclinedVersion";

	private const string kUniRateLastRemindedKey = "UniRateLastReminded";

	private const string kUniRateLastVersionUsedKey = "UniRateLastVersionUsed";

	private const string kUniRateFirstUsedKey = "UniRateFirstUsed";

	private const string kUniRateUseCountKey = "UniRateUseCount";

	private const string kUniRateEventCountKey = "UniRateEventCount";

	private const string kUniRateAppLookupURLFormat = "http://itunes.apple.com/{0}lookup";

	private const string kUniRateiOSAppStoreURLFormat = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=";

	private const string kUniRateiOS7AppStoreURLFormat = "itms-apps://itunes.apple.com/app/id";

	private const string kUniRateAndroidMarketURLFormat = "market://details?id=";

	private const string kUniRateAmazonAppstoreURLFormat = "amzn://apps/android?p=";

	private const string kDefaultTitle = "Rate {0}";

	private const string kDefaultMessage = "If you enjoy {0}, would you mind taking a moment to rate it? It will not take more than a minute. Thanks for your support!";

	private const string kDefaultCancelBtnTitle = "No, thanks";

	private const string kDefaultRateBtnTitle = "Rate it now";

	private const string kDefaultRemindBtnTitle = "Remind me later";

	private const float SECONDS_IN_A_DAY = 86400f;

	private const float SECONDS_IN_A_WEEK = 604800f;

	public int appStoreID;

	public string marketPackageName;

	public UniRateMarketType marketType;

	public string appStoreCountry;

	public string applicationName;

	public string applicationVersion;

	public string applicationBundleID;

	public int usesUntilPrompt = 10;

	public int eventsUntilPrompt;

	public float daysUntilPrompt = 3f;

	public float usesPerWeekForPrompt;

	public float remindPeriod = 1f;

	public string messageTitle = string.Empty;

	public string message = string.Empty;

	public string cancelButtonLabel = string.Empty;

	public string remindButtonLabel = string.Empty;

	public string rateButtonLabel = string.Empty;

	public bool onlyPromptIfLatestVersion = true;

	public bool promptAgainForEachNewVersion = true;

	public bool promptAtLaunch = true;

	public bool useCustomizedPromptView;

	public bool autoLocalization = true;

	public bool previewMode;

	[SerializeField]
	private string _ratingIOSURL;

	[SerializeField]
	private string _ratingAndroidURL;

	private bool _currentChecking;

	private bool _promptShowing;

	private Dictionary<string, object> _localizationDic;

	private static UniRate _instance;

	public string MessageTitle
	{
		get
		{
			return (!string.IsNullOrEmpty(messageTitle)) ? messageTitle : GetLocalizedMessageTitle();
		}
	}

	public string Message
	{
		get
		{
			return (!string.IsNullOrEmpty(message)) ? message : GetLocalizedMessage();
		}
	}

	public string CancelButtonLabel
	{
		get
		{
			return (!string.IsNullOrEmpty(cancelButtonLabel)) ? cancelButtonLabel : GetLocalizedCancelButtonLabel();
		}
	}

	public string RemindButtonLabel
	{
		get
		{
			return (!string.IsNullOrEmpty(remindButtonLabel)) ? remindButtonLabel : GetLocalizedRemindButtonLabel();
		}
	}

	public string RateButtonLabel
	{
		get
		{
			return (!string.IsNullOrEmpty(rateButtonLabel)) ? rateButtonLabel : GetLocalizedRateButtonLabel();
		}
	}

	public string RatingIOSURL
	{
		get
		{
			if (!string.IsNullOrEmpty(_ratingIOSURL))
			{
				return _ratingIOSURL;
			}
			if (appStoreID == 0)
			{
				Debug.LogWarning("UniRate does not find your App Store ID");
			}
			return (!(iOSVersion >= 7f)) ? ("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=" + appStoreID) : ("itms-apps://itunes.apple.com/app/id" + appStoreID);
		}
	}

	public string RatingAndroidURL
	{
		get
		{
			if (!string.IsNullOrEmpty(_ratingAndroidURL))
			{
				return _ratingAndroidURL;
			}
			string text = string.Empty;
			switch (marketType)
			{
			case UniRateMarketType.GooglePlay:
				text = "market://details?id=";
				break;
			case UniRateMarketType.AmazonAppstore:
				text = "amzn://apps/android?p=";
				break;
			}
			return text + marketPackageName;
		}
	}

	public DateTime firstUsed
	{
		get
		{
			return UniRatePlayerPrefs.GetDate("UniRateFirstUsed");
		}
		set
		{
			UniRatePlayerPrefs.SetDate("UniRateFirstUsed", value);
			PlayerPrefs.Save();
		}
	}

	public DateTime lastReminded
	{
		get
		{
			return UniRatePlayerPrefs.GetDate("UniRateLastReminded");
		}
		set
		{
			UniRatePlayerPrefs.SetDate("UniRateLastReminded", value);
			PlayerPrefs.Save();
		}
	}

	public int usesCount
	{
		get
		{
			return PlayerPrefs.GetInt("UniRateUseCount");
		}
		set
		{
			PlayerPrefs.SetInt("UniRateUseCount", value);
			PlayerPrefs.Save();
		}
	}

	public int eventCount
	{
		get
		{
			return PlayerPrefs.GetInt("UniRateEventCount");
		}
		set
		{
			PlayerPrefs.SetInt("UniRateEventCount", value);
			PlayerPrefs.Save();
		}
	}

	public float usesPerWeek
	{
		get
		{
			return (float)((double)usesCount / ((DateTime.Now - firstUsed).TotalSeconds / 604800.0));
		}
	}

	public bool declinedThisVersion
	{
		get
		{
			if (!string.IsNullOrEmpty(applicationVersion))
			{
				return string.Equals(PlayerPrefs.GetString("UniRateDeclinedVersion"), applicationVersion);
			}
			return false;
		}
		set
		{
			PlayerPrefs.SetString("UniRateDeclinedVersion", (!value) ? string.Empty : applicationVersion);
			PlayerPrefs.Save();
		}
	}

	public bool declinedAnyVersion
	{
		get
		{
			return !string.IsNullOrEmpty(PlayerPrefs.GetString("UniRateDeclinedVersion"));
		}
	}

	public bool ratedThisVersion
	{
		get
		{
			return PlayerPrefs.GetInt("UniRateRatedVersionChecked") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("UniRateRatedVersionChecked", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public bool ratedAnyVersion
	{
		get
		{
			return !string.IsNullOrEmpty(PlayerPrefs.GetString("UniRateRatedVersionChecked"));
		}
	}

	public float usedDays
	{
		get
		{
			return (float)(DateTime.Now - firstUsed).TotalSeconds / 86400f;
		}
	}

	public bool waitingByRemindLater
	{
		get
		{
			return lastReminded != DateTime.MaxValue;
		}
	}

	public float leftRemindDays
	{
		get
		{
			return (float)((double)remindPeriod - (DateTime.Now - lastReminded).TotalSeconds / 86400.0);
		}
	}

	private Dictionary<string, object> localizationDic
	{
		get
		{
			if (_localizationDic == null)
			{
				TextAsset textAsset = (TextAsset)Resources.Load("UniRateLocalizationStrings", typeof(TextAsset));
				_localizationDic = Json.Deserialize(textAsset.text) as Dictionary<string, object>;
			}
			return _localizationDic;
		}
	}

	public static UniRate Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType(typeof(UniRate)) as UniRate;
				if (!_instance)
				{
					GameObject gameObject = new GameObject("UniRateManager");
					_instance = gameObject.AddComponent<UniRate>();
				}
				else
				{
					_instance.gameObject.name = "UniRateManager";
				}
				UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
				UniRatePlugin.InitUniRateAndroid();
			}
			return _instance;
		}
	}

	private float iOSVersion
	{
		get
		{
			float result = -1f;
			string text = SystemInfo.operatingSystem.Replace("iPhone OS ", string.Empty);
			float.TryParse(text.Substring(0, 1), out result);
			return result;
		}
	}

	[method: MethodImpl(32)]
	public event OnUniRateFaildDelegate OnUniRateFaild;

	[method: MethodImpl(32)]
	public event OnDetectAppUpdatedDelegate OnDetectAppUpdated;

	[method: MethodImpl(32)]
	public event ShouldUniRatePromptForRatingDelegate ShouldUniRatePromptForRating;

	[method: MethodImpl(32)]
	public event OnPromptedForRatingDelegate OnPromptedForRating;

	[method: MethodImpl(32)]
	public event OnUserAttemptToRateDelegate OnUserAttemptToRate;

	[method: MethodImpl(32)]
	public event OnUserDeclinedToRateDelegate OnUserDeclinedToRate;

	[method: MethodImpl(32)]
	public event OnUserWantReminderToRateDelegate OnUserWantReminderToRate;

	[method: MethodImpl(32)]
	public event ShouldUniRateOpenRatePageDelegate ShouldUniRateOpenRatePage;

	public bool ShouldPromptForRating()
	{
		if (previewMode)
		{
			Debug.Log("UniRate is in preview mode. Make sure you set previewMode to false when release.");
			return true;
		}
		if (ratedThisVersion)
		{
			UniRateDebug.Log("Not prompt. The user has already rated this version");
			return false;
		}
		if (!promptAgainForEachNewVersion && ratedAnyVersion)
		{
			UniRateDebug.Log("Not prompt. The user has already rated for some version and promptAgainForEachNewVersion is disabled");
			return false;
		}
		if (declinedThisVersion)
		{
			UniRateDebug.Log("Not prompt. The user refused to rate this version");
			return false;
		}
		if ((daysUntilPrompt > 0f || usesPerWeekForPrompt != 0f) && firstUsed == DateTime.MaxValue)
		{
			UniRateDebug.Log("Not prompt. First launch");
			return false;
		}
		if ((DateTime.Now - firstUsed).TotalSeconds < (double)(daysUntilPrompt * 86400f))
		{
			UniRateDebug.Log("Not prompt. App was used less than " + daysUntilPrompt + " days ago");
			return false;
		}
		if (usesCount < usesUntilPrompt)
		{
			UniRateDebug.Log("Not prompt. App was only used " + usesCount + " times");
			return false;
		}
		if (eventCount < eventsUntilPrompt)
		{
			UniRateDebug.Log("Not prompt. Only " + eventCount + " times of events logged");
			return false;
		}
		if (usesPerWeek < usesPerWeekForPrompt)
		{
			UniRateDebug.Log("Not prompt. Only used " + usesPerWeek + " times per week");
			return false;
		}
		if (lastReminded != DateTime.MaxValue && (DateTime.Now - lastReminded).TotalSeconds < (double)(remindPeriod * 86400f))
		{
			UniRateDebug.Log("Not prompt. The user askd to be reminded and it is not the time now");
			return false;
		}
		return true;
	}

	public void PromptIfNetworkAvailable()
	{
		CheckAndReadyToRate(true);
	}

	public void RateIfNetworkAvailable()
	{
		CheckAndReadyToRate(false);
	}

	public void ShowPrompt()
	{
		UniRateDebug.Log("It's time to show prompt");
		if (this.OnPromptedForRating != null)
		{
			this.OnPromptedForRating();
		}
		if (!useCustomizedPromptView)
		{
			UniRatePlugin.ShowPrompt(MessageTitle, Message, RateButtonLabel, CancelButtonLabel, RemindButtonLabel);
			_promptShowing = true;
		}
	}

	public void OpenRatePage()
	{
		ratedThisVersion = true;
		UniRateDebug.Log("Open rating page of URL: " + RatingAndroidURL);
		UniRatePlugin.OpenAndroidRatePage(RatingAndroidURL);
	}

	public void LogEvent(bool withPrompt)
	{
		IncreaseEventCount();
		if (withPrompt && ShouldPromptForRating())
		{
			PromptIfNetworkAvailable();
		}
	}

	public void Reset()
	{
		PlayerPrefs.DeleteKey("UniRateRatedVersionChecked");
		PlayerPrefs.DeleteKey("UniRateDeclinedVersion");
		PlayerPrefs.DeleteKey("UniRateLastReminded");
		PlayerPrefs.DeleteKey("UniRateLastVersionUsed");
		PlayerPrefs.DeleteKey("UniRateFirstUsed");
		PlayerPrefs.DeleteKey("UniRateUseCount");
		PlayerPrefs.DeleteKey("UniRateEventCount");
		PlayerPrefs.Save();
		Instance.Init();
	}

	private void Start()
	{
		Instance.Init();
	}

	private void Init()
	{
		if (string.IsNullOrEmpty(appStoreCountry))
		{
			appStoreCountry = UniRatePlugin.GetAppStoreCountry();
			UniRateDebug.Log("Get Country Code: " + appStoreCountry);
		}
		if (string.IsNullOrEmpty(applicationVersion))
		{
			applicationVersion = UniRatePlugin.GetApplicationVersion();
			UniRateDebug.Log("Get App Version: " + applicationVersion);
		}
		if (string.IsNullOrEmpty(applicationName))
		{
			applicationName = UniRatePlugin.GetApplicationName();
			UniRateDebug.Log("Get App Name: " + applicationName);
		}
		if (string.IsNullOrEmpty(applicationBundleID))
		{
			applicationBundleID = UniRatePlugin.GetApplicationBundleID();
			UniRateDebug.Log("Get Bundle ID: " + applicationBundleID);
		}
		if (string.IsNullOrEmpty(marketPackageName))
		{
			marketPackageName = UniRatePlugin.GetPackageName();
			UniRateDebug.Log("Get Android package name: " + marketPackageName);
		}
		_promptShowing = false;
		UniRateLauched();
	}

	private void UniRateLauched()
	{
		if (!IsSameVersion())
		{
			PlayerPrefs.SetString("UniRateLastVersionUsed", applicationVersion);
			UniRatePlayerPrefs.SetDate("UniRateFirstUsed", DateTime.Now);
			PlayerPrefs.SetInt("UniRateUseCount", 0);
			PlayerPrefs.SetInt("UniRateEventCount", 0);
			PlayerPrefs.DeleteKey("UniRateLastReminded");
			PlayerPrefs.Save();
			if (this.OnDetectAppUpdated != null)
			{
				this.OnDetectAppUpdated();
			}
		}
		IncreaseUseCount();
		if (promptAtLaunch && ShouldPromptForRating())
		{
			PromptIfNetworkAvailable();
		}
	}

	private bool IsSameVersion()
	{
		if (!string.IsNullOrEmpty(applicationVersion))
		{
			return string.Equals(PlayerPrefs.GetString("UniRateLastVersionUsed"), applicationVersion);
		}
		return false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && _instance != null)
		{
			IncreaseUseCount();
			if (promptAtLaunch && !_promptShowing && ShouldPromptForRating())
			{
				PromptIfNetworkAvailable();
			}
		}
	}

	private void IncreaseUseCount()
	{
		usesCount++;
	}

	private void IncreaseEventCount()
	{
		eventCount++;
	}

	private void CheckAndReadyToRate(bool showPrompt)
	{
		if (!_currentChecking)
		{
			if (showPrompt)
			{
				ReadyToPrompt();
			}
			else
			{
				OpenRatePage();
			}
		}
	}

	private IEnumerator CheckForConnectivityInBackground(bool showPrompt)
	{
		string iTunesServiceURL3 = string.Empty;
		iTunesServiceURL3 = (string.IsNullOrEmpty(appStoreCountry) ? string.Format("http://itunes.apple.com/{0}lookup", string.Empty) : string.Format("http://itunes.apple.com/{0}lookup", appStoreCountry + "/"));
		iTunesServiceURL3 = ((appStoreID == 0) ? (iTunesServiceURL3 + "?bundleId=" + applicationBundleID) : (iTunesServiceURL3 + "?id=" + appStoreID));
		UniRateDebug.Log("Checking app info: " + iTunesServiceURL3);
		bool errorHappened = false;
		WWW www = new WWW(iTunesServiceURL3);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			UniRateAppInfo info = new UniRateAppInfo(www.text);
			if (info.validAppInfo)
			{
				if (info.bundleId.Equals(applicationBundleID))
				{
					if (appStoreID == 0)
					{
						appStoreID = info.appID;
						UniRateDebug.Log("UniRate found the app, app id: " + appStoreID);
					}
					if (onlyPromptIfLatestVersion && !previewMode && !applicationVersion.Equals(info.version))
					{
						UniRateDebug.Log("No prompt because it is not the version in app store and you set onlyPromptIfLatestVersion.");
						errorHappened = true;
						UniRateFailWithError(Error.NotTheLatestVersion);
					}
				}
				else
				{
					Debug.LogWarning("The bundle Id is not the same. Appstore: " + info.bundleId + " vs AppSetting:" + applicationBundleID);
					errorHappened = true;
					UniRateFailWithError(Error.BundleIdDoesNotMatchAppStore);
				}
			}
			else if (appStoreID != 0)
			{
				Debug.LogWarning("No App info found with this app Id " + appStoreID);
				errorHappened = true;
				UniRateFailWithError(Error.AppNotFoundOnAppStore);
			}
			else
			{
				Debug.Log("No App info found with this bundle Id " + applicationBundleID);
				Debug.Log("Could not find your app on AppStore. It is normal when your app is not released, don't worry about this message.");
			}
		}
		else
		{
			UniRateDebug.Log("Error happend in loading app information. Maybe due to no internet connection.");
			errorHappened = true;
			UniRateFailWithError(Error.NetworkError);
		}
		if (!errorHappened)
		{
			_currentChecking = false;
			if (showPrompt)
			{
				ReadyToPrompt();
			}
			else
			{
				OpenRatePage();
			}
		}
	}

	private void ReadyToPrompt()
	{
		if (this.ShouldUniRatePromptForRating != null && !this.ShouldUniRatePromptForRating())
		{
			UniRateDebug.Log("Not display prompt because ShouldUniRatePromptForRating returns false.");
		}
		else
		{
			ShowPrompt();
		}
	}

	private void UniRateFailWithError(Error error)
	{
		if (this.OnUniRateFaild != null)
		{
			this.OnUniRateFaild(error);
		}
	}

	private void UniRateUserDeclinedPrompt()
	{
		UniRateDebug.Log("User declined the prompt");
		_promptShowing = false;
		declinedThisVersion = true;
		if (this.OnUserDeclinedToRate != null)
		{
			this.OnUserDeclinedToRate();
		}
	}

	private void UniRateUserWantRemind()
	{
		UniRateDebug.Log("User wants to be reminded later");
		_promptShowing = false;
		lastReminded = DateTime.Now;
		if (this.OnUserWantReminderToRate != null)
		{
			this.OnUserWantReminderToRate();
		}
	}

	private void UniRateUserWantToRate()
	{
		UniRateDebug.Log("User wants to rate");
		_promptShowing = false;
		if (this.OnUserAttemptToRate != null)
		{
			this.OnUserAttemptToRate();
		}
		if (this.ShouldUniRateOpenRatePage != null && !this.ShouldUniRateOpenRatePage())
		{
			UniRateDebug.Log("Not open rate page because ShouldUniRateOpenRatePage() returning false");
		}
		else
		{
			OpenRatePage();
		}
	}

	private string GetLocalizedMessageTitle()
	{
		string text = "Rate {0}";
		if (autoLocalization)
		{
			string lang = Application.systemLanguage.ToString();
			text = GetContentForLanguageAndKey(lang, "title") ?? text;
		}
		return string.Format(text, applicationName);
	}

	private string GetLocalizedMessage()
	{
		string text = "If you enjoy {0}, would you mind taking a moment to rate it? It will not take more than a minute. Thanks for your support!";
		if (autoLocalization)
		{
			string lang = Application.systemLanguage.ToString();
			text = GetContentForLanguageAndKey(lang, "body") ?? text;
		}
		return string.Format(text, applicationName);
	}

	private string GetLocalizedCancelButtonLabel()
	{
		string text = "No, thanks";
		if (autoLocalization)
		{
			string lang = Application.systemLanguage.ToString();
			text = GetContentForLanguageAndKey(lang, "cancel") ?? text;
		}
		return text;
	}

	private string GetLocalizedRemindButtonLabel()
	{
		string text = "Remind me later";
		if (autoLocalization)
		{
			string lang = Application.systemLanguage.ToString();
			text = GetContentForLanguageAndKey(lang, "remind") ?? text;
		}
		return text;
	}

	private string GetLocalizedRateButtonLabel()
	{
		string text = "Rate it now";
		if (autoLocalization)
		{
			string lang = Application.systemLanguage.ToString();
			text = GetContentForLanguageAndKey(lang, "rate") ?? text;
		}
		return text;
	}

	private string GetContentForLanguageAndKey(string lang, string key)
	{
		if (localizationDic != null && localizationDic.ContainsKey(lang))
		{
			Dictionary<string, object> dictionary = localizationDic[lang] as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey(key))
			{
				return dictionary[key] as string;
			}
		}
		return null;
	}
}
