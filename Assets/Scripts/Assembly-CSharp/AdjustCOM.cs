using UnityEngine;

public class AdjustCOM : MonoBehaviour
{
	public Vector3 centerOfMass;

	private void Start()
	{
		base.rigidbody.centerOfMass = centerOfMass;
	}
}
