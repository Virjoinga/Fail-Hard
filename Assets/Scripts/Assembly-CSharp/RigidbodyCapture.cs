using UnityEngine;

public class RigidbodyCapture : MonoBehaviour
{
	private bool isTracking;

	public string TrackedTag;

	private Rigidbody m_target;

	public float distance;

	public VectorData data;

	private Vector3 m_prevPos;

	private void FixedUpdate()
	{
		if (isTracking && m_target != null)
		{
			if ((m_prevPos - m_target.rigidbody.position).magnitude >= distance)
			{
				WriteSample();
			}
		}
		else
		{
			isTracking = false;
		}
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (isTracking || !(hit.gameObject.tag == TrackedTag))
		{
			return;
		}
		if (TrackedTag == "Vehicle")
		{
			m_target = hit.attachedRigidbody;
		}
		else
		{
			GameObject gameObject = GameObject.Find("PhysicsJackson/Armature/spine_1");
			if (!(gameObject != null))
			{
				return;
			}
			m_target = gameObject.rigidbody;
		}
		isTracking = true;
		data.data.Clear();
		WriteSample();
	}

	private void OnTriggerExit(Collider hit)
	{
		if (isTracking && hit.attachedRigidbody == m_target)
		{
			isTracking = false;
			m_target = null;
		}
	}

	private void WriteSample()
	{
		if (!(m_target == null))
		{
			m_prevPos = m_target.rigidbody.worldCenterOfMass;
			Vector3 item = data.transform.InverseTransformPoint(m_target.rigidbody.worldCenterOfMass);
			data.data.Add(item);
		}
	}
}
