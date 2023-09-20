using UnityEngine;

public class BubbleScript : MonoBehaviour
{
	public static float BUBBLE_SPEED = 1f;

	private float initTime;

	private bool inWater;

	private Transform m_transform;

	private void Start()
	{
		m_transform = base.transform;
		initTime = Time.time;
	}

	private void Update()
	{
		m_transform.position += Vector3.up * Time.deltaTime * BUBBLE_SPEED;
		if (!inWater || Time.time - initTime > 3f)
		{
			PoolManager.Pools["Main"].Despawn(m_transform);
		}
	}

	private void OnTriggerStay(Collider hit)
	{
		if (!inWater && (bool)hit.gameObject.GetComponent<WaterSplashOnCollision>())
		{
			inWater = true;
		}
	}

	private void OnTriggerExit(Collider hit)
	{
		if ((bool)hit.gameObject.GetComponent<WaterSplashOnCollision>())
		{
			PoolManager.Pools["Main"].Despawn(m_transform);
		}
	}
}
