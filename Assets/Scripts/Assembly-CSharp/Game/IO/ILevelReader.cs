using UnityEngine;

namespace Game.IO
{
	public interface ILevelReader
	{
		LevelParameters ReadInfo(TextAsset data);

		LevelParameters ReadInfo(string path);

		LevelRootBlock ReadRoot(TextAsset data);
	}
}
