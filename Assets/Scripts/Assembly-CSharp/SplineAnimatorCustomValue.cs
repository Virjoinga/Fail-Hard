using SplineUtilities;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SplineAnimatorCustomValue : MonoBehaviour
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
		float customValueOnSpline = spline.GetCustomValueOnSpline(param);
		base.transform.position = spline.GetPositionOnSpline(param);
		base.transform.rotation = spline.GetOrientationOnSpline(param);
		base.renderer.material.color = Color.red * (1f - customValueOnSpline) + Color.blue * customValueOnSpline;
	}
}
