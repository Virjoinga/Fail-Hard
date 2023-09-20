using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Projector))]
public class AdjustProjectorFarClip : MonoBehaviour
{
	private Projector m_projector;

	private void Start()
	{
		m_projector = GetComponent<Projector>();
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, 10f))
		{
			m_projector.farClipPlane = hitInfo.distance + 0.1f;
		}
	}
}
