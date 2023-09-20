using System.Collections.Generic;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class TotalAchievedTargetsDisplay : MonoBehaviour
{
	private UILabel label;

	private int m_targetCount;

	private int m_stageCriteria;

	public Transform TargetIcon;

	public Transform TargetShine;

	private List<Tweener> m_tweens;

	private Vector3 m_originalScale;

	private Level m_currentLevel;

	private void Start()
	{
		Debug.LogError("DEPRECATED");
		label = GetComponentInChildren<UILabel>();
		if (GameController.Instance.CurrentLevel != null)
		{
			m_currentLevel = GameController.Instance.CurrentLevel;
			m_currentLevel.NewTargetAchieved += CurrentLevel_NewTargetAchieved;
		}
		GameController.Instance.Character.GameEventRegistered += Character_GameEventRegistered;
		label.text = m_targetCount + "/" + m_stageCriteria;
		m_tweens = new List<Tweener>();
	}

	private void Character_GameEventRegistered(GameEvent gameEvent)
	{
		if (gameEvent.EventType == GameEvent.GameEventType.StageCompleted)
		{
			AnimateGlow();
		}
	}

	private void AnimateGlow()
	{
		m_originalScale = TargetIcon.localScale;
		TweenParms p_parms = new TweenParms().Prop("localScale", 1.2f * m_originalScale, false).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
		m_tweens.Add(HOTween.To(TargetIcon, 0.4f, p_parms));
		m_tweens.Add(HOTween.To(TargetShine, 0.4f, p_parms));
		TargetIcon.localEulerAngles = new Vector3(0f, 0f, -10f);
		TweenParms p_parms2 = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 20f), true).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInOutSine);
		m_tweens.Add(HOTween.To(TargetIcon, 0.65f, p_parms2));
	}

	private void StopGlow()
	{
		if (m_tweens != null)
		{
			foreach (Tweener tween in m_tweens)
			{
				tween.Kill();
			}
			m_tweens.Clear();
		}
		TargetIcon.localEulerAngles = Vector3.zero;
		TargetIcon.localScale = m_originalScale;
	}

	private void CurrentLevel_NewTargetAchieved(LevelTargetInfo tgtInfo)
	{
		m_targetCount++;
		label.text = m_targetCount + "/" + m_stageCriteria;
	}

	private void OnDestroy()
	{
		if (m_currentLevel != null)
		{
			m_currentLevel.NewTargetAchieved -= CurrentLevel_NewTargetAchieved;
		}
		GameController.Instance.Character.GameEventRegistered -= Character_GameEventRegistered;
	}

	public void AnimateStageUnlock(int newCriteria)
	{
		m_stageCriteria = newCriteria;
		label.text = m_targetCount + "/" + m_stageCriteria;
		StopGlow();
	}
}
