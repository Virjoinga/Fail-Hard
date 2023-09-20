using UnityEngine;

namespace Game
{
	public class Prefab
	{
		public string FilePath { get; private set; }

		private GameObject load()
		{
			if (FilePath.Length <= 0)
			{
				Debug.LogError("Tried to load prefab with no given path");
				return null;
			}
			GameObject gameObject = (GameObject)Resources.Load(FilePath, typeof(GameObject));
			if (!gameObject)
			{
				Debug.LogError("Could not load prefab from: " + FilePath);
			}
			return gameObject;
		}
	}
}
