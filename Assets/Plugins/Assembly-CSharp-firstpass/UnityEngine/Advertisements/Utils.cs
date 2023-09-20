using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	internal static class Utils
	{
		public static string addUrlParameters(string url, Dictionary<string, object> parameters)
		{
			url = ((url.IndexOf('?') == -1) ? (url + "?") : (url + "&"));
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> parameter in parameters)
			{
				if (parameter.Value != null)
				{
					list.Add(parameter.Key + "=" + parameter.Value.ToString());
				}
			}
			return url + string.Join("&", list.ToArray());
		}

		public static string Join(IEnumerable enumerable, string separator)
		{
			string text = string.Empty;
			foreach (object item in enumerable)
			{
				text = text + item.ToString() + separator;
			}
			return (text.Length <= 0) ? text : text.Substring(0, text.Length - separator.Length);
		}

		public static T Optional<T>(Dictionary<string, object> jsonObject, string key, object defaultValue = null)
		{
			try
			{
				return (T)jsonObject[key];
			}
			catch
			{
				return (T)defaultValue;
			}
		}

		private static void Log(Advertisement.DebugLevel debugLevel, string message)
		{
			if ((Advertisement.debugLevel & debugLevel) != 0)
			{
				Debug.Log(message);
			}
		}

		public static void LogDebug(string message)
		{
			Log(Advertisement.DebugLevel.DEBUG, "Debug: " + message);
		}

		public static void LogInfo(string message)
		{
			Log(Advertisement.DebugLevel.INFO, "Info:" + message);
		}

		public static void LogWarning(string message)
		{
			Log(Advertisement.DebugLevel.WARNING, "Warning:" + message);
		}

		public static void LogError(string message)
		{
			Log(Advertisement.DebugLevel.ERROR, "Error: " + message);
		}
	}
}
