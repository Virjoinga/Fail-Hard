using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using Game.Util;
using UnityEngine;

public class Player : MonoBehaviour
{
	public enum AnimationType
	{
		Idle = 0,
		Drive = 1,
		Jump = 2,
		Freefall = 3,
		SpinForward = 4,
		SpinBack = 5,
		CrashOnHead = 6,
		CrashOnFeet = 7,
		Count = 8
	}

	public delegate void OnAnimationStatesModified(AnimationType animType, bool added);

	public delegate void OnPlayerJump();

	public delegate void OnPlayerJumpOff();

	public delegate void OnPlayerCrashed();

	public Dictionary<AnimationType, string> AnimationStates;

	public Dictionary<AnimationType, string> DefaultAnimationStates;

	public PlayerAnimationController AnimationController;

	public PlayerState currentState;

	public VehicleBase currentVehicle;

	public ConfigurableJoint vehicleConnector;

	public Dictionary<VehiclePartType, Gadget> CurrentGadgets;

	public GameObject gadgetPrefab;

	public Transform GadgetSlotPosBack;

	public Transform GadgetSlotPosHead;

	public GameObject DefaultHead;

	public Transform handL;

	public Transform handR;

	public Transform footL;

	public Transform footR;

	public Pose pose;

	public Vector3 rootPosition;

	public GigStatus gigController;

	private bool m_bigAirState;

	private float m_bigAirFrames;

	private Stunt currentStunt;

	private GameObject controlGizmo;

	public bool ControlsEnabled;

	private bool keyboardControls;

	private bool m_invertedControls;

	private PlayerStatus playerStatus;

	public float BigAirFrames
	{
		get
		{
			return m_bigAirFrames;
		}
		set
		{
			m_bigAirFrames = value;
			if (m_bigAirFrames == 0f && m_bigAirState)
			{
				m_bigAirState = false;
				gigController.BigAirStateChange(false);
			}
			else if (!m_bigAirState && m_bigAirFrames > (float)GameController.Instance.Scoring.BigAirFramesThreshold)
			{
				m_bigAirState = true;
				gigController.BigAirStateChange(true);
			}
			else if (m_bigAirFrames > (float)GameController.Instance.Scoring.BigAirFramesThreshold)
			{
				gigController.BigAirFrame();
			}
		}
	}

	public bool InvertedControls
	{
		get
		{
			return m_invertedControls;
		}
	}

	public PlayerStatus Status
	{
		get
		{
			if (playerStatus == null)
			{
				playerStatus = GetComponent<PlayerStatus>();
			}
			return playerStatus;
		}
		private set
		{
		}
	}

	[method: MethodImpl(32)]
	public event OnAnimationStatesModified AnimationStatesModified;

	[method: MethodImpl(32)]
	public event OnPlayerJump PlayerJumped;

	[method: MethodImpl(32)]
	public event OnPlayerJumpOff PlayerJumpedOff;

	[method: MethodImpl(32)]
	public event OnPlayerCrashed PlayerCrashed;

	[method: MethodImpl(32)]
	public static event TutorialHookHandler TutorialHook;

	private void Start()
	{
		currentState = base.gameObject.AddComponent<PlayerStateTpose>();
		base.gameObject.AddComponent<PlayerStateInHCRVehicle>();
		base.gameObject.AddComponent<PlayerStateMidAir>();
		base.gameObject.AddComponent<PlayerStateIdle>();
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		if (currentLevel != null)
		{
			currentLevel.SetPlayerTransform(base.gameObject.transform);
		}
		GameObject gameObject = GameObject.Find("Level:root");
		if (!gameObject)
		{
			Logger.Error("No level root in scene!");
			return;
		}
		gigController = gameObject.GetComponentInChildren<GigStatus>();
		if (!gigController)
		{
			Logger.Error("No gig controller in Scene!");
			return;
		}
		Character character = instance.Character;
		character.Player = this;
		DefaultAnimationStates = new Dictionary<AnimationType, string>();
		DefaultAnimationStates.Add(AnimationType.Freefall, "freefall");
		DefaultAnimationStates.Add(AnimationType.CrashOnHead, "Animation_crashonhead");
		DefaultAnimationStates.Add(AnimationType.CrashOnFeet, "Animation_crashonfeet");
		AnimationStates = new Dictionary<AnimationType, string>();
		ControlsEnabled = false;
		m_invertedControls = PlayerPrefs.GetInt("InvertControls", 0) > 0;
		CurrentGadgets = new Dictionary<VehiclePartType, Gadget>();
	}

