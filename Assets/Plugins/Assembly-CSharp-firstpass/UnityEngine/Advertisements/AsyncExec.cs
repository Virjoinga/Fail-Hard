using System;
using System.Collections;

namespace UnityEngine.Advertisements
{
	internal class AsyncExec
	{
		private static GameObject asyncExecGameObject;

		private static MonoBehaviour coroutineHost;

		private static AsyncExec asyncImpl;

		private static bool init;

		private static MonoBehaviour getImpl()
		{
			if (!init)
			{
				asyncImpl = new AsyncExec();
				GameObject gameObject = new GameObject("Unity Ads Coroutine Host");
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				asyncExecGameObject = gameObject;
				coroutineHost = asyncExecGameObject.AddComponent<MonoBehaviour>();
				Object.DontDestroyOnLoad(asyncExecGameObject);
				init = true;
			}
			return coroutineHost;
		}

		private static AsyncExec getAsyncImpl()
		{
			if (!init)
			{
				getImpl();
			}
			return asyncImpl;
		}

		public static void run(IEnumerator method)
		{
			getImpl().StartCoroutine(method);
		}

		public static void runWithCallback<T>(Func<Action<T>, IEnumerator> asyncMethod, Action<T> callback)
		{
			getImpl().StartCoroutine(asyncMethod(callback));
		}

		public static void runWithCallback<K, T>(Func<K, Action<T>, IEnumerator> asyncMethod, K arg0, Action<T> callback)
		{
			getImpl().StartCoroutine(asyncMethod(arg0, callback));
		}

		public static void runWithDelay(int delay, Action callback)
		{
			getImpl().StartCoroutine(getAsyncImpl().delayedCallback(delay, callback));
		}

		private IEnumerator delayedCallback(int delay, Action callback)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + (float)delay)
			{
				yield return null;
			}
			callback();
		}
	}
}
