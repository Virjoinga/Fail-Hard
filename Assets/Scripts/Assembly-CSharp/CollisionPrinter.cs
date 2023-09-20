using UnityEngine;

public class CollisionPrinter : MonoBehaviour
{
	public int collCount;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider hit)
	{
		collCount++;
	}

	private void OnTriggerStay(Collider hit)
	{
	}

	private void OnTriggerExit(Collider hit)
	{
		collCount--;
	}

	private void OnCollisionEnter(Collision hit)
	{
		collCount++;
	}

	private void OnCollisionStay(Collision hit)
	{
	}

	private void OnCollisionExit(Collision hit)
	{
		collCount--;
	}
}
