namespace UnityEngine.Advertisements
{
	internal abstract class DeviceInfoPlatform
	{
		public abstract string name();

		public virtual string getAdvertisingIdentifier()
		{
			return string.Empty;
		}

		public virtual bool getNoTrack()
		{
			return false;
		}

		public virtual string getVendor()
		{
			return string.Empty;
		}

		public virtual string getModel()
		{
			return string.Empty;
		}

		public virtual string getOSVersion()
		{
			return string.Empty;
		}

		public virtual string getScreenSize()
		{
			return string.Empty;
		}

		public virtual string getScreenDpi()
		{
			return string.Empty;
		}

		public virtual string getDeviceId()
		{
			return string.Empty;
		}

		public virtual string getBundleId()
		{
			return string.Empty;
		}
	}
}
