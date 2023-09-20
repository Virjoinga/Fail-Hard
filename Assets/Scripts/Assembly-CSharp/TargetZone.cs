using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

[PBSerialize("TargetZone")]
public class TargetZone : MonoBehaviour
{
	public enum TargetType
	{
		Vehicle = 0,
		Player = 1,
		Other = 2
	}

	public enum TargetState
	{
		Completed = 0,
		Uncompleted = 1,
		TooDifficult = 2
	}

	[Serializable]
	public class GameEventHelper
	{
		public GameEvent.GameEventType EventType;

		public string Data;
	}

	public delegate void ZoneTriggered(LevelTargetInfo info, bool wasCompleted);

	public TargetState targetState;

	public TargetType targetType;

	public GameObject spawnOnTrigger;

	public bool requiresStay;

	[PBSerializeField]
	public string zoneId;

	[PBSerializeField]
	public int Difficulty;

	[PBSerializeField]
	public Transform PlayTimeParent;

	public int Bonus;

	public List<GameEventHelper> Preconditions;

	private GameObject diamond;

	public GameObject diamondUncompleted;

	public GameObject diamondCompleted;

	public GameObject diamondLocked;

	public LevelTargetInfo targetInfo;

	private bool m_collectedDuringThisGig;

	private AudioClip collectedSound;

	[method: MethodImpl(32)]
	public event ZoneTriggered TriggeredEvent;

	private void Start()
	{
		if ((bool)PlayTimeParent)
		{
			base.transform.parent = PlayTimeParent;
		}
		collectedSound = (AudioClip)Resources.Load("Audio/spessu1");
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		currentLevel.RegisterTargetZone(this);
	}

	public void Init(LevelTargetInfo info)
	{
		targetInfo = info;
		if (targetInfo == null)
		{
			Debug.LogError("targetInfo == null in Init(). Running in editor?");
			targetInfo = new LevelTargetInfo();
			targetInfo.ZoneId = zoneId;
			targetInfo.Difficulty = Difficulty;
			targetInfo.Bonus = Bonus;
		}
		RefreshState();
	}

	public List<GameEvent> GetPreconditions()
	{
		List<GameEvent> list = new List<GameEvent>();
		foreach (GameEventHelper precondition in Preconditions)
		{
			GameEvent item = new GameEvent(precondition.EventType, precondition.Data);
			list.Add(item);
		}
		return list;
	}

	public void OnTriggerEnter(Collider hit)
	{
		if (!m_collectedDuringThisGig && CheckColliderTypeMatch(hit))
		{
			OnTriggered();
		}
	}

	public void RefreshState()
	{
		if (!m_collectedDuringThisGig)
		{
			targetState = TargetState.Completed;
			if ((bool)diamond)
			{
				UnityEngine.Object.Destroy(diamond);
			}
			if (targetState == TargetState.Completed)
			{
				diamond = UnityEngine.Object.Instantiate(diamondCompleted, base.transform.position, base.transform.rotation) as GameObject;
				diamond.transform.parent = base.transform;
			}
			else if (targetState == TargetState.TooDifficult)
			{
				diamond = UnityEngine.Object.Instantiate(diamondLocked, base.transform.position, base.transform.rotation) as GameObject;
				diamond.transform.parent = base.transform;
			}
			else if (targetState == TargetState.Uncompleted)
			{
				diamond = UnityEngine.Object.Instantiate(diamondUncompleted, base.transform.position, base.transform.rotation) as GameObject;
				diamond.transform.parent = base.transform;
			}
		}
	}

	private bool CheckColliderTypeMatch(Collider hit)
	{
		if (targetState != TargetState.Uncompleted && (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player"))
		{
			return true;
		}
		switch (targetType)
		{
		case TargetType.Vehicle:
			if (hit.gameObject.tag == "Vehicle")
			{
				return true;
			}
			break;
		case TargetType.Player:
			if (hit.gameObject.tag == "Player")
			{
				return true;
			}
			break;
		default:
			if (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player")
			{
				return true;
			}
			break;
		}
		return false;
	}

	public bool IsActive()
	{
		if (m_collectedDuringThisGig || targetState == TargetState.TooDifficult)
		{
			return false;
		}
		return true;
	}

	private void OnTriggered()
	{
		if (this.TriggeredEvent != null)
		{
			this.TriggeredEvent(targetInfo, targetState == TargetState.Uncompleted);
		}
		if ((bool)spawnOnTrigger)
		{
			PoolManager.Pools["Main"].Spawn(spawnOnTrigger.transform, base.transform.position, Quaternion.identity);
		}
		AudioManager.Instance.Play(collectedSound, AudioTag.DiamondAudio);
		if (diamond != null)
		{
			UnityEngine.Object.Destroy(diamond);
		}
		m_collectedDuringThisGig = true;
		RefreshState();
	}
}
