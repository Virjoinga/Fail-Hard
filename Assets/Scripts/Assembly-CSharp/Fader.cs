using System.Runtime.CompilerServices;
using Holoville.HOTween;
using UnityEngine;

public class Fader : MonoBehaviour
{
	public delegate void Fade();

	public float FadeInTime;

	public float FadeOutTime;

	public UISprite Background;

	public UISprite Wrench;

	public Transform Rotate;

	private Tweener m_rotateTween;

	[method: MethodImpl(32)]
	public event Fade FadeOutDone = delegate
	{
	};

	[method: MethodImpl(32)]
	public event Fade FadeInDone = delegate
	{
	};

	public void Start()
	{
		FadeIn();
	}

	public void FadeIn()
	{
		base.gameObject.SetActive(true);
		Background.alpha = 1f;
		Wrench.alpha = 1f;
		TweenParms p_parms = new TweenParms().Prop("alpha", 0f).OnComplete(DeactivateFadeIn);
		HOTween.To(Background, FadeInTime, p_parms);
		HOTween.To(Wrench, FadeInTime, p_parms);
		RotateTween();
	}

	public void FadeOut()
	{
		base.gameObject.SetActive(true);
		Background.alpha = 0f;
		Wrench.alpha = 0f;
		TweenParms p_parms = new TweenParms().Prop("alpha", 1f).OnComplete(DeactivateFadeOut);
		HOTween.To(Background, FadeOutTime, p_parms);
		HOTween.To(Wrench, FadeOutTime, p_parms);
		RotateTween();
	}

	public void DeactivateFadeIn()
	{
		base.gameObject.SetActive(false);
		if (m_rotateTween != null)
		{
			m_rotateTween.Kill();
			m_rotateTween = null;
		}
		this.FadeInDone();
	}

	public void DeactivateFadeOut()
	{
		this.FadeOutDone();
		if (m_rotateTween != null)
		{
			m_rotateTween.Kill();
			m_rotateTween = null;
		}
	}

	private void RotateTween()
	{
		Rotate.transform.localRotation = Quaternion.identity;
		TweenParms p_parms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 360f), true).Loops(-1).Ease(EaseType.Linear);
		m_rotateTween = HOTween.To(Rotate, 1f, p_parms);
	}
}
