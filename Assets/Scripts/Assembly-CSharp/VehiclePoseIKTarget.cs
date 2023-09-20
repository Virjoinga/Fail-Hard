using UnityEngine;

public class VehiclePoseIKTarget : MonoBehaviour
{
	public HingeJoint hj;

	private Vector3 originalPos;

	private Vector3 targetPos;

	private bool initialized;

	private Rigidbody connectedBodyPart;

	private float slerpper;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if ((bool)hj && initialized && slerpper < 1f)
		{
			slerpper += 0.5f * Time.deltaTime;
			Vector3 position = Vector3.Slerp(base.rigidbody.position, targetPos, slerpper);
			base.rigidbody.MovePosition(position);
		}
	}

	public void AttachToVehicle(Rigidbody bodyPart, GameObject vehicle)
	{
		connectedBodyPart = bodyPart;
		originalPos = base.rigidbody.position;
		base.rigidbody.MovePosition(bodyPart.position);
		Invoke("DelayedJointCreation", 0.01f);
	}

	public void DelayedJointCreation()
	{
		hj = base.gameObject.AddComponent("HingeJoint") as HingeJoint;
		hj.axis = Vector3.forward;
		hj.anchor = Vector3.zero;
		hj.connectedBody = connectedBodyPart;
		initialized = true;
		targetPos = originalPos;
	}
}
