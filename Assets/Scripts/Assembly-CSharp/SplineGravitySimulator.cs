using UnityEngine;

public class SplineGravitySimulator : MonoBehaviour
{
	public Spline spline;

	public float gravityConstant = 9.81f;

	public int iterations = 5;

	private void Start()
	{
		base.rigidbody.useGravity = false;
	}

	private void FixedUpdate()
	{
		if (!(base.rigidbody == null) && !(spline == null))
		{
			Vector3 positionOnSpline = spline.GetPositionOnSpline(spline.GetClosestPointParam(base.rigidbody.position, iterations));
			Vector3 vector = positionOnSpline - base.rigidbody.position;
			Vector3 force = vector * Mathf.Pow(vector.magnitude, -3f) * gravityConstant * base.rigidbody.mass;
			base.rigidbody.AddForce(force);
		}
	}
}
