using Holoville.HOTween;
using UnityEngine;

public class CloudAnimator : MonoBehaviour
{
	public Vector3 MovingRange;

	public float Duration;

	public EaseType Easing;

	private void Start()
	{
		TweenParms tweenParms = new TweenParms().Prop("localPosition", MovingRange, true).Loops(-1, LoopType.Yoyo);
		tweenParms.Ease(Easing);
		HOTween.To(base.transform, Duration, tweenParms);
	}
}
