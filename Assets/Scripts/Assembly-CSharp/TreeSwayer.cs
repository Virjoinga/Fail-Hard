using Holoville.HOTween;
using UnityEngine;

public class TreeSwayer : MonoBehaviour
{
	[Range(1.5f, 15f)]
	public float MaxSwayAngle;

	public float SwayLoopDuration;

	public EaseType Easing;

	private Tweener m_swayTweener;

	private void Start()
	{
		float num = Random.Range(1f, MaxSwayAngle);
		Quaternion localRotation = base.transform.localRotation;
		num += localRotation.eulerAngles.x;
		Vector3 vector = new Vector3(num, localRotation.eulerAngles.y, localRotation.eulerAngles.z);
		TweenParms tweenParms = new TweenParms().Prop("localRotation", vector).Loops(-1, LoopType.Yoyo);
		tweenParms.Ease(Easing);
		m_swayTweener = HOTween.To(base.transform, SwayLoopDuration, tweenParms);
	}

	private void OnBecameInvisible()
	{
		m_swayTweener.Pause();
	}

	private void OnBecameVisible()
	{
		m_swayTweener.Play();
	}
}