	public void SetAnimation(AnimationType animType, AnimationClip clip)
	{
		if (AnimationController.animationRoot.GetClip(clip.name) == null)
		{
			AnimationController.animationRoot.AddClip(clip, clip.name);
		}
		BindAnimationTypeToState(animType, clip.name);
	}

	public void BindAnimationTypeToState(AnimationType animType, string animState)
	{
		if (AnimationController.animationRoot.GetClip(animState) == null)
		{
			Debug.LogError("Animtion clip missing. Cannot bind type to state " + animState);
		}
		AnimationStates.Remove(animType);
		AnimationStates.Add(animType, animState);
		if (this.AnimationStatesModified != null)
		{
			this.AnimationStatesModified(animType, true);
		}
	}

	public void JumpOffVehicle(bool releaseOnly = false)
	{
		if (releaseOnly)
		{
			SetPhysicsConstraints(RigidbodyConstraints.None);
		}
		currentState.JumpOffVehicle(releaseOnly);
	}

	public void SetPhysicsConstraints(RigidbodyConstraints constraints)
	{
		base.rigidbody.constraints = constraints;
		Status.GetBodyPart(BodyPartType.Head).rigidbody.constraints = constraints;
	}

	public void RemoveAnimationTypeFromState(AnimationType animType)
	{
		AnimationStates.Remove(animType);
		if (this.AnimationStatesModified != null)
		{
			this.AnimationStatesModified(animType, false);
		}
	}

	private void OnBecameVisible()
	{
		if (Player.TutorialHook == null)
		{
			GestureRecognizer.enabled = true;
		}
		else
		{
			Invoke("StartTutorial", 2f);
		}
	}

	private void StartTutorial()
	{
		if (!keyboardControls)
		{
			TutorialStep(TutorialEvent.Begin);
		}
	}

	private void TutorialStep(TutorialEvent tutorialEvent)
	{
		if (Player.TutorialHook != null)
		{
			Player.TutorialHook(tutorialEvent);
		}
	}

	public void SetTargetThrottle(float relativeThrottle)
	{
		currentVehicle.SetTargetThrottle(relativeThrottle);
	}

	public void AssignVehicle(VehicleBase newVehicle)
	{
		currentVehicle = newVehicle;
		currentState.AssignVehicle(newVehicle);
		GameController instance = GameController.Instance;
		Character character = instance.Character;
		character.Vehicle = newVehicle;
		vehicleConnector.connectedBody = currentVehicle.gameObject.rigidbody;
	}

	public bool EquipGadget(VehiclePart gadget)
	{
		if (CurrentGadgets.ContainsKey(gadget.ItemType))
		{
			if (CurrentGadgets[gadget.ItemType].VehiclePart == gadget)
			{
				Object.Destroy(CurrentGadgets[gadget.ItemType].gameObject);
				CurrentGadgets.Remove(gadget.ItemType);
				DefaultHead.SetActive(true);
				return false;
			}
			Object.Destroy(CurrentGadgets[gadget.ItemType].gameObject);
			CurrentGadgets.Remove(gadget.ItemType);
		}
		GameObject gameObject = (GameObject)Resources.Load(gadget.GadgetPrefabResource);
		if (gameObject == null)
		{
			Debug.LogWarning("Gadget resource load failed: " + gadget.Id);
			return false;
		}
		GameObject gameObject2 = null;
		if (gadget.ItemType == VehiclePartType.PlayerGadgetBack)
		{
			gameObject2 = Object.Instantiate(gameObject, GadgetSlotPosBack.position, GadgetSlotPosBack.rotation) as GameObject;
			gameObject2.transform.parent = GadgetSlotPosBack.parent;
		}
		else if (gadget.ItemType == VehiclePartType.PlayerGadgetHead)
		{
			gameObject2 = Object.Instantiate(gameObject, GadgetSlotPosHead.position, GadgetSlotPosHead.rotation) as GameObject;
			gameObject2.transform.parent = GadgetSlotPosHead.parent;
			DefaultHead.SetActive(false);
		}
		if (gameObject2 == null)
		{
			Debug.LogWarning("Invalid gadget type: " + gadget.ItemType);
			return false;
		}
		CurrentGadgets.Add(gadget.ItemType, gameObject2.GetComponent<Gadget>());
		gameObject2.GetComponent<Gadget>().Equip(this, base.rigidbody, gadget);
		return true;
	}

