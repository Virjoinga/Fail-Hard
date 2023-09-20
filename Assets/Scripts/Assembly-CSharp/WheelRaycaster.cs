using UnityEngine;

public class WheelRaycaster : MonoBehaviour
{
	public Transform RayDirection;

	public float Radius;

	public bool IsContact;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (Physics.Raycast(base.transform.position, RayDirection.up.normalized, Radius))
		{
			IsContact = true;
		}
		else
		{
			IsContact = false;
		}
	}
}
