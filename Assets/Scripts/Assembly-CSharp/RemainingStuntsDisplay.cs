using System.Collections.Generic;
using UnityEngine;

public class RemainingStuntsDisplay : MonoBehaviour
{
	private GigStatus m_gigController;

	public UIGrid RemainingStuntsGrid;

	public GameObject StuntIndicatorPrefab;

	public GameObject RewardEffect;

	private List<GameObject> m_stuntIndicators;

	private int m_remaining;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		m_gigController.RemainingStuntsChanged += m_gigController_RemainingStuntsChanged;
		InitGrid();
	}

	private void m_gigController_RemainingStuntsChanged(int remaining, bool rewardDelta)
	{
		for (int i = 0; i < m_stuntIndicators.Count; i++)
		{
			if (i < remaining)
			{
				m_stuntIndicators[i].GetComponentInChildren<UISprite>().spriteName = "health_counter";
				continue;
			}
			if (i < m_remaining && rewardDelta)
			{
				GameObject gameObject = GOTools.SpawnAsChild(RewardEffect, m_stuntIndicators[i].transform);
				gameObject.transform.position = m_stuntIndicators[i].transform.position;
			}
			m_stuntIndicators[i].GetComponentInChildren<UISprite>().spriteName = "health_counter_empty";
		}
		m_remaining = remaining;
	}

	private void InitGrid()
	{
		m_stuntIndicators = new List<GameObject>();
		while (m_stuntIndicators.Count < m_gigController.RemainingStunts)
		{
			GameObject item = NGUITools.AddChild(RemainingStuntsGrid.gameObject, StuntIndicatorPrefab);
			m_stuntIndicators.Add(item);
		}
		RemainingStuntsGrid.Reposition();
		m_gigController_RemainingStuntsChanged(m_gigController.RemainingStunts, false);
	}
}
