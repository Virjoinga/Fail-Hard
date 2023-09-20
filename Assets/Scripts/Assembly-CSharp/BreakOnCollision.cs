using UnityEngine;

public class BreakOnCollision : MonoBehaviour
{
	public GameObject Target;

	private void OnCollisionEnter(Collision hit)
	{
		Target.SendMessage("Break");
	}

	private void OnTriggerEnter(Collider hit)
	{
		Target.SendMessage("Break");
	}
}
