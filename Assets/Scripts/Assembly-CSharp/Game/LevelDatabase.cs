using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Game.IO;
using Game.Progress;
using Game.Util;
using UnityEngine;

namespace Game
{
	public class LevelDatabase
	{
		private Dictionary<string, Level> m_levels;

		public List<Theme> Themes { get; set; }

		public bool Initialized { get; private set; }

		[method: MethodImpl(32)]
		public event LevelDatabasePopulatedHandler LevelDatabasePopulated;

		public LevelDatabase()
		{
			m_levels = new Dictionary<string, Level>();
			Themes = new List<Theme>();
			Initialized = false;
		}

		public IEnumerator Populate()
		{
			Object[] assets = Resources.LoadAll("Levels", typeof(TextAsset));
			Object[] array = assets;
			foreach (Object obj in array)
			{
				TextAsset asset = obj as TextAsset;
				if ((bool)asset)
				{
					ILevelReader reader = LevelReaderFactory.Construct(LevelFormat.bytes);
					LevelParameters info = reader.ReadInfo(asset);
					Level level = new Level(info);
					m_levels.Add(info.Name, level);
					AddToThemes(level);
					yield return true;
				}
				else
				{
					Logger.Error("Tried to read nontext asset for level!");
				}
			}
			while (Level.levelLoadCounter != 0)
			{
				Thread.Sleep(100);
			}
			Initialized = true;
			if (this.LevelDatabasePopulated != null)
			{
				this.LevelDatabasePopulated(this);
			}
		}

		private void AddToThemes(Level level)
		{
			foreach (Theme theme in Themes)
			{
				if (theme.Category == level.Parameters.ThemeCategory)
				{
					return;
				}
			}
			Theme item = new Theme(level.Parameters.ThemeCategory);
			Themes.Add(item);
		}

		public Level[] Levels()
		{
			Level[] array = new Level[m_levels.Count];
			m_levels.Values.CopyTo(array, 0);
			return array;
		}

		public Level[] Levels(ThemeCategory themeFilter)
		{
			List<ILevel> list = new List<ILevel>();
			foreach (KeyValuePair<string, Level> level in m_levels)
			{
				if (level.Value.ThemeCategory == themeFilter)
				{
					list.Add(level.Value);
				}
			}
			Level[] array = new Level[list.Count];
			list.CopyTo(array, 0);
			return array;
		}

		public Level[] Levels(ThemeCategory themeFilter, int stage)
		{
			List<ILevel> list = new List<ILevel>();
			foreach (KeyValuePair<string, Level> level in m_levels)
			{
				if (level.Value.ThemeCategory == themeFilter && level.Value.Stage == (Stage)stage)
				{
					list.Add(level.Value);
				}
			}
			Level[] array = new Level[list.Count];
			list.CopyTo(array, 0);
			return array;
		}

		public Level[] Levels(ThemeCategory themeFilter, int minStage, int maxStage)
		{
			List<ILevel> list = new List<ILevel>();
			foreach (KeyValuePair<string, Level> level in m_levels)
			{
				if (level.Value.ThemeCategory == themeFilter && (int)level.Value.Stage >= minStage && (int)level.Value.Stage <= maxStage)
				{
					list.Add(level.Value);
				}
			}
			Level[] array = new Level[list.Count];
			list.CopyTo(array, 0);
			return array;
		}

		public Level NextLevel(Level current)
		{
			Level[] array = Levels(current.ThemeCategory);
			Level result = null;
			float num = 99999f;
			float num2 = (float)((int)current.Stage * 1000) + current.Parameters.Order;
			Level[] array2 = array;
			foreach (Level level in array2)
			{
				float num3 = (float)((int)level.Stage * 1000) + level.Parameters.Order;
				if (num3 < num && level != current && num3 > num2)
				{
					num = num3;
					result = level;
				}
			}
			return result;
		}

		public Level LevelForName(string name)
		{
			if (!m_levels.ContainsKey(name))
			{
				return null;
			}
			return m_levels[name];
		}

		public LevelParameters LevelParametersForName(string name)
		{
			if (!m_levels.ContainsKey(name))
			{
				return null;
			}
			Level level = m_levels[name];
			if (level != null)
			{
				return level.Parameters;
			}
			return null;
		}

		public void TotalTargetAmountsForUnlockedLevels(ThemeCategory themeFilter, out int achievedTargetAmount, out int totalTargetAmount)
		{
			achievedTargetAmount = 0;
			totalTargetAmount = 0;
			foreach (KeyValuePair<string, Level> level in m_levels)
			{
				if (level.Value.ThemeCategory == themeFilter && !level.Value.IsLocked)
				{
					totalTargetAmount += level.Value.TargetCountForCurrentCareerState();
					achievedTargetAmount += level.Value.TargetsAchieved;
				}
			}
		}

		public void TotalTargetAmounts(ThemeCategory themeFilter, out int achievedTargetAmount, out int totalTargetAmount)
		{
			achievedTargetAmount = 0;
			totalTargetAmount = 0;
			foreach (KeyValuePair<string, Level> level in m_levels)
			{
				if (level.Value.ThemeCategory == themeFilter)
				{
					totalTargetAmount += level.Value.TargetCount();
					achievedTargetAmount += level.Value.TargetsAchieved;
				}
			}
		}
	}
}
