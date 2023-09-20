using UnityEngine;

public class RandomMover : MonoBehaviour
{
	public Vector3 Bounds;

	public float Velocity;

	private Vector3 m_originalCenter;

	private void Start()
	{
		m_originalCenter = base.transform.position;
		RandomizeNewPosition();
	}

	private void RandomizeNewPosition()
	{
		Vector3 vector = new Vector3(Random.Range(0f - Bounds.x, Bounds.x), Random.Range(0f - Bounds.y, Bounds.y), Random.Range(0f - Bounds.z, Bounds.z));
		float magnitude = (m_originalCenter + vector - base.transform.position).magnitude;
		if (magnitude > 0.1f)
		{
			TweenPosition.Begin(base.gameObject, magnitude / Velocity, m_originalCenter + vector);
			Invoke("RandomizeNewPosition", magnitude / Velocity);
		}
		else
		{
			RandomizeNewPosition();
		}
	}
}
