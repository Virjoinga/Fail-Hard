using System;
using UnityEngine;

public class AdMobListener : MonoBehaviour
{
	private static AdMobListener s_instance;

	private Action<string> m_failureAction;

	public static void CreateListener(Action<string> failureAction)
	{
		GameObject gameObject = new GameObject("AdMobListener");
		AdMobListener adMobListener = gameObject.AddComponent<AdMobListener>();
		adMobListener.m_failureAction = failureAction;
	}

	private void Start()
	{
		if (!(s_instance != null))
		{
			s_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void adFailed(string errorMessage)
	{
		if (m_failureAction != null)
		{
			m_failureAction(errorMessage);
		}
	}
}
