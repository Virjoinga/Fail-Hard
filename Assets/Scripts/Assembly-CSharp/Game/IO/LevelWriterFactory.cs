using UnityEngine;

namespace Game.IO
{
	public class LevelWriterFactory
	{
		public static ILevelWriter Construct(LevelFormat type)
		{
			ILevelWriter result = null;
			switch (type)
			{
			case LevelFormat.txt:
				Debug.LogWarning("Writing json not supported anymore.");
				break;
			case LevelFormat.bytes:
				result = new PBLevelWriter();
				break;
			default:
				result = null;
				break;
			}
			return result;
		}
	}
}
