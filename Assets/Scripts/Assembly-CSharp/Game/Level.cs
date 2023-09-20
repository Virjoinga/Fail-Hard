using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Progress;
using UnityEngine;

namespace Game
{
	public class Level : IId, ILevel
	{
		private class AsyncMessageHandler : IMessageConsumer
		{
			private Level level;

			public AsyncMessageHandler(Level parent)
			{
				level = parent;
			}

			public void Consume(VMessage msg)
			{
				if (msg.LevelInfo != null)
				{
					level.Info = msg.LevelInfo;
				}
				else
				{
					LevelInfo levelInfo = new LevelInfo();
					levelInfo.IsLocked = true;
					levelInfo.IsCompleted = false;
					levelInfo.UnlockingIndex = 9999;
					level.Info = levelInfo;
				}
				level.Agent.AddLevel(level);
				LocalStorage.Instance.UpdateLevelInfo(level.Info, level.Parameters.Name);
				levelLoadCounter--;
			}
		}

		public delegate void StarAchievedHandler(string levelId);

		public delegate void TargetAchievedHandler(LevelTargetInfo tgtInfo);

		public delegate void OnStayTargetTriggered(StayTarget target);

		public static int levelLoadCounter;

		private Dictionary<string, TargetZone> m_targetZones;

		private LevelInfo Info;

		private LevelInfo InfoChanges;

		private AsyncMessageHandler m_messageHandler;

		public string Id { get; set; }

		public Dictionary<string, int> AchievedLevelTargets
		{
			get
			{
				if (Info.AchievedLevelTargets == null)
				{
					Info.AchievedLevelTargets = new Dictionary<string, int>();
				}
				return Info.AchievedLevelTargets;
			}
			private set
			{
				Info.AchievedLevelTargets = value;
			}
		}

		public int HighScore
		{
			get
			{
				return Info.HighScore;
			}
			set
			{
				Info.HighScore = value;
				InfoChanges.HighScore = value;
				UpdateInfo();
			}
		}

		public int TargetsAchieved
		{
			get
			{
				return Info.AchievedLevelTargets.Count;
			}
			private set
			{
			}
		}

		public Stage Stage
		{
			get
			{
				return Parameters.Stage;
			}
			set
			{
				Parameters.Stage = value;
			}
		}

		public ThemeCategory ThemeCategory
		{
			get
			{
				return Parameters.ThemeCategory;
			}
			private set
			{
				Parameters.ThemeCategory = value;
			}
		}

		public bool IsLocked
		{
			get
			{
				return Info.IsLocked;
			}
			private set
			{
				Info.IsLocked = value;
				InfoChanges.IsLocked = value;
				UpdateInfo();
			}
		}

		public bool IsCompleted
		{
			get
			{
				return Info.IsCompleted;
			}
			set
			{
				Info.IsCompleted = value;
				InfoChanges.IsCompleted = value;
				UpdateInfo();
			}
		}

		public int UnlockingIndex
		{
			get
			{
				return Info.UnlockingIndex;
			}
			set
			{
				Info.UnlockingIndex = value;
				InfoChanges.UnlockingIndex = value;
				UpdateInfo();
			}
		}

		public int CompletionCriteria
		{
			get
			{
				return 1;
			}
			set
			{
				Info.CompletionCriteria = value;
				InfoChanges.CompletionCriteria = value;
				UpdateInfo();
			}
		}

		public bool UnlockShown
		{
			get
			{
				return Info.UnlockShown;
			}
			set
			{
				Info.UnlockShown = value;
				InfoChanges.UnlockShown = value;
				UpdateInfo();
			}
		}

		public IEnumerable<TargetZone> LevelTargets
		{
			get
			{
				foreach (TargetZone value in m_targetZones.Values)
				{
					yield return value;
				}
			}
		}

		private Agent Agent { get; set; }

		public LevelParameters Parameters { get; private set; }

		public Transform SpawnPoint { get; private set; }

		public Transform Player { get; private set; }

		public Cameraman Cameraman { get; private set; }

		public GigStatus GigController { get; set; }

		[method: MethodImpl(32)]
		public event StarAchievedHandler NewStarAchieved;

		[method: MethodImpl(32)]
		public event TargetAchievedHandler NewTargetAchieved;

		[method: MethodImpl(32)]
		public event TargetAchievedHandler TargetAchievedForGigController;

		[method: MethodImpl(32)]
		public event LevelCompletedHandler LevelCompleted;

		[method: MethodImpl(32)]
		public event OnStayTargetTriggered StayTargetTriggered;

		public Level(LevelParameters parameters)
		{
			InfoChanges = new LevelInfo();
			levelLoadCounter++;
			m_targetZones = new Dictionary<string, TargetZone>();
			Parameters = parameters;
			m_messageHandler = new AsyncMessageHandler(this);
			Agent = GameController.Instance.Agent;
			Storage.Instance.LoadLevelInfo(Parameters.Name, m_messageHandler);
		}

		public void Unlock(int unlockingIndex)
		{
			Unlock(unlockingIndex, true);
		}

