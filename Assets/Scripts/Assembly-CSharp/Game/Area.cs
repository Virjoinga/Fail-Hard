using System.Collections.Generic;
using Game.Util;
using UnityEngine;

namespace Game
{
	public class Area
	{
		public string Name { get; private set; }

		public int ID { get; private set; }

		public Area(int id)
		{
			ID = id;
		}

		public List<LevelParameters> levelInfos()
		{
			List<LevelParameters> result = new List<LevelParameters>();
			Logger.Error("Areas level info not implemented");
			return result;
		}

		public bool Load()
		{
			Application.LoadLevel(ID);
			return true;
		}

		public bool Load(Level level)
		{
			Application.LoadLevel(ID);
			return true;
		}
	}
}
