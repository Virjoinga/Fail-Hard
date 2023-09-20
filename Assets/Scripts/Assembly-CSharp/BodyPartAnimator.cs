using UnityEngine;

public class BodyPartAnimator : MonoBehaviour
{
	public Rigidbody targetBodyPart;

	public Rigidbody root;

	public ConfigurableJoint joint;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if ((bool)joint)
		{
			joint.targetPosition = 10f * base.transform.localPosition;
		}
	}

	public void Enable()
	{
		joint = root.gameObject.AddComponent("ConfigurableJoint") as ConfigurableJoint;
		joint.connectedBody = targetBodyPart;
		joint.anchor = Vector3.zero;
		joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Free;
		joint.zMotion = ConfigurableJointMotion.Free;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;
		JointDrive xDrive = joint.xDrive;
		xDrive.mode = JointDriveMode.Position;
		xDrive.positionSpring = 500f;
		joint.xDrive = xDrive;
		xDrive = joint.yDrive;
		xDrive.mode = JointDriveMode.Position;
		xDrive.positionSpring = 500f;
		joint.yDrive = xDrive;
		xDrive = joint.zDrive;
		xDrive.mode = JointDriveMode.Position;
		xDrive.positionSpring = 500f;
		joint.zDrive = xDrive;
		joint.targetPosition = base.transform.localPosition;
	}
}
