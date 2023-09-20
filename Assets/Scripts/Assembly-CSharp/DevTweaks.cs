using Game;
using UnityEngine;

public class DevTweaks : MonoBehaviour
{
	public bool disableAudio;

	public bool forceOffline;

	public bool forceSaveGameData;

	private void Awake()
	{
		AudioListener.pause = disableAudio;
		Storage.ForceOffline = forceOffline;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (forceSaveGameData)
		{
			LocalStorage.Instance.Shutdown();
			forceSaveGameData = false;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Storage.Instance.Shutdown();
		}
		else
		{
			Storage.Instance.Restore();
		}
	}
}
