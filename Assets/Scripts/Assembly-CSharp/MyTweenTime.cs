using UnityEngine;

public class MyTweenTime : UITweener
{
	public float from = 1f;

	public float to = 1f;

	public float time
	{
		get
		{
			return Time.timeScale;
		}
		set
		{
			Time.timeScale = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		time = Mathf.Lerp(from, to, factor);
	}

	public static MyTweenTime Begin(GameObject go, float duration, float time)
	{
		MyTweenTime myTweenTime = UITweener.Begin<MyTweenTime>(go, duration);
		myTweenTime.from = myTweenTime.time;
		myTweenTime.to = time;
		if (duration <= 0f)
		{
			myTweenTime.Sample(1f, true);
			myTweenTime.enabled = false;
		}
		return myTweenTime;
	}
}
