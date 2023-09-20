using System;

namespace Game
{
	public class Storage
	{
		public static bool ForceOffline { get; set; }

		public static IStorage Instance
		{
			get
			{
				if (ForceOffline)
				{
					CloudStorageST.ServerAvailable = false;
					return LocalStorage.Instance;
				}
				if (CloudStorageST.ServerAvailable)
				{
					return CloudStorageST.Instance;
				}
				return LocalStorage.Instance;
			}
			private set
			{
			}
		}

		public static int UnixTimeNow
		{
			get
			{
				return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
			}
			private set
			{
			}
		}
	}
}
