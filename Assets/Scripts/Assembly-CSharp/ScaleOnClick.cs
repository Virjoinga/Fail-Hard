using Holoville.HOTween;
using UnityEngine;

public class ScaleOnClick : MonoBehaviour
{
	public float ClickScale;

	public float TweenDuration;

	private Tweener m_clickTweener;

	private void OnClick()
	{
		if (m_clickTweener == null)
		{
			Vector3 vector = base.transform.localScale * ClickScale;
			TweenParms tweenParms = new TweenParms().Prop("localScale", vector);
			tweenParms.Loops(2, LoopType.Yoyo);
			tweenParms.OnComplete(TweenDone);
			m_clickTweener = HOTween.To(base.transform, TweenDuration / 2f, tweenParms);
		}
	}

	private void TweenDone()
	{
		m_clickTweener = null;
	}
}
