using Game;
using Game.Util;
using UnityEngine;

public class ToCompleteDisplay : MonoBehaviour
{
	private enum WidgetState
	{
		Uncompleted = 0,
		TargetAnimation = 1,
		Sliding = 2,
		Completed = 3
	}

	public LevelTargetDisplay LevelTargets;

	public float MaxTargetScale;

	public bool Animated;

	public UISprite Background;

	private WidgetState state;

	private int previousTargetCount;

	private bool previousCompletedState;

	private Vector3 cachedPos;

	private TweenRotation rotationTween;

	private TweenPosition positionTween;

	private GigStatus m_gigController;

	private void Awake()
	{
		int targetsAtGigStart = GameController.Instance.CurrentLevel.TargetsAchieved - SafeGigController().TotalTargetsDuringGig();
		LevelTargets.SetLevel(GameController.Instance.CurrentLevel, targetsAtGigStart, 55);
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
		if (Background != null)
		{
			Vector3 localScale = Background.transform.localScale;
			localScale.x = Mathf.Max(171, (GameController.Instance.CurrentLevel.TargetCount() + 2) * 55);
			Background.transform.localScale = localScale;
		}
		Invoke("UpdateStats", 0.2f);
	}

	public void UpdateStats()
	{
		if (!SafeGigController())
		{
			Logger.Error("No gig controller! Cannot update stats!");
		}
		else if (Animated)
		{
			LevelTargets.UpdateTargetStatus(SafeGigController().TotalTargetsAchieved());
		}
		else
		{
			LevelTargets.UpdateWithoutAnimation(SafeGigController().TotalTargetsAchieved());
		}
	}

	public void OnTargetsAnimated()
	{
	}

	public void Skip()
	{
		LevelTargets.SkipAnimation();
		OnTargetsAnimated();
	}

	public void OnClick()
	{
		Skip();
	}
}
