using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class HighlightButton : MonoBehaviour
{
	public GameObject ButtonObject;

	private UISprite ButtonSprite;

	public int LoopCount;

	public Color TargetColor;

	public float TargetScale;

	public bool PlayInStart;

	private Tweener m_colorTween;

	private Tweener m_scaleTween;

	private Tweener m_dimTween;

	private Color m_colorBackup;

	public void Start()
	{
		if (PlayInStart)
		{
			ShowHighlight();
		}
	}

	public void ShowHighlight()
	{
		ButtonSprite = ButtonObject.GetComponent<UISprite>();
		if (ButtonSprite != null)
		{
			DimColor(1f, LoopTweens);
		}
		else
		{
			LoopTweens();
		}
	}

	public void StopHighlight()
	{
		if (m_colorTween != null && !m_colorTween.isComplete)
		{
			m_colorTween.Kill();
		}
		if (m_scaleTween != null && !m_scaleTween.isComplete)
		{
			m_scaleTween.Kill();
		}
		if (ButtonSprite != null)
		{
			RestoreColor();
		}
	}

	private void OnDisable()
	{
		StopHighlight();
	}

	private void DimColor(float duration, TweenDelegate.TweenCallback onComplete)
	{
		m_colorBackup = ButtonSprite.color;
		TweenParms tweenParms = new TweenParms().Prop("color", TargetColor);
		tweenParms.OnComplete(onComplete);
		m_dimTween = HOTween.To(ButtonSprite, duration, tweenParms);
	}

	private void LoopTweens()
	{
		if (ButtonSprite != null)
		{
			TweenParms tweenParms = new TweenParms().Prop("color", m_colorBackup);
			tweenParms.Loops(LoopCount, LoopType.Yoyo);
			tweenParms.Ease(EaseType.EaseInOutSine);
			tweenParms.OnComplete(RestoreColor);
			m_colorTween = HOTween.To(ButtonSprite, 0.6f, tweenParms);
		}
		Vector3 vector = ButtonObject.transform.localScale * TargetScale;
		TweenParms tweenParms2 = new TweenParms().Prop("localScale", vector);
		tweenParms2.Loops(LoopCount, LoopType.Yoyo);
		m_scaleTween = HOTween.To(ButtonObject.transform, 0.6f, tweenParms2);
	}

	private void RestoreColor()
	{
		if (m_dimTween != null && !m_dimTween.isComplete)
		{
			m_dimTween.Kill();
		}
		if (ButtonSprite != null)
		{
			TweenParms p_parms = new TweenParms().Prop("color", m_colorBackup);
			m_dimTween = HOTween.To(ButtonSprite, 0.2f, p_parms);
		}
	}
}
