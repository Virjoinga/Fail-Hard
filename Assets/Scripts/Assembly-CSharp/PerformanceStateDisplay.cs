using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class PerformanceStateDisplay : MonoBehaviour
{
	private UILabel m_label;

	private PerformanceChecker m_checker;

	private PerformanceChecker.PerformanceState m_currentState;

	private void Start()
	{
		m_label = GetComponent<UILabel>();
		m_checker = PerformanceChecker.Instance;
		if (m_checker == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		PerformanceChecker.PerformanceState performanceState = m_checker.CurrentState();
		m_label.text = Enum.GetName(typeof(PerformanceChecker.PerformanceState), performanceState);
		m_currentState = performanceState;
		StartCoroutine(CheckState());
	}

	private IEnumerator CheckState()
	{
		while (true)
		{
			yield return new WaitForSeconds(2f);
			PerformanceChecker.PerformanceState state = m_checker.CurrentState();
			if (state != m_currentState)
			{
				m_label.text = Enum.GetName(typeof(PerformanceChecker.PerformanceState), state);
				m_currentState = state;
			}
		}
	}
}
