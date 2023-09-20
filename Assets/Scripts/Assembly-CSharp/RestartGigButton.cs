using Game;
using UnityEngine;

public class RestartGigButton : MonoBehaviour
{
	public Fader Fader;

	private void OnClick()
	{
		Fader.FadeOutDone += ReloadScene;
		Fader.FadeOut();
	}

	private void ReloadScene()
	{
		Fader.FadeOutDone -= ReloadScene;
		GameController.Instance.BundleModel.SetNowPlaying(GameController.Instance.CurrentLevel);
		AsyncOperation asyncOperation = GameController.Instance.EnterPlayMode();
		if (asyncOperation != null)
		{
			asyncOperation.allowSceneActivation = true;
		}
	}
}
