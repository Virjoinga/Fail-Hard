using System;
using UnityEngine;

public class TimeCountdownLabel : MonoBehaviour
{
	private UILabel label;

	public int TimerDuration { get; set; }

	public DateTime OriginalStartTime { get; set; }

	public int TimeLeft { get; set; }

	private void Start()
	{
		label = GetComponent<UILabel>();
		TimeLeft = (int)((double)TimerDuration - (DateTime.UtcNow - OriginalStartTime).TotalSeconds);
	}

	private void Update()
	{
		TimeLeft = (int)((double)TimerDuration - (DateTime.UtcNow - OriginalStartTime).TotalSeconds);
		int num = TimeLeft / 3600;
		label.text = num.ToString("D2") + ":" + (TimeLeft % 3600 / 60).ToString("D2") + ":" + (TimeLeft % 60).ToString("D2");
	}
}
