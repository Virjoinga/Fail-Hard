using UnityEngine;

public class PerformanceChecker : MonoBehaviour
{
	public enum PerformanceState
	{
		Unknown = 100,
		Bad = 1,
		Good = 2
	}

	private class PerformanceDecider
	{
		private int m_badIntervals;

		private int m_goodIntervals;

		public void AddState(PerformanceState state)
		{
			switch (state)
			{
			case PerformanceState.Good:
				m_goodIntervals++;
				break;
			case PerformanceState.Bad:
				m_badIntervals++;
				break;
			default:
				Debug.LogWarning("Trying to add Uknown performance state...");
				break;
			}
		}

		public PerformanceState OverallState()
		{
			if (m_badIntervals + m_goodIntervals < 5)
			{
				return PerformanceState.Unknown;
			}
			float num = (float)m_badIntervals / (float)m_goodIntervals;
			if (num < 0.2f)
			{
				return PerformanceState.Good;
			}
			return PerformanceState.Bad;
		}
	}

	private class FrameCounter
	{
		private int m_frames;

		private float m_startTime = -1f;

		private float m_timeLeft = -1f;

		private float m_accum;

		private PerformanceState m_state;

		public void Start(float interval)
		{
			m_frames = 0;
			m_accum = 0f;
			m_state = PerformanceState.Unknown;
			m_startTime = Time.realtimeSinceStartup;
			m_timeLeft = interval;
		}

		public bool IsRunning()
		{
			return m_startTime > 0f;
		}

		public void Update()
		{
			m_frames++;
			m_timeLeft -= Time.deltaTime;
			m_accum += Time.timeScale / Time.deltaTime;
			if (m_timeLeft <= 0f)
			{
				CheckState();
			}
		}

		public PerformanceState State()
		{
			return m_state;
		}

		private void CheckState()
		{
			float num = m_accum / (float)m_frames;
			if (num < 27f)
			{
				m_state = PerformanceState.Bad;
			}
			else
			{
				m_state = PerformanceState.Good;
			}
		}
	}

	public const float FPS_THRESHOLD = 27f;

	public const float BAD_INTERVAL_THRESHOLD = 0.2f;

	public const float CHECK_INTERVAL = 2f;

	private static PerformanceChecker s_instance;

	private FrameCounter m_counter = new FrameCounter();

	private PerformanceDecider m_decider = new PerformanceDecider();

	public static PerformanceChecker Instance
	{
		get
		{
			return s_instance;
		}
	}

	public PerformanceState CurrentState()
	{
		return m_decider.OverallState();
	}

	private void Awake()
	{
		s_instance = this;
	}

	private void OnEnable()
	{
		m_counter.Start(2f);
	}

	private void Update()
	{
		m_counter.Update();
		PerformanceState performanceState = m_counter.State();
		if (performanceState != PerformanceState.Unknown)
		{
			m_decider.AddState(performanceState);
			m_counter.Start(2f);
		}
	}
}
