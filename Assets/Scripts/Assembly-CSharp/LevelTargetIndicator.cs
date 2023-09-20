using Holoville.HOTween;
using UnityEngine;

public class LevelTargetIndicator : MonoBehaviour
{
	public enum TargetState
	{
		Locked = 0,
		NotAchieved = 1,
		Achieved = 2
	}

	public UISprite targetSprite;

	public GameObject shine;

	private TargetState m_argetState;

	public string notAchievedSpriteName;

	public string achievedSpriteName;

	public AudioClip targetStart;

	public AudioClip targetHit;

	private bool animationCompleted;

	public void SetTargetState(TargetState newState)
	{
		m_argetState = newState;
		switch (m_argetState)
		{
		case TargetState.Achieved:
			targetSprite.spriteName = achievedSpriteName;
			targetSprite.alpha = 1f;
			targetSprite.depth = 2;
			shine.SetActive(true);
			break;
		case TargetState.NotAchieved:
			targetSprite.spriteName = notAchievedSpriteName;
			targetSprite.alpha = 0.8f;
			targetSprite.depth = 1;
			shine.SetActive(false);
			break;
		case TargetState.Locked:
			targetSprite.spriteName = notAchievedSpriteName;
			targetSprite.alpha = 0.8f;
			targetSprite.depth = 1;
			shine.SetActive(false);
			break;
		}
	}

	public void SetAchieved(bool isAchieved)
	{
		if (isAchieved)
		{
			targetSprite.spriteName = achievedSpriteName;
		}
		else
		{
			targetSprite.spriteName = notAchievedSpriteName;
		}
	}

	public void TriggerNewTargetAnimation(float duration)
	{
		TweenParms p_parms = new TweenParms().Prop("localScale", 1.3f * base.transform.localScale, true).Loops(1).Ease(EaseType.EaseInCubic);
		HOTween.From(base.transform, duration, p_parms);
		TweenParms tweenParms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 360f), true).Loops(1).Ease(EaseType.Linear);
		tweenParms.OnComplete(AnimationCompleted, AudioManager.Instance.NGPlay(targetStart));
		HOTween.To(targetSprite.transform, duration, tweenParms);
		targetSprite.depth = 5;
	}

	private void AnimationCompleted(TweenEvent e)
	{
		targetSprite.depth = 2;
		((AudioSource)e.parms[0]).Stop();
		AudioManager.Instance.NGPlay(targetHit);
	}
}
