using System.Collections.Generic;
using Game.Progress;
using Game.Util;
using UnityEngine;

namespace Game
{
	public class GameController
	{
		public enum GameUIView
		{
			MainView = 0,
			Garage = 1,
			ThemeSelection = 2,
			LevelGrid = 3
		}

		public enum GameState
		{
			Menu = 0,
			Playing = 1
		}

		private static GameController s_instance;

		private GameState m_state;

		private ILevel m_levelToLoad;

		private static bool m_initializing;

		public Agent Agent { get; set; }

		public ScoringAlgorithm Scoring { get; set; }

		public ItemListModel ItemModelPlaceholder { get; set; }

		public VehicleListModel VehicleModel { get; set; }

		public BundleListModel BundleModel { get; set; }

		public ThemeCategory MyCurrentTheme { get; set; }

		public Bundle CurrentBundle { get; set; }

		public GameUIView CurrentView { get; set; }

		public Bundle BundleToShow { get; set; }

		public Level CurrentLevel { get; set; }

		public Area SelectedArea { get; private set; }

		public LevelDatabase LevelDatabase { get; private set; }

		public Character Character { get; private set; }

		private AudioManager m_audioManager
		{
			get
			{
				return AudioManager.Instance;
			}
			set
			{
			}
		}

		public StoreFront Store { get; private set; }

		public static GameController Instance
		{
			get
			{
				if (s_instance == null)
				{
					if (m_initializing)
					{
						Debug.LogError("Accessing GameController.Instance while initializing. This is bad.");
						return null;
					}
					m_initializing = true;
					s_instance = new GameController();
				}
				else
				{
					m_initializing = false;
				}
				return s_instance;
			}
		}

		private GameController()
		{
			LevelDatabase = new LevelDatabase();
			LevelDatabase.LevelDatabasePopulated += LevelsReady;
			ItemModelPlaceholder = ItemListModel.Instance;
			VehicleModel = VehicleListModel.Instance;
			BundleModel = BundleListModel.Instance;
			Character = new Character();
			Agent = new Agent();
			Scoring = new ScoringAlgorithm();
			Store = new StoreFront();
			CurrentView = GameUIView.MainView;
			Character.ConnectStore(Store);
		}

		private void LevelsReady(LevelDatabase db)
		{
			Agent.LevelLoadingDone();
			BundleModel.RefreshBundleStates();
			Character.Mechanics[0].RestoreVehicleStates();
			LocalStorage.Instance.ClearDirty();
			Store.Initialize();
			VideoAds.Instance.Initialize(Store);
		}

		public GameState state()
		{
			return m_state;
		}

		public void SetState(GameState state)
		{
			m_state = state;
		}

		public void SetSelectedArea(int id)
		{
			SelectedArea = new Area(id);
		}

		public List<Area> areas()
		{
			List<Area> result = new List<Area>();
			Debug.LogError("Area loading not functional");
			return result;
		}

		public void SetLevelToLoad(ILevel level)
		{
			m_levelToLoad = level;
			CurrentLevel = m_levelToLoad as Level;
			MyCurrentTheme = m_levelToLoad.Parameters.ThemeCategory;
		}

		public AsyncOperation EnterPlayMode(bool async = false)
		{
			Logger.Log("Trying to load scene: " + m_levelToLoad.Parameters.Scene + " with level: " + m_levelToLoad.Parameters.Name);
			AsyncOperation result = null;
			if (async)
			{
				result = Application.LoadLevelAdditiveAsync(m_levelToLoad.Parameters.Scene);
			}
			else
			{
				Application.LoadLevel(m_levelToLoad.Parameters.Scene);
			}
			SetState(GameState.Playing);
			CurrentLevel = m_levelToLoad as Level;
			m_levelToLoad = null;
			return result;
		}
	}
}
