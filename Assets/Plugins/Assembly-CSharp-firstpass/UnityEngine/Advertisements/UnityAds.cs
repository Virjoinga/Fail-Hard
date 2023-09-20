using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	internal class UnityAds : MonoBehaviour
	{
		public delegate void UnityAdsCampaignsAvailable();

		public delegate void UnityAdsCampaignsFetchFailed();

		public delegate void UnityAdsShow();

		public delegate void UnityAdsHide();

		public delegate void UnityAdsVideoCompleted(string rewardItemKey, bool skipped);

		public delegate void UnityAdsVideoStarted();

		private static UnityAds sharedInstance;

		private static bool _adsShow = false;

		private static HashSet<string> _campaignsAvailable = new HashSet<string>();

		private static string _rewardItemNameKey = string.Empty;

		private static string _rewardItemPictureKey = string.Empty;

		public static UnityAdsCampaignsAvailable OnCampaignsAvailable;

		public static UnityAdsCampaignsFetchFailed OnCampaignsFetchFailed;

		public static UnityAdsShow OnShow;

		public static UnityAdsHide OnHide;

		public static UnityAdsVideoCompleted OnVideoCompleted;

		public static UnityAdsVideoStarted OnVideoStarted;

		public static UnityAds SharedInstance
		{
			get
			{
				if (!sharedInstance)
				{
					sharedInstance = (UnityAds)Object.FindObjectOfType(typeof(UnityAds));
				}
				if (!sharedInstance)
				{
					GameObject gameObject = new GameObject();
					gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
					GameObject gameObject2 = gameObject;
					sharedInstance = gameObject2.AddComponent<UnityAds>();
					gameObject2.name = "UnityAdsPluginBridgeObject";
					Object.DontDestroyOnLoad(gameObject2);
				}
				return sharedInstance;
			}
		}

		public void Init(string gameId, bool testModeEnabled)
		{
			UnityAdsExternal.init(gameId, testModeEnabled, SharedInstance.gameObject.name);
		}

		public void Awake()
		{
			if (base.gameObject == SharedInstance.gameObject)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		public static bool isSupported()
		{
			return UnityAdsExternal.isSupported();
		}

		public static string getSDKVersion()
		{
			return UnityAdsExternal.getSDKVersion();
		}

		public static bool canShowAds(string network)
		{
			return _campaignsAvailable.Contains(network) && UnityAdsExternal.canShowAds(network);
		}

		public static void setLogLevel(Advertisement.DebugLevel logLevel)
		{
			UnityAdsExternal.setLogLevel(logLevel);
		}

		public static bool canShow()
		{
			return _campaignsAvailable.Count > 0;
		}

		public static bool hasMultipleRewardItems()
		{
			if (_campaignsAvailable.Count > 0)
			{
				return UnityAdsExternal.hasMultipleRewardItems();
			}
			return false;
		}

		public static List<string> getRewardItemKeys()
		{
			List<string> result = new List<string>();
			if (_campaignsAvailable.Count > 0)
			{
				string rewardItemKeys = UnityAdsExternal.getRewardItemKeys();
				result = new List<string>(rewardItemKeys.Split(';'));
			}
			return result;
		}

		public static string getDefaultRewardItemKey()
		{
			if (_campaignsAvailable.Count > 0)
			{
				return UnityAdsExternal.getDefaultRewardItemKey();
			}
			return string.Empty;
		}

		public static string getCurrentRewardItemKey()
		{
			if (_campaignsAvailable.Count > 0)
			{
				return UnityAdsExternal.getCurrentRewardItemKey();
			}
			return string.Empty;
		}

		public static bool setRewardItemKey(string rewardItemKey)
		{
			if (_campaignsAvailable.Count > 0)
			{
				return UnityAdsExternal.setRewardItemKey(rewardItemKey);
			}
			return false;
		}

		public static void setDefaultRewardItemAsRewardItem()
		{
			if (_campaignsAvailable.Count > 0)
			{
				UnityAdsExternal.setDefaultRewardItemAsRewardItem();
			}
		}

		public static string getRewardItemNameKey()
		{
			if (_rewardItemNameKey == null || _rewardItemNameKey.Length == 0)
			{
				fillRewardItemKeyData();
			}
			return _rewardItemNameKey;
		}

		public static string getRewardItemPictureKey()
		{
			if (_rewardItemPictureKey == null || _rewardItemPictureKey.Length == 0)
			{
				fillRewardItemKeyData();
			}
			return _rewardItemPictureKey;
		}

		public static Dictionary<string, string> getRewardItemDetailsWithKey(string rewardItemKey)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string empty = string.Empty;
			if (_campaignsAvailable.Count > 0)
			{
				empty = UnityAdsExternal.getRewardItemDetailsWithKey(rewardItemKey);
				if (empty != null)
				{
					List<string> list = new List<string>(empty.Split(';'));
					Utils.LogDebug("UnityAndroid: getRewardItemDetailsWithKey() rewardItemDataString=" + empty);
					if (list.Count == 2)
					{
						dictionary.Add(getRewardItemNameKey(), list.ToArray().GetValue(0).ToString());
						dictionary.Add(getRewardItemPictureKey(), list.ToArray().GetValue(1).ToString());
					}
				}
			}
			return dictionary;
		}

		public static void setNetworks(HashSet<string> networks)
		{
			UnityAdsExternal.setNetworks(networks);
		}

		public static void setNetwork(string network)
		{
			UnityAdsExternal.setNetwork(network);
		}

		public static bool show(string zoneId = null)
		{
			return show(zoneId, string.Empty, null);
		}

		public static bool show(string zoneId, string rewardItemKey)
		{
			return show(zoneId, rewardItemKey, null);
		}

		public static bool show(string zoneId, string rewardItemKey, Dictionary<string, string> options)
		{
			if (!_adsShow && _campaignsAvailable.Count > 0 && (bool)SharedInstance)
			{
				string options2 = parseOptionsDictionary(options);
				if (UnityAdsExternal.show(zoneId, rewardItemKey, options2))
				{
					if (OnShow != null)
					{
						OnShow();
					}
					_adsShow = true;
					return true;
				}
			}
			return false;
		}

		public static void hide()
		{
			if (_adsShow)
			{
				UnityAdsExternal.hide();
			}
		}

		private static void fillRewardItemKeyData()
		{
			string rewardItemDetailsKeys = UnityAdsExternal.getRewardItemDetailsKeys();
			if (rewardItemDetailsKeys != null && rewardItemDetailsKeys.Length > 2)
			{
				List<string> list = new List<string>(rewardItemDetailsKeys.Split(';'));
				_rewardItemNameKey = list.ToArray().GetValue(0).ToString();
				_rewardItemPictureKey = list.ToArray().GetValue(1).ToString();
			}
		}

		private static string parseOptionsDictionary(Dictionary<string, string> options)
		{
			string text = string.Empty;
			if (options != null)
			{
				bool flag = false;
				if (options.ContainsKey("noOfferScreen"))
				{
					text = text + ((!flag) ? string.Empty : ",") + "noOfferScreen:" + options["noOfferScreen"];
					flag = true;
				}
				if (options.ContainsKey("openAnimated"))
				{
					text = text + ((!flag) ? string.Empty : ",") + "openAnimated:" + options["openAnimated"];
					flag = true;
				}
				if (options.ContainsKey("sid"))
				{
					text = text + ((!flag) ? string.Empty : ",") + "sid:" + options["sid"];
					flag = true;
				}
				if (options.ContainsKey("muteVideoSounds"))
				{
					text = text + ((!flag) ? string.Empty : ",") + "muteVideoSounds:" + options["muteVideoSounds"];
					flag = true;
				}
				if (options.ContainsKey("useDeviceOrientationForVideo"))
				{
					text = text + ((!flag) ? string.Empty : ",") + "useDeviceOrientationForVideo:" + options["useDeviceOrientationForVideo"];
					flag = true;
				}
			}
			return text;
		}

		public void onHide()
		{
			_adsShow = false;
			if (OnHide != null)
			{
				OnHide();
			}
			Utils.LogDebug("onHide");
		}

		public void onShow()
		{
			Utils.LogDebug("onShow");
		}

		public void onVideoStarted()
		{
			if (OnVideoStarted != null)
			{
				OnVideoStarted();
			}
			Utils.LogDebug("onVideoStarted");
		}

		public void onVideoCompleted(string parameters)
		{
			if (parameters != null)
			{
				List<string> list = new List<string>(parameters.Split(';'));
				string text = list.ToArray().GetValue(0).ToString();
				bool flag = ((list.ToArray().GetValue(1).ToString() == "true") ? true : false);
				if (OnVideoCompleted != null)
				{
					OnVideoCompleted(text, flag);
				}
				Utils.LogDebug("onVideoCompleted: " + text + " - " + flag);
			}
		}

		public void onFetchCompleted(string network)
		{
			_campaignsAvailable.Add(network);
			if (OnCampaignsAvailable != null)
			{
				OnCampaignsAvailable();
			}
			Utils.LogDebug("onFetchCompleted - " + network);
		}

		public void onFetchFailed()
		{
			if (OnCampaignsFetchFailed != null)
			{
				OnCampaignsFetchFailed();
			}
			Utils.LogDebug("onFetchFailed");
		}
	}
}
