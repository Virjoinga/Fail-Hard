using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game.Progress
{
	public class Agent
	{
		private int unlockCount;

		private List<ILevel> promotionQueue;

		[method: MethodImpl(32)]
		public event StageCompletedHandler StageCompleted;

		[method: MethodImpl(32)]
		public event LevelUnlockedHandler LevelUnlocked;

		public Agent()
		{
			promotionQueue = new List<ILevel>();
			unlockCount = 0;
		}

		public ILevel[] PromotedLevels()
		{
			ILevel[] array = new ILevel[promotionQueue.Count];
			promotionQueue.CopyTo(array, 0);
			return array;
		}

		public ILevel GetNextLevel(Level l)
		{
			if (promotionQueue.Count > 0)
			{
				return promotionQueue[0];
			}
			return null;
		}

		public void AddLevel(ILevel level)
		{
			if (level.ThemeCategory == ThemeCategory.Development)
			{
				((Level)level).Unlock(0, false);
				return;
			}
			GameController.Instance.BundleModel.UpdateBundleData((Level)level);
			((Level)level).NewStarAchieved += StarAchieved;
			level.LevelCompleted += LevelCompleted;
			if (!level.IsLocked && unlockCount < ((Level)level).UnlockingIndex)
			{
				unlockCount = ((Level)level).UnlockingIndex;
			}
		}

		public void LevelLoadingDone()
		{
			GameController.Instance.BundleModel.Dump();
			CheckBundleUnlocks(false);
			foreach (Bundle value in GameController.Instance.BundleModel.Model.Values)
			{
				if (!(value.Id == "Development"))
				{
					value.RefreshCurrentStage();
					CheckLevelUnlock(value, 0);
				}
			}
			GameController.Instance.Character.GameEventRegistered += OnGameEventRegistered;
		}

		public void OnGameEventRegistered(GameEvent ge)
		{
		}

		public void LevelCompleted(ILevel level)
		{
			GameController instance = GameController.Instance;
			instance.Character.AddGameEvent(new GameEvent(GameEvent.GameEventType.LevelCompleted, level.Parameters.Name));
			if (promotionQueue.Contains(level))
			{
				promotionQueue.Remove(level);
			}
			if (!CheckLevelUnlock(instance.CurrentBundle, instance.CurrentBundle.CurrentStage) && instance.CurrentBundle.State == BundleState.BundlePurchased && instance.CurrentBundle.CurrentStage >= instance.CurrentBundle.Stages.Count - 1)
			{
				instance.CurrentBundle.State = BundleState.BundleCompletePending;
			}
			RefreshStageUnlock();
		}

		public void StarAchieved(string levelId)
		{
			Bundle currentBundle = GameController.Instance.CurrentBundle;
			currentBundle.TargetAchieved(levelId);
			RefreshStageUnlock();
		}

		private bool CheckLevelUnlock(Bundle bundle, int startFromStage)
		{
			if (bundle.State == BundleState.BundleLocked || bundle.State == BundleState.BundleUnlocked)
			{
				return false;
			}
			foreach (BundleStage stage in bundle.Stages)
			{
				if (stage.Index < startFromStage)
				{
					continue;
				}
				if (stage.Index > bundle.CurrentStage)
				{
					return false;
				}
				foreach (BundleLevel level2 in stage.Levels)
				{
					Level level = GameController.Instance.LevelDatabase.LevelForName(level2.LevelId);
					if (level == null)
					{
						Debug.LogWarning("Playing in editor? Level not found " + level2.LevelId);
					}
					else if (!level.IsCompleted)
					{
						if (!level.IsLocked)
						{
							return false;
						}
						level.Unlock(++unlockCount, false);
						if (this.LevelUnlocked != null)
						{
							this.LevelUnlocked(level);
						}
						return true;
					}
				}
			}
			return false;
		}

		public bool RefreshStageUnlock()
		{
			Bundle currentBundle = GameController.Instance.CurrentBundle;
			if (currentBundle.CurrentStage < currentBundle.Stages.Count)
			{
				if (currentBundle.AchievedTargets < currentBundle.StageCriteria(currentBundle.CurrentStage))
				{
					return false;
				}
				string levelId = currentBundle.Stages[currentBundle.CurrentStage].Levels[currentBundle.Stages[currentBundle.CurrentStage].Levels.Count - 1].LevelId;
				Level level = GameController.Instance.LevelDatabase.LevelForName(levelId);
				if (level == null)
				{
					Debug.LogWarning("Playing in editor? Level not found " + levelId);
					return false;
				}
				if (level.IsCompleted)
				{
					if (this.StageCompleted != null)
					{
						this.StageCompleted(currentBundle.CurrentStage);
					}
					string data = currentBundle.Id + ":" + currentBundle.CurrentStage;
					GameController.Instance.Character.AddGameEvent(new GameEvent(GameEvent.GameEventType.StageCompleted, data));
					currentBundle.CurrentStage++;
					if (currentBundle.CurrentStage == currentBundle.Stages.Count)
					{
					}
					NotificationCentre.Send(Notification.Types.StageCompleted, null);
					CheckBundleUnlocks(true);
					CheckLevelUnlock(currentBundle, currentBundle.CurrentStage);
					return true;
				}
			}
			return false;
		}

		public void BundlePurchased(Bundle bundle)
		{
			bundle.State = BundleState.BundlePurchased;
			CheckBundleUnlocks(true);
			CheckLevelUnlock(bundle, 0);
		}

		private void CheckBundleUnlocks(bool notify)
		{
			GameController instance = GameController.Instance;
			foreach (KeyValuePair<string, Bundle> item in instance.BundleModel.Model)
			{
				if (item.Value.State != 0 || !instance.Character.ContainsEvents(item.Value.Preconditions))
				{
					continue;
				}
				if (item.Value.Price.Amount == 0)
				{
					item.Value.State = BundleState.BundlePurchased;
					if (item.Value.Id != "Development")
					{
						GameController.Instance.BundleToShow = item.Value;
					}
				}
				else
				{
					item.Value.State = BundleState.BundleUnlocked;
				}
				if (!notify)
				{
				}
			}
		}
	}
}
