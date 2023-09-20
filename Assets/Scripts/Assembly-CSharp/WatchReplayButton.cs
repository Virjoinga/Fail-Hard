using Game;
using UnityEngine;

public class WatchReplayButton : MonoBehaviour
{
	public UITexture ScreenShotTexture;

	private void OnEnable()
	{
		GigStatus component = GameObject.Find("GigController").GetComponent<GigStatus>();
		if (component != null && !Configuration.IsAndroid && ReplayFilter.ReplayEnabled())
		{
			component.SetupScreenShotTexture(ref ScreenShotTexture);
		}
	}

	private void Start()
	{
		if (Configuration.IsAndroid || !ReplayFilter.ReplayEnabled())
		{
			base.gameObject.SetActive(false);
		}
	}
}
