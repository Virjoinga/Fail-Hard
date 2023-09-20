using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class GigStatus : MonoBehaviour
{
	public enum GigState
	{
		Uninitialized = 0,
		InGarage = 1,
		GigReadyToStart = 2,
		StuntReadyToStart = 3,
		StuntActive = 4,
		StuntOver = 5,
		GigOver = 6
	}

	public delegate void OnBonusCollected(string text, int points, Vector3 worldPos, Vector3 uiOffset);

	public delegate void OnStateChanged(GigState newState);

	public delegate void OnPrepareForStuntOver();

	public delegate void OnRemainingStuntsChanged(int remaining, bool rewardDelta);

	public delegate void OnGigStarsCollected(int collected, int deltaToNextLimit);

	private static Texture2D screenShotTexture2D;

	private GigState m_gigState;

	private List<Stunt> stunts;

	private Stunt currentStunt;

	public GameObject playerPrefab;

	public GameObject vehiclePrefab;

	private GameObject playerRoot;

	private GameObject vehicleRoot;

	public Player player;

	private PlayerStatus playerStatus;

	private AudienceSounds audience;

	private Level m_currentLevel;

	private Transform playerSpawnPoint;

	private GigUI gigUI;

	public GigConfigPreview GigConfig;

	public GameObject GigConfigPrefab;

	public bool StageUnlockAchieved;

	private int m_activeStayTargetsCount;

	private bool m_playerNotMoving;

	private List<int> m_levelTargetLimits;

	private int m_remainingStunts;

	public GigState CurrentGigState
	{
		get
		{
			return m_gigState;
		}
		private set
		{
			if (value != m_gigState)
			{
				m_gigState = value;
				if (this.StateChanged != null)
				{
					this.StateChanged(m_gigState);
				}
			}
		}
	}

	public GigStatistics GigStatistics { get; private set; }

	public bool IsGigStarted { get; private set; }

	public List<GameEvent> EventsDuringGig { get; private set; }

	public int MaxStunts { get; private set; }

	public int RemainingStunts
	{
		get
		{
			return m_remainingStunts;
		}
	}

	[method: MethodImpl(32)]
	public event OnBonusCollected BonusCollected;

	[method: MethodImpl(32)]
	public event OnStateChanged StateChanged;

	[method: MethodImpl(32)]
	public event OnPrepareForStuntOver PrepareForStuntOver;

	[method: MethodImpl(32)]
	public event OnRemainingStuntsChanged RemainingStuntsChanged;

	[method: MethodImpl(32)]
	public event OnGigStarsCollected GigStarsChanged;

	private void SetRemainingStunts(int newVal, bool reward)
	{
		if (m_remainingStunts != newVal)
		{
			m_remainingStunts = newVal;
			if (this.RemainingStuntsChanged != null)
			{
				this.RemainingStuntsChanged(m_remainingStunts, reward);
			}
		}
	}

	public void Start()
	{
		GameController instance = GameController.Instance;
		m_currentLevel = instance.CurrentLevel;
		m_currentLevel.GigController = this;
		GigStatistics = new GigStatistics();
		string path = "Levels/" + m_currentLevel.Parameters.Name + "_snapshot";
		screenShotTexture2D = (Texture2D)Resources.Load(path);
		Init();
	}

	public void SetupScreenShotTexture(ref UITexture texture)
	{
		if (screenShotTexture2D != null)
		{
			texture.mainTexture = screenShotTexture2D;
		}
		else
		{
			Debug.LogWarning("No screenshot texture for replay!");
		}
	}

	private void m_currentLevel_TargetAchievedForGigController(LevelTargetInfo tgtInfo)
	{
		if (currentStunt != null)
		{
			currentStunt.TargetsAchieved++;
			CoinsForTarget(m_currentLevel.TargetPosition(tgtInfo));
			if (GigStatistics.StuntCount == 1 && StarsDuringGig() == 3)
			{
				int coinsForHoleInOne = GameController.Instance.Scoring.CoinsForHoleInOne;
				GigStatistics.HoleInOneBonus += coinsForHoleInOne;
				EarnedCoins(coinsForHoleInOne);
			}
		}
	}

	private void CoinsForTarget(Vector3 pos, bool isStayTarget = false)
	{
		int targetsAchieved = currentStunt.TargetsAchieved;
		int num = GameController.Instance.Scoring.ComboBonus(targetsAchieved);
		if (num > 0)
		{
			GigStatistics.TotalComboBonus += num;
			if (this.BonusCollected != null)
			{
				this.BonusCollected("COMBO", num, pos, 60f * Vector3.up);
			}
		}
		int num2 = ((!isStayTarget) ? ((int)((float)GameController.Instance.Scoring.CoinsForLevelTarget * GigStatistics.Multiplier)) : ((int)((float)GameController.Instance.Scoring.CoinsForStayTarget * GigStatistics.Multiplier)));
		GigStatistics.MultiTargetBonus += num2 - GameController.Instance.Scoring.CoinsForLevelTarget;
		EarnedCoins(num2);
		if (this.BonusCollected != null)
		{
			this.BonusCollected("COINS", num2, pos, Vector3.zero);
		}
		GigStatistics.TotalComboBonus += num;
		EarnedCoins(num);
		GigStatistics.CollectedTarget();
	}

	public void Init()
	{
		stunts = new List<Stunt>();
		CurrentGigState = GigState.Uninitialized;
		GameController instance = GameController.Instance;
		Tracking.DesignEvent(Tracking.Events.EnteringLevel, instance.CurrentLevel.Parameters.Name, instance.Character.Coins);
		playerPrefab = (GameObject)Resources.Load(instance.Character.CharacterResource);
		if (!playerPrefab)
		{
			Debug.LogError("Could not load player object!");
		}
		EventsDuringGig = new List<GameEvent>();
		instance.Character.GameEventRegistered += Character_GameEventRegistered;
		m_currentLevel.TargetAchievedForGigController += m_currentLevel_TargetAchievedForGigController;
		m_currentLevel.StayTargetTriggered += m_currentLevel_StayTargetTriggered;
		GameObject gameObject = GameObject.Find("SpawnPoint");
		if (gameObject == null)
		{
			Debug.LogError("No spawn point in scene!");
		}
		else
		{
			playerSpawnPoint = gameObject.transform;
		}
		gameObject = GameObject.Find("GigUI");
		if (gameObject == null)
		{
			Debug.LogError("No GigUI in scene!");
			return;
		}
		gigUI = gameObject.GetComponent<GigUI>();
		gameObject = GameObject.Find("Audience");
		if (gameObject == null)
		{
			Debug.LogError("No Audience in scene!");
			return;
		}
		audience = gameObject.GetComponent<AudienceSounds>();
		GameController.Instance.Agent.StageCompleted += StageCompleted;
		InitStarLimits();
		GigStatistics.StarsAtGigStart = m_currentLevel.TargetsAchieved;
		MaxStunts = m_currentLevel.CompletionCriteria + 2;
		SetRemainingStunts(MaxStunts, false);
		GameObject gameObject2 = (GameObject)Resources.Load("LevelObjects/StartIndicator");
		if (gameObject2 != null)
		{
			Object.Instantiate(gameObject2, playerSpawnPoint.position, playerSpawnPoint.rotation);
		}
		SpawnGarage();
	}

	private void Character_GameEventRegistered(GameEvent gameEvent)
	{
		EventsDuringGig.Add(gameEvent);
	}

	public List<GameEvent> ConsumeEvents()
	{
		List<GameEvent> eventsDuringGig = EventsDuringGig;
		EventsDuringGig = new List<GameEvent>();
		return eventsDuringGig;
	}

	public List<GameEvent> ConsumeEvents(GameEvent.GameEventType eventType)
	{
		List<GameEvent> list = new List<GameEvent>();
		foreach (GameEvent item in EventsDuringGig)
		{
			if (item.EventType == eventType)
			{
				list.Add(item);
				EventsDuringGig.Remove(item);
			}
		}
		return list;
	}

	public GameEvent ConsumeEvent(GameEvent.GameEventType eventType)
	{
		foreach (GameEvent item in EventsDuringGig)
		{
			if (item.EventType == eventType)
			{
				EventsDuringGig.Remove(item);
				return item;
			}
		}
		return null;
	}

	public GameEvent PeekEvent(GameEvent.GameEventType eventType)
	{
		foreach (GameEvent item in EventsDuringGig)
		{
			if (item.EventType == eventType)
			{
				return item;
			}
		}
		return null;
	}

	private void m_currentLevel_StayTargetTriggered(StayTarget target)
	{
		if (currentStunt == null)
		{
			return;
		}
		if (target.StayState == StayTarget.StayTargetState.Active)
		{
			m_activeStayTargetsCount++;
		}
		else if (target.StayState == StayTarget.StayTargetState.Inactive)
		{
			m_activeStayTargetsCount--;
		}
		else if (target.StayState == StayTarget.StayTargetState.Collected)
		{
			m_activeStayTargetsCount--;
			currentStunt.TargetsAchieved++;
			currentStunt.AchievedStayTargetCount++;
			CoinsForTarget(target.transform.position, true);
		}
		if (m_playerNotMoving && m_activeStayTargetsCount == 0)
		{
			gigUI.StuntOver();
			SoundController componentInChildren = vehicleRoot.GetComponentInChildren<SoundController>();
			if (componentInChildren != null)
			{
				componentInChildren.StuntActive = false;
			}
			playerRoot.GetComponentInChildren<PlayerSounds>().StuntActive = false;
		}
	}

	private void OnDestroy()
	{
		GameController.Instance.Agent.StageCompleted -= StageCompleted;
		m_currentLevel.StayTargetTriggered -= m_currentLevel_StayTargetTriggered;
		m_currentLevel.TargetAchievedForGigController -= m_currentLevel_TargetAchievedForGigController;
		GameController.Instance.Character.GameEventRegistered -= Character_GameEventRegistered;
	}

	public void StartGig()
	{
		IsGigStarted = true;
		CurrentGigState = GigState.GigReadyToStart;
	}

	public void EndGig()
	{
		EndStunt();
		AudioManager.Instance.Cleanup();
	}

	public int TargetCount()
	{
		return m_currentLevel.TargetCount();
	}

	public void RestartStunt()
	{
		if (CurrentGigState != GigState.StuntReadyToStart && CurrentGigState != GigState.InGarage)
		{
			Debug.LogWarning("Restart can be done only in ready to start or InGarage states");
			return;
		}
		currentStunt = null;
		StartStunt();
	}

	public void StartStunt()
	{
		if (CurrentGigState != GigState.GigOver)
		{
			GestureRecognizer.enabled = false;
			if ((bool)playerRoot)
			{
				Object.Destroy(vehicleRoot);
				Object.Destroy(playerRoot);
				m_currentLevel.Cameraman.StartTracking(null);
			}
			vehiclePrefab = (GameObject)Resources.Load(GameController.Instance.Character.CurrentVehicle.ResourceName);
			vehicleRoot = Object.Instantiate(vehiclePrefab, playerSpawnPoint.position, playerSpawnPoint.rotation) as GameObject;
			playerRoot = Object.Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation) as GameObject;
			player = playerRoot.GetComponentInChildren<Player>();
			if ((bool)audience)
			{
				audience.GetComponent<AudienceSounds>().reset();
			}
			m_playerNotMoving = false;
			m_activeStayTargetsCount = 0;
			Invoke("DelayedStartStunt", 0.1f);
		}
	}

	private void DelayedStartStunt()
	{
		VehicleBase componentInChildren = vehicleRoot.GetComponentInChildren<VehicleBase>();
		player.AssignVehicle(componentInChildren);
		currentStunt = player.StartStunt();
		SoundController componentInChildren2 = vehicleRoot.GetComponentInChildren<SoundController>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.StuntActive = true;
		}
		playerRoot.GetComponentInChildren<PlayerSounds>().StuntActive = true;
		foreach (VehiclePart equippedGadget in GameController.Instance.Character.GetEquippedGadgets())
		{
			player.EquipGadget(equippedGadget);
		}
		foreach (VehiclePart equippedGadget2 in GameController.Instance.Character.CurrentVehicle.GetEquippedGadgets())
		{
			componentInChildren.EquipGadget(equippedGadget2);
		}
		componentInChildren.vehicleData.CurrentCondition = RemainingStunts;
		CurrentGigState = GigState.StuntReadyToStart;
	}

	public void SkipStunt()
	{
		player.SkipStunt();
		player.currentVehicle.vehicleData.CurrentCondition--;
		SetRemainingStunts(RemainingStunts - 1, false);
	}

	public void CancelStunt()
	{
		player.SkipStunt();
	}

	public void EndStunt()
	{
		if (currentStunt == null || CurrentGigState == GigState.GigOver || CurrentGigState == GigState.StuntReadyToStart)
		{
			return;
		}
		Stunt stunt = player.EndStunt();
		stunts.Add(stunt);
		if (stunt.TargetsAchieved == 0)
		{
			GigStatistics.ResetMultiplier();
		}
		stunt.Coins = GameController.Instance.Character.Coins - stunt.MoneyAtStart;
		stunt.Score += stunt.Coins;
		if (stunt.AchievedStayTargetCount > 1)
		{
			stunt.Score *= stunt.AchievedStayTargetCount;
		}
		if ((bool)audience)
		{
			audience.GetComponent<AudienceSounds>().reset();
		}
		currentStunt = null;
		CurrentGigState = GigState.StuntOver;
		if (RemainingStunts <= 0 || m_currentLevel.RemainingCoins() == 0)
		{
			CurrentGigState = GigState.GigOver;
			if (RemainingStunts > 0)
			{
				RemainingStuntsReward(RemainingStunts);
				SetRemainingStunts(0, true);
			}
		}
		int totalGigCoins = GigStatistics.TotalGigCoins;
		if (totalGigCoins > m_currentLevel.HighScore)
		{
			m_currentLevel.HighScore = totalGigCoins;
		}
		LocalStorage.Instance.SyncDirtyToDisk();
	}

	public int GetStuntCount()
	{
		return stunts.Count;
	}

	public Stunt FirstStunt()
	{
		if (stunts.Count > 0)
		{
			return stunts[0];
		}
		return new Stunt();
	}

	public Stunt LastStunt()
	{
		if (stunts.Count > 0)
		{
			return stunts[stunts.Count - 1];
		}
		return new Stunt();
	}

	public Stunt CurrentStunt()
	{
		return currentStunt;
	}

	public int TotalTargetsAchieved()
	{
		if (m_currentLevel.IsCompleted)
		{
			return m_currentLevel.TargetsAchieved;
		}
		return TotalTargetsDuringGig();
	}

	public int TotalTargetsDuringGig()
	{
		int num = 0;
		if (stunts == null)
		{
			return 0;
		}
		foreach (Stunt stunt in stunts)
		{
			num += stunt.NewTargetsAchieved.Count;
		}
		if (currentStunt != null)
		{
			num += currentStunt.NewTargetsAchieved.Count;
		}
		return num;
	}

	public int TotalGigScore()
	{
		int num = 0;
		foreach (Stunt stunt in stunts)
		{
			num += stunt.Score;
		}
		return num;
	}

	public int TotalGigCoins()
	{
		return GigStatistics.TotalGigCoins;
	}

	public int BestStuntScore()
	{
		int num = 0;
		foreach (Stunt stunt in stunts)
		{
			if (stunt.Score > num)
			{
				num = stunt.Score;
			}
		}
		return num;
	}

	public void PlayerMoving()
	{
		IsGigStarted = true;
		gigUI.StuntStarted();
		GigStatistics.StuntCount++;
		CurrentGigState = GigState.StuntActive;
		PointCameraAtPlayer();
	}

	public void PlayerCrashed()
	{
		if (this.PrepareForStuntOver != null && m_activeStayTargetsCount <= 0)
		{
			this.PrepareForStuntOver();
		}
	}

	public void PlayerNotMoving()
	{
		if (m_activeStayTargetsCount > 0)
		{
			m_playerNotMoving = true;
			return;
		}
		gigUI.StuntOver();
		SoundController componentInChildren = vehicleRoot.GetComponentInChildren<SoundController>();
		if (componentInChildren != null)
		{
			componentInChildren.StuntActive = false;
		}
		playerRoot.GetComponentInChildren<PlayerSounds>().StuntActive = false;
	}

	public void PointCameraAtPlayer()
	{
		if (player != null)
		{
			m_currentLevel.Cameraman.StartTracking(player.gameObject.transform);
			if (player.currentVehicle != null)
			{
				m_currentLevel.Cameraman.AddLookAtTarget(player.currentVehicle.transform);
			}
		}
	}

	public void PointCameraAtSpawnPoint()
	{
		m_currentLevel.Cameraman.StartTracking(playerSpawnPoint);
	}

	public void PointCameraAtGarage()
	{
		CurrentGigState = GigState.InGarage;
		m_currentLevel.Cameraman.ConfigWithCamera(GigConfig.ConfigCamera, GigConfig.transform);
	}

	public Transform GetSpawnPoint()
	{
		return playerSpawnPoint;
	}

	public void BigAirStateChange(bool isBigAir)
	{
		if (isBigAir)
		{
			int num = (int)((float)GameController.Instance.Scoring.CoinsForBigAir * GigStatistics.Multiplier);
			GigStatistics.BigAirBonus += num;
			EarnedCoins(num);
			if (this.BonusCollected != null)
			{
				this.BonusCollected("BIG AIR", num, player.transform.position, Vector3.zero);
			}
		}
	}

	public void BigAirFrame()
	{
		if (currentStunt != null)
		{
			currentStunt.BigAirTime++;
		}
	}

	public void Flip()
	{
		if (currentStunt != null)
		{
			currentStunt.FlipCount++;
			int num = (int)((float)GameController.Instance.Scoring.CoinsForFlip(currentStunt.FlipCount) * GigStatistics.Multiplier);
			GigStatistics.FlipBonus += num;
			EarnedCoins(num);
			if (this.BonusCollected != null)
			{
				this.BonusCollected("FLIP", num, player.transform.position, Vector3.zero);
			}
		}
	}

	public void RemainingStuntsReward(int stunts)
	{
		int num = (int)((float)GameController.Instance.Scoring.CoinsForRemainingStunts(stunts) * GigStatistics.Multiplier);
		EarnedCoins(num);
		if (this.BonusCollected != null)
		{
			this.BonusCollected("STUNTS", num, player.transform.position, Vector3.zero);
		}
	}

	public void CollateralDamage(float normalizedAmount, Vector3 worldPos)
	{
		if (currentStunt != null)
		{
			int num = (int)((float)GameController.Instance.Scoring.CoinsForCollateralDamage(normalizedAmount) * GigStatistics.Multiplier);
			GigStatistics.CollateralDamageBonus += num;
			EarnedCoins(num);
			if (this.BonusCollected != null)
			{
				this.BonusCollected("CRASH", num, worldPos, Vector3.zero);
			}
		}
	}

	private void EarnedCoins(int coins)
	{
		GigStatistics.TotalGigCoins += coins;
		GameController.Instance.Character.Coins += coins;
		int stars = GetStars(GigStatistics.TotalGigCoins);
		while (GigStatistics.StarsDuringGig < stars)
		{
			GigStatistics.StarsDuringGig++;
			m_currentLevel.SaveStar("hackStar" + GigStatistics.StarsDuringGig);
			GameController instance = GameController.Instance;
			int num = instance.Scoring.CareerLevelForStars(instance.Character.CareerStars);
			int num2 = instance.Scoring.LevelUpCriteria(num + 1);
			instance.Character.CareerStars++;
			if (instance.Character.CareerStars >= num2)
			{
				GameController.Instance.Character.AddGameEvent(new GameEvent(GameEvent.GameEventType.CareerLevelUp, (num + 1).ToString()));
				instance.Character.Coins += instance.Scoring.CoinsForLevelUp(num + 1);
			}
			if (this.GigStarsChanged != null)
			{
				if (GigStatistics.StarsDuringGig == 3)
				{
					this.GigStarsChanged(GigStatistics.StarsDuringGig, 0);
				}
				else
				{
					this.GigStarsChanged(GigStatistics.StarsDuringGig, GetStarLimit(GigStatistics.StarsDuringGig));
				}
			}
		}
	}

	public int StarsDuringGig()
	{
		return GigStatistics.StarsDuringGig;
	}

	public int StarsAtGigStart()
	{
		return GigStatistics.StarsAtGigStart;
	}

	private void InitStarLimits()
	{
		if (GameController.Instance.CurrentBundle == null)
		{
			Debug.LogWarning("No current bundle. Playing in editor?");
			m_levelTargetLimits = new List<int>();
			m_levelTargetLimits.Add(1000);
			m_levelTargetLimits.Add(2000);
			m_levelTargetLimits.Add(3000);
		}
		else
		{
			m_levelTargetLimits = GameController.Instance.CurrentBundle.StarCriteria(m_currentLevel.Parameters.Name);
		}
	}

	public int GetCoinsToNextStarLimit()
	{
		if (GigStatistics.StarsDuringGig == 3)
		{
			return 0;
		}
		return GetStarLimit(GigStatistics.StarsDuringGig) - GigStatistics.TotalGigCoins;
	}

	public int GetStarLimit(int starIndex)
	{
		if (m_levelTargetLimits == null)
		{
			InitStarLimits();
		}
		return m_levelTargetLimits[starIndex];
	}

	public int GetStars(int coins)
	{
		for (int i = 0; i < 3; i++)
		{
			if (coins < m_levelTargetLimits[i])
			{
				return i;
			}
		}
		return 3;
	}

	private void SpawnGarage()
	{
		GameObject gameObject = GameObject.Find("GarageSpawnPoint");
		if (gameObject == null)
		{
			GameObject gameObject2 = Object.Instantiate(GigConfigPrefab, 1000f * Vector3.up, Quaternion.identity) as GameObject;
			GigConfig = gameObject2.GetComponent<GigConfigPreview>();
			GigConfig.ShowBackground(true);
		}
		else
		{
			GameObject gameObject3 = Object.Instantiate(GigConfigPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
			GigConfig = gameObject3.GetComponent<GigConfigPreview>();
			GigConfig.ShowBackground(false);
		}
		GigConfig.ShowPlayerAndVehicle(true, GameController.Instance.Character.CurrentVehicle);
	}

	private void StageCompleted(int stage)
	{
		StageUnlockAchieved = true;
	}
}
