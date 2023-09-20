using System.Collections.Generic;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
	public List<Rigidbody> Rigidbodies;

	private bool m_isTriggered;

	private void OnTriggerEnter(Collider hit)
	{
		if (m_isTriggered || hit.gameObject.rigidbody == null)
		{
			return;
		}
		m_isTriggered = true;
		foreach (Rigidbody rigidbody in Rigidbodies)
		{
			rigidbody.isKinematic = false;
		}
	}
}
