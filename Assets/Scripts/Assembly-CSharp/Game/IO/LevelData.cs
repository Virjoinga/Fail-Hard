namespace Game.IO
{
	public class LevelData
	{
		public LevelParameters info;

		public LevelRootBlock levelRoot;

		public override string ToString()
		{
			string text = string.Empty;
			if (info != null)
			{
				text += info.ToString();
			}
			if (levelRoot != null)
			{
				text = text + ":" + levelRoot.ToString();
			}
			return text;
		}
	}
}
