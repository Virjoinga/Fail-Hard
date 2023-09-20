using UnityEngine;

public class HeadTracking : MonoBehaviour
{
	private ConfigurableJoint cj;

	private void Start()
	{
		cj = GetComponent<ConfigurableJoint>();
	}

	private void FixedUpdate()
	{
		Vector3 right = Vector3.right;
		Vector3 from = -base.transform.forward;
		Vector3 zero = Vector3.zero;
		zero.y = Vector3.Angle(from, right);
		if (from.y < right.y)
		{
			zero.y = 0f - zero.y;
		}
		cj.targetRotation = Quaternion.Euler(zero);
	}

	public void DisableTracking()
	{
		JointDrive angularYZDrive = cj.angularYZDrive;
		angularYZDrive.mode = JointDriveMode.None;
		cj.angularYZDrive = angularYZDrive;
	}
}
