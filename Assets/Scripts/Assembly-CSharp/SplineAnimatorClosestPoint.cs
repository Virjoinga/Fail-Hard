using SplineUtilities;
using UnityEngine;

public class SplineAnimatorClosestPoint : MonoBehaviour
{
	public Spline spline;

	public WrapMode wMode = WrapMode.Once;

	public Transform target;

	public int iterations = 5;

	public float diff = 0.5f;

	public float offset;

	private void Update()
	{
		if (!(target == null) && !(spline == null))
		{
			float param = SplineUtils.WrapValue(spline.GetClosestPointParam(target.position, iterations) + offset, 0f, 1f, wMode);
			base.transform.position = spline.GetPositionOnSpline(param);
			base.transform.rotation = spline.GetOrientationOnSpline(param);
		}
	}
}
