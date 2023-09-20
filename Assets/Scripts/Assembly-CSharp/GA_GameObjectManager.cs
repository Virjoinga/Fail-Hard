using System.Collections;
using UnityEngine;

public class GA_GameObjectManager : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void RunCoroutine(IEnumerator routine)
	{
		StartCoroutine(routine);
	}

	public void OnApplicationQuit()
	{
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && GA.SettingsGA.NewSessionOnResume)
		{
			GA.Log("GA: Generating new session id");
			GA.API.GenericInfo.SetSessionUUID();
		}
	}
}
