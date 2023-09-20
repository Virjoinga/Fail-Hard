using System.Collections.Generic;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements
{
	internal static class DeviceInfo
	{
		private static DeviceInfoPlatform impl;

		private static bool init;

		private static string json;

		private static DeviceInfoPlatform getImpl()
		{
			if (!init)
			{
				impl = new DeviceInfoAndroid();
				init = true;
			}
			return impl;
		}

		public static string currentPlatform()
		{
			return getImpl().name();
		}

		public static string advertisingIdentifier()
		{
			return getImpl().getAdvertisingIdentifier();
		}

		public static bool noTrack()
		{
			return getImpl().getNoTrack();
		}

		public static string deviceVendor()
		{
			return getImpl().getVendor();
		}

		public static string deviceModel()
		{
			return getImpl().getModel();
		}

		public static string osVersion()
		{
			return getImpl().getOSVersion();
		}

		public static string screenSize()
		{
			return getImpl().getScreenSize();
		}

		public static string screenDpi()
		{
			return getImpl().getScreenDpi();
		}

		public static string deviceID()
		{
			return getImpl().getDeviceId();
		}

		public static string bundleID()
		{
			return getImpl().getBundleId();
		}

		private static Dictionary<string, object> mainInfoDict()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			addDeviceInfo(dictionary, "platform", currentPlatform());
			addDeviceInfo(dictionary, "unity", Application.unityVersion);
			addDeviceInfo(dictionary, "aid", advertisingIdentifier());
			addDeviceInfo(dictionary, "notrack", noTrack());
			addDeviceInfo(dictionary, "make", deviceVendor());
			addDeviceInfo(dictionary, "model", deviceModel());
			addDeviceInfo(dictionary, "os", osVersion());
			addDeviceInfo(dictionary, "screen", screenSize());
			addDeviceInfo(dictionary, "dpi", screenDpi());
			addDeviceInfo(dictionary, "did", deviceID());
			addDeviceInfo(dictionary, "bundle", bundleID());
			addDeviceInfo(dictionary, "test", Engine.Instance.testMode);
			addDeviceInfo(dictionary, "sdk", Settings.sdkVersion);
			return dictionary;
		}

		public static string getJson()
		{
			if (json == null)
			{
				json = Json.Serialize(mainInfoDict());
			}
			return json;
		}

		public static string adRequestJSONPayload(string network)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			addDeviceInfo(dictionary, "network", network);
			dictionary["info"] = mainInfoDict();
			return Json.Serialize(dictionary);
		}

		private static void addDeviceInfo(Dictionary<string, object> dict, string key, object value)
		{
			if (value != null)
			{
				if (!(value is string))
				{
					dict.Add(key, value);
				}
				else if (value is string && ((string)value).Length > 0)
				{
					dict.Add(key, value);
				}
			}
		}
	}
}
