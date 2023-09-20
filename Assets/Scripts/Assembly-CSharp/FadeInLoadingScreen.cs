using Game;
using Holoville.HOTween;
using UnityEngine;

public class FadeInLoadingScreen : MonoBehaviour
{
	public UIPanel Panel;

	public bool AsyncLoadWithEmptyScene = true;

	private void OnEnable()
	{
		Panel.alpha = 0f;
		TweenParms p_parms = new TweenParms().Prop("alpha", 1f).OnComplete(StartLevel);
		HOTween.To(Panel, 0.2f, p_parms);
	}

	private void StartLevel()
	{
		if (AsyncLoadWithEmptyScene)
		{
			Application.LoadLevel(8);
		}
		else
		{
			GameController.Instance.EnterPlayMode();
		}
	}
}
