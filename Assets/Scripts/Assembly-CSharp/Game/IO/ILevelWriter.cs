namespace Game.IO
{
	public interface ILevelWriter
	{
		void WriteInfo(LevelParameters data, string file);

		void WriteRoot(LevelRootBlock data, string file);
	}
}
