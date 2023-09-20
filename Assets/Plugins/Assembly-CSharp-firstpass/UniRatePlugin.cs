using UnityEngine;

public class UniRatePlugin
{
	private static AndroidJavaObject _uniRateInterface;

	public static void ShowPrompt(string title, string msg, string rateTitle, string canceltitle, string remindTitle)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_uniRateInterface.Call("ShowPrompt", title, msg, rateTitle, canceltitle, remindTitle);
		}
	}

	public static string GetAppStoreCountry()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return string.Empty;
		}
		return string.Empty;
	}

	public static string GetApplicationName()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return _uniRateInterface.Call<string>("GetAppName", new object[0]);
		}
		return string.Empty;
	}

	public static string GetApplicationVersion()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return _uniRateInterface.Call<string>("GetAppVersion", new object[0]);
		}
		return "0";
	}

	public static string GetApplicationBundleID()
	{
		return string.Empty;
	}

	public static string GetPackageName()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return _uniRateInterface.Call<string>("GetPackageName", new object[0]);
		}
		return string.Empty;
	}

	public static void OpenAndroidRatePage(string rateURL)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_uniRateInterface.Call("OpenRatePage", rateURL);
		}
		else
		{
			Debug.Log("The review page can only be opened on a real Android device.");
		}
	}

	public static void OpenWPMarket()
	{
	}

	public static void InitUniRateAndroid()
	{
		if (Application.platform == RuntimePlatform.Android && _uniRateInterface == null)
		{
			_uniRateInterface = new AndroidJavaObject("com.onevcat.unirate.UniRatePlugin");
		}
	}
}
