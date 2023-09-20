using Game;
using Game.Util;
using UnityEngine;

public class LevelIntroHUD : MonoBehaviour
{
	private GigStatus m_gigStatus;

	public GameObject MissionDisplay;

	public GameObject GigTargets;

	public ButtonSlider NavigationButtons;

	public GameObject RestartButton;

	public GameObject GridButton;

	public GameplayControls ControlButtons;

	public GigUIHelpOverlay HelpOverlay;

	private LevelIntro levelIntro;

	private bool m_firstStunt = true;

	private bool m_garageVisited;

	private GigStatus.GigState m_cachedState;

	public Ads Ads;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		if ((bool)gameObject)
		{
			m_gigStatus = gameObject.GetComponentInChildren<GigStatus>();
			m_gigStatus.StateChanged += m_gigStatus_StateChanged;
			levelIntro = gameObject.GetComponentInChildren<LevelIntro>();
			if (levelIntro != null)
			{
				levelIntro.IntroStateChanged += OnIntroStateChanged;
			}
		}
		else
		{
			Logger.Error("No level root was found in scene!");
		}
	}

	private void m_gigStatus_StateChanged(GigStatus.GigState newState)
	{
		if (m_cachedState != newState)
		{
			if (newState == GigStatus.GigState.GigReadyToStart && m_firstStunt)
			{
				StartIntro();
				m_firstStunt = false;
			}
			else if (newState == GigStatus.GigState.StuntReadyToStart)
			{
				levelIntro.EndIntro();
				m_gigStatus.PointCameraAtPlayer();
			}
			m_cachedState = newState;
		}
	}

	private void OnEnable()
	{
		Ads.Hide();
		if (m_gigStatus != null && m_gigStatus.player != null)
		{
			m_gigStatus.player.ControlsEnabled = true;
		}
		NGUITools.FindInParents<GigUI>(base.gameObject).ShowHeader(true);
		if (!m_firstStunt)
		{
			SetupStuntState();
		}
		if (GameController.Instance.CurrentBundle.State == BundleState.BundleCompletePending)
		{
			NavigationButtons.RemoveObject(RestartButton);
			NavigationButtons.RemoveObject(GridButton);
		}
	}

	public void OnIntroStateChanged(bool playing)
	{
		if (playing)
		{
			SetupIntroState(true);
		}
		else if (base.gameObject.activeInHierarchy)
		{
			if (!m_garageVisited)
			{
				GetComponentInChildren<SpawnGarageView>().ManualTrigger();
				m_garageVisited = true;
			}
			else
			{
				SetupStuntState();
			}
		}
		else
		{
			levelIntro.StartIntro();
		}
	}

	private void SetupIntroState(bool animate)
	{
		ControlButtons.Hide();
		MissionDisplay.GetComponent<GigMission>().Refresh();
		MissionDisplay.GetComponent<ShowHider>().Show();
	}

	private void SetupStuntState()
	{
		string headIcon = string.Empty;
		string torsoIcon = string.Empty;
		string vehicleIcon = string.Empty;
		string headLabel = string.Empty;
		string torsoLabel = string.Empty;
		string vehicleLabel = string.Empty;
		foreach (VehiclePart equippedGadget in GameController.Instance.Character.GetEquippedGadgets())
		{
			if (equippedGadget.ItemType == VehiclePartType.PlayerGadgetHead && equippedGadget.HasAction)
			{
				headIcon = equippedGadget.IconTextureName + "_on";
				if (equippedGadget.MaxCondition > 0)
				{
					if (equippedGadget.CurrentCondition <= 0)
					{
						headIcon = string.Empty;
					}
					headLabel = equippedGadget.CurrentCondition.ToString();
				}
			}
			else
			{
				if (equippedGadget.ItemType != VehiclePartType.PlayerGadgetBack || !equippedGadget.HasAction)
				{
					continue;
				}
				torsoIcon = equippedGadget.IconTextureName + "_on";
				if (equippedGadget.MaxCondition > 0)
				{
					if (equippedGadget.CurrentCondition <= 0)
					{
						torsoIcon = string.Empty;
					}
					torsoLabel = equippedGadget.CurrentCondition.ToString();
				}
			}
		}
		foreach (VehiclePart equippedGadget2 in GameController.Instance.Character.CurrentVehicle.GetEquippedGadgets())
		{
			if (equippedGadget2.ItemType != VehiclePartType.VehicleGadget || !equippedGadget2.HasAction)
			{
				continue;
			}
			vehicleIcon = equippedGadget2.IconTextureName + "_on";
			if (equippedGadget2.MaxCondition > 0)
			{
				if (equippedGadget2.CurrentCondition <= 0)
				{
					vehicleIcon = string.Empty;
				}
				vehicleLabel = equippedGadget2.CurrentCondition.ToString();
			}
		}
		ControlButtons.Show(headIcon, headLabel, torsoIcon, torsoLabel, vehicleIcon, vehicleLabel);
		m_gigStatus.PointCameraAtPlayer();
		MissionDisplay.GetComponent<ShowHider>().Hide();
	}

	public void OnStuntStarted()
	{
		levelIntro.EndIntro();
		GetComponent<NavigateFromTo>().ManualTrigger();
	}

	private void StartIntro()
	{
		SetupIntroState(false);
		levelIntro.StartIntro();
		if (ButtonControlScheme.Instance != null)
		{
			ButtonControlScheme.Instance.Reset();
		}
	}

	private void OnClick()
	{
		if (levelIntro.IsPlaying)
		{
			levelIntro.EndIntro();
		}
		else
		{
			levelIntro.StartIntro();
		}
	}

	public void InterruptedWithGarage(bool isGarage)
	{
		NGUITools.FindInParents<GigUI>(base.gameObject).ShowHeader(false);
		if (levelIntro.IsPlaying)
		{
			levelIntro.EndIntro();
		}
		ControlButtons.Hide();
		MissionDisplay.GetComponent<ShowHider>().Hide();
		if (m_gigStatus != null && m_gigStatus.player != null)
		{
			m_gigStatus.player.ControlsEnabled = false;
		}
	}
}
