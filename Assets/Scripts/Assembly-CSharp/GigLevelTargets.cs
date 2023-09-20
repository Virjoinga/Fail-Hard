using System.Collections.Generic;
using UnityEngine;

public class GigLevelTargets : MonoBehaviour
{
	private GigStatus m_gigController;

	public UISprite ProgressBar;

	public UILabel CoinsLabel;

	public List<StarBarStar> Stars;

	private List<float> m_starLimits;

	private int m_maxLimit;

	private int m_prevCoins;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		m_gigController.GigStarsChanged += m_gigController_GigStarsChanged;
		m_gigController_GigStarsChanged(m_gigController.StarsAtGigStart(), 0);
		ProgressBar.fillAmount = 0f;
		m_prevCoins = 0;
		m_starLimits = new List<float>();
		m_maxLimit = m_gigController.GetStarLimit(2);
		m_starLimits.Add((float)m_gigController.GetStarLimit(0) / (float)m_maxLimit);
		m_starLimits.Add((float)m_gigController.GetStarLimit(1) / (float)m_maxLimit);
		m_starLimits.Add(1f);
		Stars[0].transform.localPosition -= (1f - m_starLimits[0]) * Stars[2].transform.localPosition;
		Stars[1].transform.localPosition -= (1f - m_starLimits[1]) * Stars[2].transform.localPosition;
	}

	private void m_gigController_GigStarsChanged(int collected, int deltaToNextLimit)
	{
		for (int i = 0; i < collected; i++)
		{
			if (!Stars[i].Active)
			{
				bool silent = m_gigController.GigStatistics.TotalGigCoins == 0;
				Stars[i].Activate(silent);
			}
		}
	}

	private void Update()
	{
		ProgressBar.fillAmount = (float)m_gigController.GigStatistics.TotalGigCoins / (float)m_maxLimit;
		if (m_prevCoins != m_gigController.GigStatistics.TotalGigCoins)
		{
			m_prevCoins = m_gigController.GigStatistics.TotalGigCoins;
			CoinsLabel.text = m_gigController.GigStatistics.TotalGigCoins.ToString();
		}
	}
}
