using UnityEngine;

public class EffectOnCollision : MonoBehaviour
{
	public GameObject effect;

	public float minVelocity;

	private void Start()
	{
		Debug.LogError("OBSOLETE 5.9.2013: This script should not be used anymore!");
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.rigidbody != null && Mathf.Abs(collision.gameObject.rigidbody.velocity.magnitude) > minVelocity)
		{
			Object.Instantiate(effect, collision.transform.position, Quaternion.identity);
		}
	}
}
