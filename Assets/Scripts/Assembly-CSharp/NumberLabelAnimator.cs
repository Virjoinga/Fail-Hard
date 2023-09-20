using Game;
using UnityEngine;

public class NumberLabelAnimator : MonoBehaviour
{
	public int animationInterval;

	public int targetValue;

	public int currentValue;

	public string Postfix = string.Empty;

	public bool AlwaysSkip;

	private UILabel label;

	private bool animating;

	public AudioClip loopingSound;

	public AudioClip endSound;

	private int m_prevValue;

	private void Start()
	{
		label = GetComponent<UILabel>();
		animating = false;
	}

	private void Update()
	{
		if (AlwaysSkip)
		{
			currentValue = targetValue;
		}
		else if (targetValue > currentValue)
		{
			currentValue += Mathf.CeilToInt(0.3f * (float)(targetValue - currentValue));
			if (!animating)
			{
				ChangeAnimationState(true);
			}
		}
		else if (targetValue < currentValue)
		{
			currentValue -= Mathf.CeilToInt(0.3f * (float)(currentValue - targetValue));
			if (!animating)
			{
				ChangeAnimationState(true);
			}
		}
		else if (animating)
		{
			ChangeAnimationState(false);
		}
		if (m_prevValue != currentValue)
		{
			m_prevValue = currentValue;
			label.text = currentValue + Postfix;
		}
	}

	private void ChangeAnimationState(bool newState)
	{
		if (newState == animating)
		{
			return;
		}
		animating = newState;
		if (animating && base.audio != null)
		{
			if (loopingSound != null)
			{
				base.audio.clip = loopingSound;
				base.audio.loop = true;
				AudioManager.Instance.Play(base.audio, AudioTag.Other);
			}
		}
		else if (base.audio != null && base.audio.isPlaying && endSound != null)
		{
			base.audio.clip = endSound;
			base.audio.loop = false;
			AudioManager.Instance.Play(base.audio, AudioTag.Other);
		}
	}

	public void SkipTo(int newValue, bool withEndSound)
	{
		currentValue = newValue;
		targetValue = newValue;
		animating = false;
		if (withEndSound && base.audio != null && base.audio.isPlaying)
		{
			base.audio.Stop();
			if (withEndSound && endSound != null)
			{
				base.audio.clip = endSound;
				base.audio.loop = false;
				AudioManager.Instance.Play(base.audio, AudioTag.Other);
			}
		}
	}
}
