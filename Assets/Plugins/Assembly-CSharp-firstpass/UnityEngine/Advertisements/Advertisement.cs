namespace UnityEngine.Advertisements
{
	public static class Advertisement
	{
		public enum DebugLevel
		{
			NONE = 0,
			ERROR = 1,
			WARNING = 2,
			INFO = 4,
			DEBUG = 8
		}

		private static DebugLevel _debugLevel = ((!Debug.isDebugBuild) ? ((DebugLevel)7) : ((DebugLevel)15));

		public static DebugLevel debugLevel
		{
			get
			{
				return _debugLevel;
			}
			set
			{
				_debugLevel = value;
				UnityAds.setLogLevel(_debugLevel);
			}
		}

		public static bool isSupported
		{
			get
			{
				return Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
			}
		}

		public static bool isInitialized
		{
			get
			{
				return Engine.Instance.isInitialized;
			}
		}

		public static bool allowPrecache
		{
			get
			{
				return Engine.Instance.allowPrecache;
			}
			set
			{
				Engine.Instance.allowPrecache = value;
			}
		}

		public static bool isShowing
		{
			get
			{
				return Engine.Instance.isShowing();
			}
		}

		public static bool UnityDeveloperInternalTestMode { get; set; }

		public static void Initialize(string appId, bool testMode = false)
		{
			Engine.Instance.Initialize(appId, testMode);
		}

		public static void Show(string zoneId = null, ShowOptions options = null)
		{
			Engine.Instance.Show(zoneId, options);
		}

		public static bool isReady(string zoneId = null)
		{
			return Engine.Instance.isReady(zoneId);
		}
	}
}
