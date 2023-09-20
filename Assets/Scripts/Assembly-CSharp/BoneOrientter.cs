using UnityEngine;

public class BoneOrientter : MonoBehaviour
{
	public Transform targetPoint;

	public ConfigurableJoint cj;

	public Vector3 boneAxis;

	private void Start()
	{
		if (cj == null)
		{
			cj = GetComponent<ConfigurableJoint>();
		}
	}

	private void FixedUpdate()
	{
		SolveIK();
	}

	private void SolveIK()
	{
		Vector3 normalized = (targetPoint.position - cj.connectedBody.position).normalized;
		cj.targetRotation = Quaternion.FromToRotation(boneAxis, cj.connectedBody.transform.InverseTransformDirection(normalized));
	}
}
