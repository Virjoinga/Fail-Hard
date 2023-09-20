using Game;
using UnityEngine;

public class AJInitializer : MonoBehaviour
{
	public bool forceOffline;

	private GameController m_gc;

	public Everyplay everyplay;

	private bool m_initialized;

	public LanguageCode DefaultLanguage = LanguageCode.EN;

	public void Awake()
	{
		if (forceOffline)
		{
			Storage.ForceOffline = true;
			LocalStorage.Init();
		}
		else
		{
			CloudStorageST.Instance.Init();
		}
	}

	public void Start()
	{
		Language.SwitchLanguage(DefaultLanguage);
		m_gc = GameController.Instance;
	}

	public void Update()
	{
		if (!m_initialized)
		{
			LevelDatabase levelDatabase = m_gc.LevelDatabase;
			levelDatabase.LevelDatabasePopulated += LevelsReady;
			if (!levelDatabase.Initialized)
			{
				StartCoroutine(levelDatabase.Populate());
			}
			m_initialized = true;
		}
	}

	private void LevelsReady(LevelDatabase db)
	{
		Storage.Instance.UpdateCounter(Version.PROTOCOL);
	}
}
