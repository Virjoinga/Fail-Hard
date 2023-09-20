using System;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Fluctuation : MonoBehaviour
{
	[Serializable]
	public class RangeField
	{
		public float max;

		public float min;
	}

	private const float Epsilon = 0.05f;

	public RangeField Range;

	private Light m_light;

	private bool m_growing;

	private float m_speed;

	private void Start()
	{
		m_light = GetComponent<Light>();
		m_speed = 0.001f;
	}

	private void Update()
	{
		if (m_growing)
		{
			if (m_light.intensity < Range.max - 0.05f)
			{
				base.light.intensity = Mathf.Lerp(m_light.intensity, Range.max, m_speed);
			}
			else
			{
				m_growing = false;
				m_speed = UnityEngine.Random.Range(0.001f, 0.004f) / 10f;
			}
		}
		if (!m_growing)
		{
			if (m_light.intensity > Range.min + 0.05f)
			{
				base.light.intensity = Mathf.Lerp(m_light.intensity, Range.min, m_speed);
				return;
			}
			m_growing = true;
			m_speed = UnityEngine.Random.Range(0.001f, 0.004f) / 5f;
		}
	}
}
