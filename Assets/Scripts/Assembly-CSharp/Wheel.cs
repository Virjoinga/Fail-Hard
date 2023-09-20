using UnityEngine;

public class Wheel : MonoBehaviour
{
	public WheelCollider wheelCollider;

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.right * 6f * wheelCollider.rpm * Time.deltaTime, Space.Self);
	}
}
