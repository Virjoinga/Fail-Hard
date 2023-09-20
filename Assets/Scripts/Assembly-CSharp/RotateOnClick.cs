using Holoville.HOTween;
using UnityEngine;

public class RotateOnClick : MonoBehaviour
{
	public float RotateAmount;

	public float Duration;

	public EaseType Easing;

	private void OnClick()
	{
		TweenParms p_parms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, RotateAmount), true).Ease(Easing);
		HOTween.To(base.transform, Duration, p_parms);
	}
}
