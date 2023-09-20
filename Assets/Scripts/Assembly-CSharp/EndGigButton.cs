using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class EndGigButton : MonoBehaviour
{
	private GigStatus m_gigController;

	public UIPanel StageCompletedPanel;

	public UIPanel ScorePanel;

	public GameObject ButtonSprite;

	public UISprite ButtonGlow;

	public Ads Ads;

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
		if (m_gigController == null)
		{
			GameObject gameObject = GameObject.Find("Level:root");
			m_gigController = gameObject.GetComponentInChildren<GigStatus>();
			m_gigController.StateChanged += m_gigController_StateChanged;
		}
		GetComponent<NavigateFromTo>().toPanel = ScorePanel;
	}

	private void m_gigController_StateChanged(GigStatus.GigState newState)
	{
		if (newState == GigStatus.GigState.StuntReadyToStart)
		{
			RefreshButtonState();
		}
	}

	private void RefreshButtonState()
	{
		if (!GameController.Instance.CurrentLevel.IsCompleted || m_gigController.GetStuntCount() == 0)
		{
			ButtonSprite.SetActive(false);
			base.collider.enabled = false;
		}
		else
		{
			ButtonSprite.SetActive(true);
			base.collider.enabled = true;
		}
		if (m_gigController.EventsDuringGig.Count > 0)
		{
			AnimateButton();
		}
		else
		{
			DontAnimate();
		}
	}

	private void OnClick()
	{
		Ads.Show();
		m_gigController.EndStunt();
	}

	private void AnimateButton()
	{
		if (m_tweens.Count <= 0)
		{
			TweenParms p_parms = new TweenParms().Prop("localScale", 1.04f * Vector3.one, false).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
			m_tweens.Add(HOTween.To(base.transform, 0.5f, p_parms));
			base.transform.localEulerAngles = new Vector3(0f, 0f, -5f);
			TweenParms p_parms2 = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 10f), true).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
			m_tweens.Add(HOTween.To(base.transform, 0.75f, p_parms2));
			ButtonGlow.alpha = 0.1f;
			TweenParms p_parms3 = new TweenParms().Prop("alpha", 0.8f, false).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
			m_tweens.Add(HOTween.To(ButtonGlow, 0.5f, p_parms3));
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
