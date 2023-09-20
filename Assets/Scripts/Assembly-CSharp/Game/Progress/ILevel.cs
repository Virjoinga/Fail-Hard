namespace Game.Progress
{
	public interface ILevel : IId
	{
		int TargetsAchieved { get; }

		Stage Stage { get; set; }

		ThemeCategory ThemeCategory { get; }

		bool IsLocked { get; }

		bool IsCompleted { get; set; }

		LevelParameters Parameters { get; }

		event LevelCompletedHandler LevelCompleted;

		void Unlock(int unlockingIndex);
	}
}
