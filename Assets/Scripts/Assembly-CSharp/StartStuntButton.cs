using System.Collections.Generic;
using Game;
using Game.Util;
using Holoville.HOTween;
using UnityEngine;

public class StartStuntButton : MonoBehaviour
{
	public UIUpdateStats scoreUI;

	private GigStatus m_gigStatus;

	private NavigateFromTo navi;

	public UISprite ButtonGlow;

	private List<Tweener> m_tweens;

	private void Start()
	{
		Init();
		navi = GetComponent<NavigateFromTo>();
	}

	private void Init()
	{
		if (m_tweens == null)
		{
			m_tweens = new List<Tweener>();
		}
		if (m_gigStatus == null)
		{
			GameObject gameObject = GameObject.Find("Level:root");
			if ((bool)gameObject)
			{
				m_gigStatus = gameObject.GetComponentInChildren<GigStatus>();
			}
		}
	}

	private void OnEnable()
	{
		Init();
		if (m_gigStatus.EventsDuringGig.Count == 0 && !GameController.Instance.CurrentLevel.IsCompleted)
		{
			AnimateButton();
		}
		else
		{
			DontAnimate();
		}
	}

	public void OnClick()
	{
		if ((bool)m_gigStatus)
		{
			m_gigStatus.EndStunt();
			m_gigStatus.StartStunt();
			navi.ManualTrigger();
		}
		else
		{
			Logger.Error("No gig status, cannot start stunt!");
		}
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
