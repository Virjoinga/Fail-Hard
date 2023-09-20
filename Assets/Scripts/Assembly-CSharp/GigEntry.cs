using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class GigEntry : MonoBehaviour
{
	private bool m_isBlocking;

	public List<GameObject> TutorialPrefabs;

	public List<UIPanel> GigPanels;

	private GameObject m_tutorialImage;

	private void Start()
	{
		HardCodedTutorialSelection();
		if (!m_isBlocking)
		{
			StartGig();
		}
	}

	private void HardCodedTutorialSelection()
	{
		Level currentLevel = GameController.Instance.CurrentLevel;
		if (!currentLevel.IsCompleted)
		{
			if (currentLevel.Parameters.Name == "GravelPitLoop")
			{
				m_tutorialImage = NGUITools.AddChild(base.gameObject, TutorialPrefabs[0]);
				m_isBlocking = true;
			}
			else if (currentLevel.Parameters.Name == "GravelPitCrateTruck")
			{
				m_tutorialImage = NGUITools.AddChild(base.gameObject, TutorialPrefabs[1]);
				m_isBlocking = true;
			}
			else if (currentLevel.Parameters.Name == "GravelPitMattresses")
			{
				m_tutorialImage = NGUITools.AddChild(base.gameObject, TutorialPrefabs[2]);
				m_isBlocking = true;
			}
			else if (currentLevel.Parameters.Name == "GravelPitGutterDown")
			{
				m_tutorialImage = NGUITools.AddChild(base.gameObject, TutorialPrefabs[3]);
				m_isBlocking = true;
			}
			else if (currentLevel.Parameters.Name == "SnowSkiJump1")
			{
				m_tutorialImage = NGUITools.AddChild(base.gameObject, TutorialPrefabs[4]);
				m_isBlocking = true;
			}
		}
	}

	private void StartGig()
	{
		foreach (UIPanel gigPanel in GigPanels)
		{
			gigPanel.gameObject.SetActive(true);
		}
		if (m_tutorialImage != null)
		{
			m_tutorialImage.SetActive(false);
		}
		TweenParms p_parms = new TweenParms().Prop("alpha", 0).OnComplete(FadeOutDone);
		HOTween.To(GetComponent<UIPanel>(), 0.2f, p_parms);
	}

	public void OnClick()
	{
		if (m_isBlocking)
		{
			StartGig();
			m_isBlocking = false;
		}
	}

	private void FadeOutDone()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		gameObject.GetComponentInChildren<GigStatus>().StartGig();
		base.gameObject.SetActive(false);
	}
}
