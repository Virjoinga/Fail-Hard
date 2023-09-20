using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

[PBSerialize("StayTarget")]
public class StayTarget : MonoBehaviour
{
	public enum StayTargetType
	{
		Vehicle = 0,
		Player = 1,
		Both = 2,
		DynamicLevelObject = 3,
		Any = 4
	}

	public enum StayTargetState
	{
		Inactive = 0,
		Active = 1,
		Collected = 2
	}

	public delegate void OnStayTargetTriggered(StayTarget target);

	public StayTargetType TargetType;

	private int m_playerTriggerCount;

	private int m_vehicleTriggerCount;

	public GameObject SpawnOnTrigger;

	[PBSerializeField]
	public Transform PlayTimeParent;

	public int Bonus;

	public float Duration;

	private GameObject VisibleObject;

	public bool ShowObject;

	public GameObject VisibleObjectPrefab;

	public StayTargetState StayState;

	private GigStatus m_gigStatus;

	[method: MethodImpl(32)]
	public event OnStayTargetTriggered StayTargetTriggered;

	private void Start()
	{
		if ((bool)PlayTimeParent)
		{
			base.transform.parent = PlayTimeParent;
		}
		GameController instance = GameController.Instance;
		Level currentLevel = instance.CurrentLevel;
		currentLevel.RegisterStayTarger(this);
		GameObject gameObject = GameObject.Find("Level:root");
		m_gigStatus = gameObject.GetComponentInChildren<GigStatus>();
		m_gigStatus.StateChanged += m_gigStatus_StateChanged;
	}

	private void m_gigStatus_StateChanged(GigStatus.GigState newState)
	{
		if (newState == GigStatus.GigState.StuntReadyToStart)
		{
			ResetState();
		}
	}

	private void OnDestroy()
	{
		m_gigStatus.StateChanged -= m_gigStatus_StateChanged;
	}

	private void ResetState()
	{
		CancelInvoke("StayTargetCompleted");
		m_vehicleTriggerCount = 0;
		m_playerTriggerCount = 0;
		StayState = StayTargetState.Inactive;
	}

	public void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag == "Vehicle")
		{
			m_vehicleTriggerCount++;
		}
		else if (hit.gameObject.tag == "Player")
		{
			m_playerTriggerCount++;
		}
		if (StayState == StayTargetState.Inactive && CriteriaMatch())
		{
			StayState = StayTargetState.Active;
			Invoke("StayTargetCompleted", Duration);
			if (this.StayTargetTriggered != null)
			{
				this.StayTargetTriggered(this);
			}
		}
	}

	private void StayTargetCompleted()
	{
		StayState = StayTargetState.Collected;
		if ((bool)SpawnOnTrigger)
		{
			GOTools.Spawn(SpawnOnTrigger, base.transform.position, Quaternion.identity);
		}
		if (this.StayTargetTriggered != null)
		{
			this.StayTargetTriggered(this);
		}
	}

	private bool CriteriaMatch()
	{
		if (TargetType == StayTargetType.Player)
		{
			return m_playerTriggerCount > 0;
		}
		if (TargetType == StayTargetType.Vehicle)
		{
			return m_vehicleTriggerCount > 0;
		}
		return m_playerTriggerCount > 0 && m_vehicleTriggerCount > 0;
	}

	public void OnTriggerExit(Collider hit)
	{
		if (hit.gameObject.tag == "Vehicle")
		{
			m_vehicleTriggerCount--;
		}
		else if (hit.gameObject.tag == "Player")
		{
			m_playerTriggerCount--;
		}
		if (StayState == StayTargetState.Active && !CriteriaMatch())
		{
			StayState = StayTargetState.Inactive;
			CancelInvoke("StayTargetCompleted");
			if (this.StayTargetTriggered != null)
			{
				this.StayTargetTriggered(this);
			}
		}
	}
}
