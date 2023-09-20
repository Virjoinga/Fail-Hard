namespace UnityEngine.Advertisements
{
	internal class DeviceInfoAndroid : DeviceInfoPlatform
	{
		private AndroidJavaObject androidImpl;

		public DeviceInfoAndroid()
		{
			androidImpl = new AndroidJavaObject("com.unity3d.ads.picture.DeviceInfo");
		}

		private T androidCall<T>(string method)
		{
			return androidImpl.Call<T>(method, new object[0]);
		}

		public override string name()
		{
			return "android";
		}

		public override string getAdvertisingIdentifier()
		{
			string text = androidCall<string>("getAdvertisingTrackingId");
			return (text == null) ? string.Empty : text;
		}

		public override bool getNoTrack()
		{
			return androidCall<bool>("getLimitAdTracking");
		}

		public override string getVendor()
		{
			return androidCall<string>("getManufacturer");
		}

		public override string getModel()
		{
			return androidCall<string>("getModel");
		}

		public override string getOSVersion()
		{
			return androidCall<string>("getOSVersion");
		}

		public override string getScreenSize()
		{
			double num = androidCall<double>("getScreenSize");
			return (!(num > 0.0)) ? string.Empty : string.Format("{0:0.00}", num);
		}

		public override string getScreenDpi()
		{
			int num = androidCall<int>("getScreenDpi");
			return (num <= 0) ? string.Empty : num.ToString();
		}

		public override string getDeviceId()
		{
			string text = androidCall<string>("getAndroidId");
			return (text == null) ? string.Empty : text;
		}

		public override string getBundleId()
		{
			string text = androidCall<string>("getPackageName");
			return (text == null) ? string.Empty : text;
		}
	}
}
