using System.Collections.Generic;
using Game;
using UnityEngine;

public class MechanicsIndicator : MonoBehaviour
{
	public List<UISprite> idleMechs;

	public List<UISprite> busyMechs;

	private bool refreshNeeded;

	private void Start()
	{
		foreach (Mechanic mechanic in GameController.Instance.Character.Mechanics)
		{
			mechanic.StateChanged += OnStateChanged;
		}
	}

	private void OnEnable()
	{
		UpdateIndicators();
	}

	public void OnStateChanged(Mechanic.MechanicState s)
	{
		refreshNeeded = true;
	}

	private void Update()
	{
		if (refreshNeeded)
		{
			UpdateIndicators();
		}
	}

	public void UpdateIndicators()
	{
		int num = 0;
		int num2 = 0;
		foreach (Mechanic mechanic in GameController.Instance.Character.Mechanics)
		{
			if (mechanic.State == Mechanic.MechanicState.Busy)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		for (int i = 0; i < idleMechs.Count; i++)
		{
			idleMechs[i].gameObject.SetActive(false);
			busyMechs[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < num; j++)
		{
			idleMechs[j].gameObject.SetActive(false);
			busyMechs[j].gameObject.SetActive(true);
		}
		for (int k = num; k < num + num2; k++)
		{
			idleMechs[k].gameObject.SetActive(true);
			busyMechs[k].gameObject.SetActive(false);
		}
		refreshNeeded = false;
	}
}
