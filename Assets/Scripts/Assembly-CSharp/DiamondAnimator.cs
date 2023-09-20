using Holoville.HOTween;
using UnityEngine;

public class DiamondAnimator : MonoBehaviour
{
	public Transform ToAnimate;

	public float RotationSpeed;

	public float BobbingSpeed;

	public float BobbingLength;

	private void Start()
	{
		Vector3 localPosition = ToAnimate.localPosition;
		localPosition.y += BobbingLength;
		TweenParms p_parms = new TweenParms().Prop("localPosition", localPosition).Loops(-1, LoopType.Yoyo);
		HOTween.To(ToAnimate, 1f / BobbingSpeed, p_parms);
		TweenParms p_parms2 = new TweenParms().Prop("localRotation", new Vector3(0f, 360f, 0f), true).Loops(-1).Ease(EaseType.Linear);
		HOTween.To(ToAnimate, 1f / RotationSpeed, p_parms2);
	}

	private void Update()
	{
	}
}
