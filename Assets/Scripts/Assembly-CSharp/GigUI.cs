using System.Collections.Generic;
using UnityEngine;

public class GigUI : MonoBehaviour
{
	public List<GameObject> ActivateOnAwake;

	public GameObject RemainingStunts;

	public GameObject CrowdMeter;

	public GameObject Stars;

	public void Awake()
	{
		ActivateOnAwake.ForEach(delegate(GameObject x)
		{
			x.SetActive(true);
		});
	}

	public void StuntOver()
	{
		if ((bool)GetComponentInChildren<EndStuntButton>())
		{
			GetComponentInChildren<EndStuntButton>().TriggerEndStunt();
		}
	}

	public void StuntStarted()
	{
		if (GetComponentInChildren<LevelIntroHUD>() != null)
		{
			GetComponentInChildren<LevelIntroHUD>().OnStuntStarted();
		}
	}

	public void ShowHeader(bool isShown)
	{
		RemainingStunts.SetActive(isShown);
		CrowdMeter.SetActive(isShown);
		Stars.SetActive(isShown);
	}
}
