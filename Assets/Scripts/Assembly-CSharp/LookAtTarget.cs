using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
	public Transform Target;

	private Transform m_transform;

	private void Awake()
	{
		m_transform = base.transform;
	}

	private void LateUpdate()
	{
		m_transform.LookAt(Target);
	}
}
