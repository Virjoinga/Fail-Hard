using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class TweenAlphaEnableDisable : MonoBehaviour
{
	public float FadeTime;

	public UISprite m_buttonSprite;

	private bool m_started;

	private void OnEnable()
	{
		if (m_buttonSprite != null)
		{
			m_buttonSprite.alpha = 0f;
			TweenParms p_parms = new TweenParms().Prop("alpha", 1f);
			HOTween.To(m_buttonSprite, FadeTime, p_parms);
		}
	}

	public void Disable()
	{
		if (m_buttonSprite != null)
		{
			TweenParms tweenParms = new TweenParms().Prop("alpha", 0f);
			tweenParms.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				base.gameObject.SetActive(false);
			});
			HOTween.To(m_buttonSprite, FadeTime, tweenParms);
		}
	}
}