		public void Unlock(int unlockingIndex, bool animate)
		{
			Info.IsLocked = false;
			InfoChanges.IsLocked = false;
			Info.UnlockingIndex = unlockingIndex;
			InfoChanges.UnlockingIndex = unlockingIndex;
			Info.UnlockShown = !animate;
			InfoChanges.UnlockShown = !animate;
			int num = 0;
			foreach (LevelTargetInfo targetInfo in Parameters.TargetInfos)
			{
				if (GameController.Instance.Character.ContainsEvents(targetInfo.Preconditions))
				{
					num++;
				}
			}
			Info.CompletionCriteria = num;
			InfoChanges.CompletionCriteria = num;
			UpdateInfo();
		}

		public void Reset()
		{
			m_targetZones.Clear();
		}

		public void RegisterTargetZone(TargetZone zone)
		{
			LevelTargetInfo targetInfoByZoneId = Parameters.GetTargetInfoByZoneId(zone.zoneId);
			if (targetInfoByZoneId == null)
			{
				Debug.LogError("Invalid zoneId, not in level info. Ignoring...");
				return;
			}
			if (m_targetZones.ContainsKey(zone.zoneId))
			{
				Debug.LogError("Duplicate level target. Ignoring...");
				return;
			}
			m_targetZones.Add(zone.zoneId, zone);
			zone.Init(targetInfoByZoneId);
			zone.TriggeredEvent += OnTargetAchieved;
		}

		public void RegisterStayTarger(StayTarget target)
		{
			target.StayTargetTriggered += target_StayTargetTriggered;
		}

		private void target_StayTargetTriggered(StayTarget target)
		{
			if (this.StayTargetTriggered != null)
			{
				this.StayTargetTriggered(target);
			}
		}

		public Vector3 TargetPosition(LevelTargetInfo info)
		{
			foreach (TargetZone value in m_targetZones.Values)
			{
				if (value.targetInfo == info)
				{
					return value.transform.position;
				}
			}
			Debug.LogError("Invalid target info");
			return Vector3.zero;
		}

		public int TargetCount()
		{
			return 3;
		}

		public int RemainingCoins()
		{
			int num = 0;
			foreach (TargetZone value in m_targetZones.Values)
			{
				if (value.IsActive())
				{
					num++;
				}
			}
			return num;
		}

		public int TargetCount(int maxDifficulty)
		{
			int num = 0;
			foreach (LevelTargetInfo targetInfo in Parameters.TargetInfos)
			{
				if (targetInfo.Difficulty <= maxDifficulty)
				{
					num++;
				}
			}
			return num;
		}

		public int TargetCountForConditionListOr(List<GameEvent> conditions, bool includeEmpty)
		{
			int num = 0;
			foreach (LevelTargetInfo targetInfo in Parameters.TargetInfos)
			{
				if (conditions.Count == 0 && targetInfo.Preconditions.Count == 0)
				{
					num++;
					continue;
				}
				foreach (GameEvent condition in conditions)
				{
					if (includeEmpty && targetInfo.Preconditions.Count == 0)
					{
						num++;
						break;
					}
					if (targetInfo.Preconditions.Contains(condition))
					{
						num++;
						break;
					}
				}
			}
			return num;
		}

		public int TargetCountForCurrentCareerState()
		{
			int num = 0;
			foreach (LevelTargetInfo targetInfo in Parameters.TargetInfos)
			{
				if (GameController.Instance.Character.ContainsEvents(targetInfo.Preconditions))
				{
					num++;
				}
			}
			return num;
		}

		public void SetSpawnPoint(Transform spawnPoint)
		{
			SpawnPoint = spawnPoint;
		}

		public void SetCamera(Cameraman camera)
		{
			Cameraman = camera;
		}

		public void SetPlayerTransform(Transform player)
		{
			Player = player;
		}

		public void SaveStar(string hackId)
		{
			if (AchievedLevelTargets.ContainsKey(hackId))
			{
				Dictionary<string, int> achievedLevelTargets;
				Dictionary<string, int> dictionary = (achievedLevelTargets = AchievedLevelTargets);
				string key;
				string key2 = (key = hackId);
				int num = achievedLevelTargets[key];
				dictionary[key2] = num + 1;
				return;
			}
			AchievedLevelTargets[hackId] = 1;
			if (this.NewStarAchieved != null)
			{
				this.NewStarAchieved(Parameters.Name);
			}
			if (!IsCompleted)
			{
				IsCompleted = true;
				Tracking.DesignEvent(Tracking.Events.LevelCompleted, Parameters.Name);
				if (this.LevelCompleted != null)
				{
					this.LevelCompleted(this);
				}
			}
			InfoChanges.AchievedLevelTargets = Info.AchievedLevelTargets;
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			LocalStorage.Instance.UpdateLevelInfo(Info, Parameters.Name);
			if (CloudStorageST.ServerAvailable)
			{
				InfoChanges.global_dirty = false;
				CloudStorageST.Instance.UpdateLevelInfo(InfoChanges, Parameters.Name);
				InfoChanges = new LevelInfo();
			}
		}

		private void OnTargetAchieved(LevelTargetInfo tgtInfo, bool wasCompleted)
		{
			if (this.TargetAchievedForGigController != null)
			{
				this.TargetAchievedForGigController(tgtInfo);
			}
		}
	}
}