	public Stunt StartStunt()
	{
		currentStunt = new Stunt();
		currentStunt.TotalHPLevelAtStart = Status.GetTotalHPLevel();
		return currentStunt;
	}

	public void SkipStunt()
	{
		currentState.DetectDualTap(null);
	}

	public Stunt EndStunt()
	{
		currentStunt.PlayerEndPosition = base.transform.position;
		currentStunt.VehicleEndPosition = currentVehicle.transform.position;
		currentStunt.TotalDamage = currentStunt.TotalHPLevelAtStart - Status.GetTotalHPLevel();
		currentStunt.Score += (int)BigAirFrames;
		return currentStunt;
	}

	private void Update()
	{
		if (Input.anyKey)
		{
			keyboardControls = true;
		}
		currentState.UpdateFunc();
		if (gigController.CurrentGigState == GigStatus.GigState.StuntActive && base.rigidbody.position.y < -20f)
		{
			gigController.PlayerNotMoving();
		}
	}

	private void FixedUpdate()
	{
		currentState.FixedUpdateFunc();
	}

	public void Flip()
	{
		gigController.Flip();
	}

	public float GetPlayerSpinMultiplier()
	{
		float num = 1f;
		foreach (Gadget value in CurrentGadgets.Values)
		{
			if (value.VehiclePart.ItemType == VehiclePartType.PlayerGadgetBack || value.VehiclePart.ItemType == VehiclePartType.PlayerGadgetHead)
			{
				num *= value.PlayerSpinMultiplier;
			}
		}
		return num;
	}

	public float GetVehicleSpinMultiplier()
	{
		float num = 1f;
		if (currentVehicle.CurrentGadget != null)
		{
			num *= currentVehicle.CurrentGadget.VehicleSpinMultiplier;
		}
		return num;
	}

	public void HandleAction(ButtonControlScheme.VehicleControl action)
	{
		switch (action)
		{
		case ButtonControlScheme.VehicleControl.ActionVehicle:
			if (currentVehicle.CurrentGadget != null)
			{
				currentVehicle.CurrentGadget.Action();
			}
			return;
		case ButtonControlScheme.VehicleControl.ActionTorso:
		{
			foreach (Gadget value in CurrentGadgets.Values)
			{
				if (value.VehiclePart.ItemType == VehiclePartType.PlayerGadgetBack)
				{
					value.Action();
				}
			}
			return;
		}
		}
		foreach (Gadget value2 in CurrentGadgets.Values)
		{
			if (value2.VehiclePart.ItemType == VehiclePartType.PlayerGadgetHead)
			{
				value2.Action();
			}
		}
	}

	public void SignalPlayerJumped()
	{
		foreach (Gadget value in CurrentGadgets.Values)
		{
			value.PlayerJumpWithVehicle();
		}
		if (currentVehicle.CurrentGadget != null)
		{
			currentVehicle.CurrentGadget.PlayerJumpWithVehicle();
		}
		if (this.PlayerJumped != null)
		{
			this.PlayerJumped();
		}
	}

	public void SignalPlayerJumpedOff()
	{
		foreach (Gadget value in CurrentGadgets.Values)
		{
			value.PlayerJumpOff();
		}
		if (currentVehicle.CurrentGadget != null)
		{
			currentVehicle.CurrentGadget.PlayerJumpOff();
		}
		if (this.PlayerJumpedOff != null)
		{
			this.PlayerJumpedOff();
		}
	}

	public void SignalPlayerCrashed()
	{
		if (this.PlayerCrashed != null)
		{
			this.PlayerCrashed();
		}
	}
}
