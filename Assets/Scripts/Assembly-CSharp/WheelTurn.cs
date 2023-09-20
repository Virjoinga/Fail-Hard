using UnityEngine;

public class WheelTurn : MonoBehaviour
{
	public Transform frontWheelTransform;

	public Transform rearWheelTransform;

	public WheelCollider frontCollider;

	public WheelCollider rearCollider;

	public float throttle;

	private void Update()
	{
		frontCollider.motorTorque = throttle;
		frontWheelTransform.Rotate(Vector3.right * 6f * frontCollider.rpm * Time.deltaTime, Space.Self);
		rearWheelTransform.Rotate(Vector3.right * 6f * rearCollider.rpm * Time.deltaTime, Space.Self);
	}

	private void AddThrottle(float delta)
	{
		throttle += delta;
	}
}
