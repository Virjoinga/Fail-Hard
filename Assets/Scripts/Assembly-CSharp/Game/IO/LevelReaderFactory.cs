using UnityEngine;

namespace Game.IO
{
	public class LevelReaderFactory
	{
		public static ILevelReader Construct(LevelFormat type)
		{
			ILevelReader result = null;
			switch (type)
			{
			case LevelFormat.txt:
				Debug.LogWarning("Reading json supported anymore.");
				break;
			case LevelFormat.bytes:
				result = new PBLevelReader();
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
	}
}
