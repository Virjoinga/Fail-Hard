using Game;
using UnityEngine;

public class UIQuitGig : MonoBehaviour
{
	public GameController.GameUIView view;

	public Fader Fader;

	public Ads Ads;

	private void OnClick()
	{
		Ads.Show();
		if ((bool)Fader)
		{
			Fader.FadeOutDone += QuitStunt;
			Fader.FadeOut();
		}
		else
		{
			QuitStunt();
		}
	}

	private void QuitStunt()
	{
		if ((bool)Fader)
		{
			Fader.FadeOutDone -= QuitStunt;
		}
		GameController.Instance.CurrentView = view;
		Application.LoadLevel(1);
	}
}
