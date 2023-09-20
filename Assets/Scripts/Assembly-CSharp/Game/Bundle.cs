using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
	public class Bundle
	{
		public delegate void OnStateChanged(Bundle bundle);

		private BundleParameters m_bundleParams;

		private BundleInfo m_bundleInfo;

		private BundleInfo m_bundleInfoChanges;

		public string Id
		{
			get
			{
				return m_bundleParams.Id;
			}
		}

		public string Name
		{
			get
			{
				return m_bundleParams.Name;
			}
		}

		public string Description { get; set; }

		public string ResourceName { get; set; }

		public List<BundleStage> Stages
		{
			get
			{
				return m_bundleParams.Stages;
			}
		}

		public Price Price
		{
			get
			{
				return m_bundleParams.Price;
			}
		}

		public List<GameEvent> Preconditions
		{
			get
			{
				return m_bundleParams.Preconditions;
			}
		}

		public int AchievedTargets { get; set; }

		public int CurrentStage { get; set; }

		public BundleStage NowPlayingStage { get; set; }

		public string NowPlayingLevel { get; set; }

		public int LastAnimatedStage { get; set; }

		public BundleState State
		{
			get
			{
				return m_bundleInfo.State;
			}
			set
			{
				m_bundleInfo.State = value;
				m_bundleInfoChanges.State = value;
				UpdateInfo();
				if (this.StateChanged != null)
				{
					this.StateChanged(this);
				}
			}
		}

		[method: MethodImpl(32)]
		public event OnStateChanged StateChanged;

		public Bundle(BundleParameters parameters, BundleInfo info)
		{
			m_bundleParams = parameters;
			InitStageIndexes();
			m_bundleInfo = info;
			m_bundleInfoChanges = new BundleInfo();
			HardCodedParams();
		}

		private void HardCodedParams()
		{
			if (Id == "PV50City")
			{
				ResourceName = "Levels/citybundle";
				Description = "Unlock a new exciting chapter in your stuntman career today!";
			}
			else if (Id == "PV50Backyard")
			{
				ResourceName = "Levels/suburbbundle";
				Description = "Unlock a new exciting chapter in your stuntman career today!";
			}
			else if (Id == "PV50Beach")
			{
				ResourceName = "Levels/beachbundle";
				Description = "Unlock a new exciting chapter in your stuntman career today!";
			}
			else if (Id == "Wings")
			{
				ResourceName = "Levels/wingsbundle";
				Description = "Make sure you have the wings before entering.";
			}
			else if (Id == "Monster")
			{
				ResourceName = "Levels/monsterbundle";
				Description = "Rough terrains ahead, Monster is recommended!";
			}
			else if (Id == "PV50Beginner")
			{
				ResourceName = "Levels/gravelpitbundle";
				Description = "Unlock a new exciting chapter in your stuntman career today!";
			}
			else if (Id == "Snow")
			{
				ResourceName = "Levels/snowbundle";
				Description = "Playing in snow has never been so fun!";
			}
			else if (Id == "Rocket")
			{
				ResourceName = "Levels/rocketbundle";
				Description = "Make sure you have the rocket pack before entering.";
			}
			else if (Id == "HC")
			{
				ResourceName = "Levels/hcbundle";
				Description = "For real stuntmen only.";
			}
			else
			{
				ResourceName = "Levels/beachbundle";
				Description = "Unlock a new exciting chapter in your stuntman career today!";
			}
		}

		private void InitStageIndexes()
		{
			int num = 0;
			foreach (BundleStage stage in m_bundleParams.Stages)
			{
				stage.Index = num;
				num++;
			}
		}

		public void InitStageUnlockAnimationState()
		{
			int lastAnimatedStage = RefreshCurrentStage();
			LastAnimatedStage = lastAnimatedStage;
		}

		public void AddStage(BundleStage bs)
		{
			bs.Index = Stages.Count;
			Stages.Add(bs);
		}

		public void TargetAchieved(Level level)
		{
			foreach (BundleStage stage in Stages)
			{
				if (stage.ContainsLevel(level.Parameters.Name))
				{
					AchievedTargets++;
					break;
				}
			}
		}

		public void TargetAchieved(string levelId)
		{
			foreach (BundleStage stage in Stages)
			{
				if (stage.ContainsLevel(levelId))
				{
					AchievedTargets++;
					break;
				}
			}
		}

		public void SetNowPlaying(Level level)
		{
			foreach (BundleStage stage in Stages)
			{
				if (stage.ContainsLevel(level.Parameters.Name))
				{
					NowPlayingStage = stage;
					NowPlayingLevel = level.Parameters.Name;
					break;
				}
			}
			GameController.Instance.SetLevelToLoad(level);
		}

		public Level NextLevel(Level current, bool allowLocked, out int stage)
		{
			foreach (BundleStage stage2 in Stages)
			{
				if (stage2.Index > CurrentStage && !allowLocked)
				{
					stage = -1;
					return null;
				}
				int num = stage2.Levels.FindIndex((BundleLevel item) => item.LevelId == current.Parameters.Name);
				if (num < 0)
				{
					continue;
				}
				Level level = null;
				stage = stage2.Index;
				if (num >= stage2.Levels.Count - 1)
				{
					if (stage2.Index < Stages.Count - 1)
					{
						level = GameController.Instance.LevelDatabase.LevelForName(Stages[stage2.Index + 1].Levels[0].LevelId);
						stage = stage2.Index + 1;
					}
					else
					{
						level = null;
					}
				}
				else
				{
					level = GameController.Instance.LevelDatabase.LevelForName(stage2.Levels[num + 1].LevelId);
				}
				if (level != null && !allowLocked && level.IsLocked)
				{
					level = null;
				}
				return level;
			}
			stage = -1;
			return null;
		}

		public bool UpdateDataForLevel(Level level)
		{
			foreach (BundleStage stage in Stages)
			{
				if (stage.ContainsLevel(level.Parameters.Name))
				{
					stage.TotalTargets += level.TargetCount();
					AchievedTargets += level.TargetsAchieved;
					return true;
				}
			}
			return false;
		}

		public int RefreshCurrentStage()
		{
			int num = 0;
			int num2 = 0;
			foreach (BundleStage stage in Stages)
			{
				num = stage.Index;
				bool flag = false;
				foreach (BundleLevel level2 in stage.Levels)
				{
					Level level = GameController.Instance.LevelDatabase.LevelForName(level2.LevelId);
					if (level == null)
					{
						Debug.LogWarning("Skipping missing level " + level2.LevelId);
					}
					else if (!level.IsCompleted)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				num2 += stage.TotalTargets;
				if (AchievedTargets < (int)((float)num2 * stage.CompletedCriteria))
				{
					break;
				}
			}
			CurrentStage = num;
			return num;
		}

		public int TotalTargets()
		{
			int num = 0;
			foreach (BundleStage stage in Stages)
			{
				num += stage.TotalTargets;
			}
			return num;
		}

		public int TotalTargetsForCurrentStage()
		{
			int num = 0;
			foreach (BundleStage stage in Stages)
			{
				num += stage.TotalTargets;
				if (AchievedTargets < (int)((float)num * stage.CompletedCriteria))
				{
					break;
				}
			}
			return num;
		}

		public int TotalTargetsForStage(int index)
		{
			int num = 0;
			foreach (BundleStage stage in Stages)
			{
				if (stage.Index <= index)
				{
					num += stage.TotalTargets;
					continue;
				}
				break;
			}
			return num;
		}

		public int StageCriteria(int index)
		{
			if (index >= Stages.Count)
			{
				Debug.LogWarning("Invalid index");
				index = Stages.Count - 1;
			}
			return (int)((float)TotalTargetsForStage(index) * Stages[index].CompletedCriteria);
		}

		private void UpdateInfo()
		{
			LocalStorage.Instance.UpdateBundleInfo(m_bundleInfo, m_bundleParams.Id);
			if (CloudStorageST.ServerAvailable)
			{
				CloudStorageST.Instance.UpdateBundleInfo(m_bundleInfoChanges, m_bundleParams.Id);
				m_bundleInfoChanges = new BundleInfo();
			}
		}

		public List<int> StarCriteria(string levelId)
		{
			foreach (BundleStage stage in Stages)
			{
				int num = stage.Levels.FindIndex((BundleLevel item) => item.LevelId == levelId);
				if (num >= 0)
				{
					List<int> list = new List<int>();
					list.Add((int)stage.Levels[num].StarCriteria[0]);
					list.Add((int)stage.Levels[num].StarCriteria[1]);
					list.Add((int)stage.Levels[num].StarCriteria[2]);
					return list;
				}
			}
			return null;
		}
	}
}
