using UnityEngine;

public class StuntBackButton : MonoBehaviour
{
	public UIPanel BackToScore;

	public UIPanel BackToStuntOver;

	public NavigateFromTo m_navi;

	private GigStatus gigController;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		gigController = gameObject.GetComponentInChildren<GigStatus>();
	}

	private void OnClick()
	{
		gigController.CancelStunt();
		m_navi.NavigateBack();
	}
}
