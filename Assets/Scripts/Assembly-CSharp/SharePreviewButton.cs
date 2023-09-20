using Holoville.HOTween;
using UnityEngine;

public class SharePreviewButton : MonoBehaviour
{
	public ShareScreenshotPreview Preview;

	public UISprite FadeAway;

	public GameObject Deactivate;

	private bool m_clicked;

	private void OnEnable()
	{
		m_clicked = false;
		Deactivate.SetActive(true);
	}

	private void OnClick()
	{
		if (!m_clicked)
		{
			Preview.CreateAndShare();
			m_clicked = true;
			TweenParms tweenParms = new TweenParms().Prop("alpha", 0f);
			tweenParms.OnComplete(Hide);
			HOTween.To(FadeAway, 0.15f, tweenParms);
		}
	}

	private void Hide()
	{
		FadeAway.alpha = 1f;
		Deactivate.SetActive(false);
	}
}
