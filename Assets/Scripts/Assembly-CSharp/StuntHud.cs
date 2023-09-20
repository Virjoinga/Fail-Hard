using System.Collections.Generic;
using Game;
using UnityEngine;

public class StuntHud : MonoBehaviour
{
	public GameplayControls ControlButtons;

	public BonusDisplay BonusDisplay;

	public GameObject StayTargetCountdownPrefab;

	private List<GameObject> m_activeTargets;

	public Transform CountdownPosition;

	public Ads Ads;

	private GigStatus m_gigController;

	private Level m_currentLevel;

	private int m_prevHeadCondition;

	private int m_prevTorsoCondition;

	private int m_prevVehicleCondition;

	private void Start()
	{
		m_currentLevel = GameController.Instance.CurrentLevel;
		m_currentLevel.StayTargetTriggered += CurrentLevel_StayTargetTriggered;
		m_activeTargets = new List<GameObject>();
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		m_gigController.BonusCollected += m_gigController_BonusCollected;
	}

	private void Update()
	{
		if (m_gigController.player == null)
		{
			return;
		}
		List<VehiclePart> list = null;
		foreach (VehiclePart equippedGadget in GameController.Instance.Character.GetEquippedGadgets())
		{
			if (equippedGadget.ItemType == VehiclePartType.PlayerGadgetHead && equippedGadget.HasAction)
			{
				if (equippedGadget.MaxCondition <= 0)
				{
					continue;
				}
				if (equippedGadget.CurrentCondition <= 0)
				{
					ControlButtons.DisableEnable(ControlButtons.ActionHead, false);
					if (list == null)
					{
						list = new List<VehiclePart>();
					}
					list.Add(equippedGadget);
				}
				else if (m_prevHeadCondition != equippedGadget.CurrentCondition)
				{
					m_prevHeadCondition = equippedGadget.CurrentCondition;
					ControlButtons.UpdateLabel(ControlButtons.ActionHead, equippedGadget.CurrentCondition.ToString());
				}
			}
			else
			{
				if (equippedGadget.ItemType != VehiclePartType.PlayerGadgetBack || !equippedGadget.HasAction || equippedGadget.MaxCondition <= 0)
				{
					continue;
				}
				if (equippedGadget.CurrentCondition <= 0)
				{
					ControlButtons.DisableEnable(ControlButtons.ActionTorso, false);
					if (list == null)
					{
						list = new List<VehiclePart>();
					}
					list.Add(equippedGadget);
				}
				else if (m_prevTorsoCondition != equippedGadget.CurrentCondition)
				{
					m_prevTorsoCondition = equippedGadget.CurrentCondition;
					ControlButtons.UpdateLabel(ControlButtons.ActionTorso, equippedGadget.CurrentCondition.ToString());
				}
			}
		}
		foreach (VehiclePart equippedGadget2 in GameController.Instance.Character.CurrentVehicle.GetEquippedGadgets())
		{
			if (equippedGadget2.ItemType != VehiclePartType.VehicleGadget || !equippedGadget2.HasAction || equippedGadget2.MaxCondition <= 0)
			{
				continue;
			}
			if (equippedGadget2.CurrentCondition <= 0)
			{
				ControlButtons.DisableEnable(ControlButtons.ActionVehicle, false);
				if (list == null)
				{
					list = new List<VehiclePart>();
				}
				list.Add(equippedGadget2);
			}
			else if (m_prevVehicleCondition != equippedGadget2.CurrentCondition)
			{
				m_prevVehicleCondition = equippedGadget2.CurrentCondition;
				ControlButtons.UpdateLabel(ControlButtons.ActionVehicle, equippedGadget2.CurrentCondition.ToString());
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (VehiclePart item in list)
		{
			if (item.ItemType == VehiclePartType.VehicleGadget)
			{
				GameController.Instance.Character.CurrentVehicle.GadgetEquipped(item, false);
			}
			else
			{
				GameController.Instance.Character.GadgetEquipped(item, false);
			}
		}
	}

	private void m_gigController_BonusCollected(string text, int points, Vector3 pos, Vector3 offset)
	{
		BonusDisplay.CreateNewBonus(text, points);
	}

	private void OnDestroy()
	{
		m_currentLevel.StayTargetTriggered -= CurrentLevel_StayTargetTriggered;
	}

	private void CurrentLevel_StayTargetTriggered(StayTarget target)
	{
		if (target.StayState == StayTarget.StayTargetState.Active)
		{
			GameObject gameObject = GOTools.SpawnAsChild(StayTargetCountdownPrefab, CountdownPosition);
			gameObject.GetComponent<StayTargetCountdown>().SetData(target, 0);
			m_activeTargets.Add(gameObject);
		}
		else if (target.StayState == StayTarget.StayTargetState.Inactive)
		{
			GameObject gameObject2 = FindCountdown(target);
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<StayTargetCountdown>().Fail();
				m_activeTargets.Remove(gameObject2);
			}
		}
		else
		{
			GameObject gameObject3 = FindCountdown(target);
			if (gameObject3 != null)
			{
				gameObject3.GetComponent<StayTargetCountdown>().Success();
				m_activeTargets.Remove(gameObject3);
			}
		}
		RefreshCountdownPositions();
	}

	private void RefreshCountdownPositions()
	{
		for (int i = 0; i < m_activeTargets.Count; i++)
		{
			m_activeTargets[i].transform.position = CountdownPosition.position;
			m_activeTargets[i].transform.localPosition += i * 100 * Vector3.right - 50 * (m_activeTargets.Count - 1) * Vector3.right;
		}
	}

	private void OnEnable()
	{
		ButtonControlScheme.Instance.Reset();
		if (m_activeTargets != null)
		{
			m_activeTargets.Clear();
		}
	}

	private GameObject FindCountdown(StayTarget t)
	{
		foreach (GameObject activeTarget in m_activeTargets)
		{
			if (activeTarget.GetComponent<StayTargetCountdown>().Target == t)
			{
				return activeTarget;
			}
		}
		return null;
	}

	private void OnDisable()
	{
		Ads.Show();
		ControlButtons.Hide();
	}
}
