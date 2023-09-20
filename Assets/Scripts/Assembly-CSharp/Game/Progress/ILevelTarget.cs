namespace Game.Progress
{
	public interface ILevelTarget : IId
	{
		string Description { get; set; }

		ILevel Parent { get; set; }

		bool IsCompletionTarget { get; set; }

		bool IsCompleted { get; }

		int Bonus { get; set; }
	}
}
