using UnityEngine;

namespace Game.Util
{
	public class Logger
	{
		public static void Log<T>(T value)
		{
			if (value != null)
			{
			}
		}

		public static void Log(string text)
		{
		}

		public static void Warning(string text)
		{
			Debug.LogWarning(text);
		}

		public static void Error(string text)
		{
			Debug.LogError(text);
		}
	}
}
