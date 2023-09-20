using UnityEngine;

public class CareerLevelUp : MonoBehaviour
{
	public UIPanel Common;

	public UILabel CurrentStarsLabel;

	public UILabel RewardLabel;

	public UILabel NextStarsLabel;

	private float m_accidentFilter = 1f;

	private void OnEnable()
	{
		base.collider.enabled = false;
		Invoke("AccidentFilter", m_accidentFilter);
		Common.alpha = 0f;
	}

	private void AccidentFilter()
	{
		base.collider.enabled = true;
	}

	public void SetData(UIPanel toPanel, int currentStars, int reward, int nextStars)
	{
		CurrentStarsLabel.text = "x" + currentStars;
		RewardLabel.text = reward + "@@";
		NextStarsLabel.text = "Next reward: " + nextStars + " stars";
		GetComponent<NavigateFromTo>().toPanel = toPanel;
	}

	private void OnClick()
	{
		Common.alpha = 1f;
		GetComponent<NavigateFromTo>().ManualTrigger();
	}
}
