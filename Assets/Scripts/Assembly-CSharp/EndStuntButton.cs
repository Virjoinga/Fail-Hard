using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class EndStuntButton : MonoBehaviour
{
	private GigStatus gigController;

	public UIPanel ScorePanel;

	public UIPanel StuntOverPanel;

	public UIPanel CareerLevelUpPanel;

	public float EndStuntDelay = 2f;

	public UISprite ButtonGlow;

	private bool m_ending;

	private List<Tweener> m_tweens;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		if (m_tweens == null)
		{
			m_tweens = new List<Tweener>();
		}
		if (gigController == null)
		{
			GameObject gameObject = GameObject.Find("Level:root");
			gigController = gameObject.GetComponentInChildren<GigStatus>();
			gigController.PrepareForStuntOver += gigController_PrepareForStuntOver;
		}
		DontAnimate();
	}

	private void gigController_PrepareForStuntOver()
	{
		AnimateButton();
	}

	public void TriggerEndStunt(bool delay = true)
	{
		if (m_ending)
		{
			DelayedEnd();
			return;
		}
		m_ending = true;
		gigController.SkipStunt();
		gigController.EndStunt();
		UIPanel toPanel = StuntOverPanel;
		if (gigController.CurrentGigState == GigStatus.GigState.GigOver)
		{
			toPanel = ScorePanel;
		}
		GameEvent gameEvent = gigController.ConsumeEvent(GameEvent.GameEventType.CareerLevelUp);
		if (gameEvent != null)
		{
			GetComponent<NavigateFromTo>().toPanel = CareerLevelUpPanel;
			GameController instance = GameController.Instance;
			int num = int.Parse(gameEvent.Data);
			int reward = instance.Scoring.CoinsForLevelUp(num);
			int nextStars = instance.Scoring.LevelUpCriteria(num + 1);
			CareerLevelUpPanel.GetComponent<CareerLevelUp>().SetData(toPanel, instance.Character.CareerStars, reward, nextStars);
			GetComponent<NavigateFromTo>().toPanel = CareerLevelUpPanel;
		}
		else
		{
			GetComponent<NavigateFromTo>().toPanel = toPanel;
		}
		if (delay)
		{
			Invoke("DelayedEnd", EndStuntDelay);
		}
		else
		{
			DelayedEnd();
		}
	}

	private void DelayedEnd()
	{
		if (m_ending)
		{
			GetComponent<NavigateFromTo>().ManualTrigger();
		}
		m_ending = false;
	}

	public void OnClick()
	{
		TriggerEndStunt(false);
	}

	private void AnimateButton()
	{
		if (m_tweens.Count <= 0)
		{
			TweenParms p_parms = new TweenParms().Prop("localScale", 1.3f * Vector3.one, false).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
			m_tweens.Add(HOTween.To(base.transform, 0.5f, p_parms));
			ButtonGlow.alpha = 0.1f;
			TweenParms p_parms2 = new TweenParms().Prop("alpha", 0.8f, false).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
			m_tweens.Add(HOTween.To(ButtonGlow, 0.5f, p_parms2));
		}
	}

	private void DontAnimate()
	{
		if (m_tweens != null)
		{
			foreach (Tweener tween in m_tweens)
			{
				tween.Kill();
			}
			m_tweens.Clear();
		}
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		ButtonGlow.alpha = 0f;
	}
}
