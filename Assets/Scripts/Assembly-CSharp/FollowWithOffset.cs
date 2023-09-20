using UnityEngine;

[ExecuteInEditMode]
public class FollowWithOffset : MonoBehaviour
{
	public GameObject Target;

	public float YOffset;

	private Transform m_thisTransform;

	private Transform m_targetTransform;

	private void Start()
	{
		m_targetTransform = Target.transform;
		m_thisTransform = base.transform;
	}

	private void Update()
	{
		Vector3 position = m_targetTransform.position;
		position.y += YOffset;
		m_thisTransform.position = position;
	}
}
