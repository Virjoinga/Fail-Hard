using UnityEngine;

public static class Configuration
{
	private static RuntimePlatform s_currentPlatform;

	public static RuntimePlatform CurrentPlatform
	{
		get
		{
			return s_currentPlatform;
		}
	}

	public static bool IsAndroid
	{
		get
		{
			return s_currentPlatform == RuntimePlatform.Android;
		}
	}

	public static bool IsIPhone
	{
		get
		{
			return s_currentPlatform == RuntimePlatform.IPhonePlayer;
		}
	}

	static Configuration()
	{
		s_currentPlatform = Application.platform;
	}
}
