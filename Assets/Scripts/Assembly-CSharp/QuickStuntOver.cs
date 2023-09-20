using UnityEngine;

public class QuickStuntOver : MonoBehaviour
{
	public float StuntOverDelay;

	private GigStatus m_gigController;

	public Ads Ads;

	private int m_step;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		m_gigController.StateChanged += m_gigController_StateChanged;
	}

	private void m_gigController_StateChanged(GigStatus.GigState newState)
	{
		if (m_step == 2 && newState == GigStatus.GigState.StuntReadyToStart)
		{
			Navigate();
		}
	}

	private void OnEnable()
	{
		m_step = 0;
		Ads.Show();
	}

	public void SkipStuntOver()
	{
		if (m_step <= 0)
		{
			TurnCameraToSpawnPoint();
			SpawnPlayer();
			Ads.Hide();
		}
	}

	public void TurnCameraToSpawnPoint()
	{
		m_step++;
		m_gigController.PointCameraAtSpawnPoint();
	}

	private void SpawnPlayer()
	{
		m_step++;
		m_gigController.StartStunt();
	}

	private void Navigate()
	{
		m_step++;
		GetComponent<NavigateFromTo>().ManualTrigger();
	}

	private void OnClick()
	{
		SkipStuntOver();
	}
}
