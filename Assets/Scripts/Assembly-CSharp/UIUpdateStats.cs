using Game;
using Game.Util;
using UnityEngine;

public class UIUpdateStats : MonoBehaviour
{
	public GameObject HeaderRemainingStunts;

	public GameObject HeaderTargets;

	public GameObject HeaderMission;

	public GameObject HeaderCrowdMeter;

	public GameObject Tooltips;

	public GameObject NextLevelButton;

	public GameObject DisabledNextButton;

	public GameObject BundleListButton;

	public GameObject NextToGridButton;

	public GameObject GridButton;

	public GameObject DisabledGridButton;

	public GameObject RestartButton;

	public GameObject DisabledRestartButton;

	public LevelTargetDisplay levelTargets;

	public LevelTargetDisplay BestStars;

	public NumberLabelAnimator BestCoins;

	public NumberLabelAnimator TotalCoins;

	public MissionStatusIndicator MissionStatus;

	public TotalAchievedTargetsDisplay TotalTargetsDisplay;

	public GameplayControls GameplayControls;

	public Ads Ads;

	private GigStatus m_gigController;

	private int m_highscore;

	private bool m_skipped;

	private bool m_highScoreInitialized;

	private bool m_targetsAnimated;

	private void Awake()
	{
		m_highscore = SafeGigController().FirstStunt().HighScoreAtStart;
		levelTargets.SetLevel(GameController.Instance.CurrentLevel, 0, 120);
		if (SafeGigController().StarsAtGigStart() >= GameController.Instance.CurrentLevel.CompletionCriteria)
		{
			MissionStatus.SetState(true);
			BestStars.SetLevel(GameController.Instance.CurrentLevel, SafeGigController().StarsAtGigStart(), 40);
			m_highScoreInitialized = true;
		}
		else
		{
			MissionStatus.gameObject.SetActive(false);
			MissionStatus.SetState(false);
		}
	}

	private GigStatus SafeGigController()
	{
		if (m_gigController == null)
		{
			GameObject gameObject = GameObject.Find("Level:root");
			m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		}
		return m_gigController;
	}

	private void OnEnable()
	{
		GameplayControls.Hide();
		Ads.Show();
		HeaderRemainingStunts.SetActive(false);
		HeaderTargets.SetActive(false);
		HeaderMission.SetActive(false);
		HeaderCrowdMeter.SetActive(false);
		BestCoins.SkipTo(m_highscore, false);
		Invoke("UpdateStats", 0.2f);
		ConfigureButtons();
	}

	private void ConfigureButtons()
	{
		if (SafeGigController().ConsumeEvent(GameEvent.GameEventType.StageCompleted) != null && GameController.Instance.CurrentBundle.CurrentStage != GameController.Instance.CurrentBundle.Stages.Count)
		{
			NextToGridButton.SetActive(true);
			return;
		}
		int stage;
		Level level = GameController.Instance.CurrentBundle.NextLevel(GameController.Instance.CurrentLevel, true, out stage);
		if (level == null)
		{
			if (!GameController.Instance.CurrentLevel.IsCompleted)
			{
				DisabledNextButton.SetActive(true);
				NextLevelButton.SetActive(false);
			}
			else if (SafeGigController().PeekEvent(GameEvent.GameEventType.LevelCompleted) != null)
			{
				NextToGridButton.SetActive(false);
				GridButton.SetActive(false);
				DisabledGridButton.SetActive(true);
				RestartButton.SetActive(false);
				DisabledRestartButton.SetActive(true);
				BundleListButton.SetActive(true);
			}
			else
			{
				BundleListButton.SetActive(true);
			}
		}
		else if (level.IsLocked)
		{
			if (GameController.Instance.CurrentLevel.IsCompleted)
			{
				NextToGridButton.SetActive(true);
			}
			else
			{
				DisabledNextButton.SetActive(true);
			}
		}
		else
		{
			NextLevelButton.SetActive(true);
		}
	}

	public void UpdateStats()
	{
		if (!m_gigController)
		{
			Logger.Error("No gig controller! Cannot update stats!");
			return;
		}
		levelTargets.UpdateTargetStatus(SafeGigController().StarsDuringGig());
		TotalCoins.targetValue = SafeGigController().GigStatistics.TotalGigCoins;
		if (m_highscore < GameController.Instance.CurrentLevel.HighScore)
		{
			BestCoins.targetValue = GameController.Instance.CurrentLevel.HighScore;
			m_highscore = GameController.Instance.CurrentLevel.HighScore;
		}
	}

	public void OnTargetsAnimated()
	{
		if (m_targetsAnimated)
		{
			return;
		}
		for (int i = 0; i < SafeGigController().StarsDuringGig(); i++)
		{
			TotalCoins.targetValue += GameController.Instance.Scoring.CoinsForStar(i + 1);
		}
		m_targetsAnimated = true;
		MissionStatus.gameObject.SetActive(true);
		GameEvent gameEvent = SafeGigController().ConsumeEvent(GameEvent.GameEventType.LevelCompleted);
		if (gameEvent != null)
		{
			MissionStatus.SetState(true);
			if (GameController.Instance.Character.ContainsEvent(new GameEvent(GameEvent.GameEventType.LevelCompleted, "BeachRowboat")))
			{
				AskRateMe();
			}
		}
		else if (!GameController.Instance.CurrentLevel.IsCompleted)
		{
			MissionStatus.SetState(false);
		}
		else if (m_highScoreInitialized && SafeGigController().StarsAtGigStart() < SafeGigController().StarsDuringGig())
		{
			BestStars.UpdateTargetStatus(SafeGigController().StarsDuringGig() - SafeGigController().StarsAtGigStart());
		}
	}

	private void AskRateMe()
	{
		UniRate.Instance.LogEvent(true);
	}

	public void Skip()
	{
		m_skipped = true;
		TotalCoins.SkipTo(TotalCoins.targetValue, true);
		levelTargets.SkipAnimation();
	}

	public void OnClick()
	{
		if (!m_skipped)
		{
			Skip();
		}
	}
}
