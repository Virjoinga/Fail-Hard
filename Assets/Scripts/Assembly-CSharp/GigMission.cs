using Game;
using UnityEngine;

public class GigMission : MonoBehaviour
{
	public UILabel MissionDescription;

	private GigStatus m_gigController;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		Refresh();
	}

	public void Refresh()
	{
		if (GameController.Instance.CurrentLevel.IsCompleted)
		{
			int currentStage = GameController.Instance.CurrentBundle.CurrentStage;
			if (currentStage == GameController.Instance.CurrentBundle.Stages.Count - 1)
			{
				base.gameObject.SetActive(false);
				return;
			}
			int num = Mathf.Max(GameController.Instance.CurrentBundle.StageCriteria(currentStage) - GameController.Instance.CurrentBundle.AchievedTargets, 0);
			MissionDescription.text = "Get " + num + " stars to open next stage.";
		}
		else
		{
			MissionDescription.text = "Earn " + m_gigController.GetCoinsToNextStarLimit() + " coins to unlock next level.";
		}
	}

	private void OnEnable()
	{
		if (!(m_gigController == null))
		{
			Refresh();
		}
	}
}
