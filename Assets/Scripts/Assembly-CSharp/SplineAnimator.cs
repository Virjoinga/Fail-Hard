using SplineUtilities;
using UnityEngine;

public class SplineAnimator : MonoBehaviour
{
	public Spline spline;

	public WrapMode wrapMode = WrapMode.Once;

	public float speed = 1f;

	public float offSet;

	public float passedTime;

	private void Update()
	{
		passedTime += Time.deltaTime * speed;
		float param = SplineUtils.WrapValue(passedTime + offSet, 0f, 1f, wrapMode);
		base.transform.position = spline.GetPositionOnSpline(param);
		base.transform.rotation = spline.GetOrientationOnSpline(param);
	}
}
