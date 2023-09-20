using Game;
using UnityEngine;

public class CoinCollider : MonoBehaviour
{
	public float Value;

	public void SetPhysics(bool isPhysics)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.isTrigger = !isPhysics;
		}
		base.rigidbody.isKinematic = !isPhysics;
		if (isPhysics)
		{
			DiamondAnimator component = GetComponent<DiamondAnimator>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (!(hit.gameObject.tag != "Vehicle") || !(hit.gameObject.tag != "Player"))
		{
			GameController.Instance.Character.Coins += (int)Value;
			Object.Destroy(base.gameObject, 0.01f);
		}
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (!(hit.gameObject.tag != "Vehicle") || !(hit.gameObject.tag != "Player"))
		{
			GameController.Instance.Character.Coins += (int)Value;
			Object.Destroy(base.gameObject, 0.01f);
		}
	}
}
