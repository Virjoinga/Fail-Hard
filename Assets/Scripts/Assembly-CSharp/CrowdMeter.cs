using System.Collections.Generic;
using UnityEngine;

public class CrowdMeter : MonoBehaviour
{
	public Transform Pointer;

	private GigStatus m_gigController;

	public List<GameObject> Lights;

	private float m_multiplier;

	private float m_prevMultiplier;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		m_multiplier = 0f;
	}

	private void Update()
	{
		if (m_multiplier != m_gigController.GigStatistics.CrowdMeterValue)
		{
			m_multiplier += (m_gigController.GigStatistics.CrowdMeterValue - m_multiplier) * Time.deltaTime * 4f;
			if (Mathf.Abs(m_multiplier - m_gigController.GigStatistics.CrowdMeterValue) < 0.04f)
			{
				m_multiplier = m_gigController.GigStatistics.CrowdMeterValue;
			}
			float num = 0f - (50f * m_multiplier - 100f);
			if (num > 0f)
			{
				num = 0f;
			}
			Pointer.localPosition = num * Vector3.right;
			RefreshLights();
			m_prevMultiplier = m_multiplier;
		}
	}

	private void RefreshLights()
	{
		int num = LightIndex(m_prevMultiplier);
		int num2 = LightIndex(m_multiplier);
		if (num2 != num)
		{
			if (num >= 0)
			{
				Lights[num].SetActive(false);
			}
			if (num2 >= 0)
			{
				Lights[num2].SetActive(true);
			}
		}
	}

	private int LightIndex(float multiplier)
	{
		int num = (int)(multiplier - 2f);
		if (num >= Lights.Count)
		{
			num = Lights.Count - 1;
		}
		else if (num < 0)
		{
			num = -1;
		}
		return num;
	}
}
