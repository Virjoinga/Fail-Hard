using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UITimer : MonoBehaviour
{
	public delegate void OnSkipClicked();

	private UIFilledSprite filledSprite;

	public TimeCountdownLabel label;

	private int timeLeft;

	public int TimerDuration
	{
		get
		{
			return label.TimerDuration;
		}
		set
		{
			label.TimerDuration = value;
		}
	}

	public DateTime OriginalStartTime
	{
		get
		{
			return label.OriginalStartTime;
		}
		set
		{
			label.OriginalStartTime = value;
		}
	}

	[method: MethodImpl(32)]
	public event OnSkipClicked SkipClicked;

	private void Start()
	{
		filledSprite = GetComponentInChildren<UIFilledSprite>();
	}

	private void Update()
	{
		timeLeft = (int)((double)TimerDuration - (DateTime.UtcNow - OriginalStartTime).TotalSeconds);
		filledSprite.fillAmount = (float)timeLeft / (float)TimerDuration;
	}

	private void Skip()
	{
		if (this.SkipClicked != null)
		{
			this.SkipClicked();
		}
	}
}
