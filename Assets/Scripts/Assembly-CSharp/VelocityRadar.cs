using UnityEngine;

public class VelocityRadar : MonoBehaviour
{
	private bool triggered;

	public float distance;

	private void OnTriggerEnter(Collider hit)
	{
		if (!triggered)
		{
			MonoBehaviour.print(distance + "\t" + hit.attachedRigidbody.velocity.x);
			triggered = true;
		}
	}
}
