using UnityEngine;

public class PerfTimer
{
	private string m_timerName;

	private float m_startTime;

	private float m_endTime = -1f;

	private PerfTimer(float startTime, string name)
	{
		m_timerName = "Timer:[" + name + "] ";
		m_startTime = startTime;
	}

	public static PerfTimer Start(string name = null)
	{
		return new PerfTimer(Time.realtimeSinceStartup, name);
	}

	public void StopAndPrint()
	{
		m_endTime = Time.realtimeSinceStartup;
		Debug.Log(m_timerName + (m_endTime - m_startTime) * 1000f + "ms");
	}
}
