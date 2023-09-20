using Game;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
	public Cameraman cameraman;

	private void Start()
	{
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		if (cameraman == null)
		{
			currentLevel.SetCamera(base.gameObject.GetComponent<Cameraman>());
		}
		else
		{
			currentLevel.SetCamera(cameraman);
		}
	}

	public void EnableScreenshot()
	{
		ScreenShotRenderTexture screenShotRenderTexture = base.gameObject.AddComponent<ScreenShotRenderTexture>();
		screenShotRenderTexture.Init();
	}
}
