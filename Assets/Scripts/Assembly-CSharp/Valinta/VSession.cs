using System;
using System.Collections.Generic;

namespace Valinta
{
	internal class VSession
	{
		internal static VSession Instance;

		private string m_userId;

		private string m_appId;

		private double m_sessionStartTime;

		private double m_sessionEndTime;

		private float m_pausedTime;

		private List<VSong> m_skippedSongs;

		public string SessionData;

		public VSession(string appId, string uid)
		{
			if (Instance == null)
			{
				Instance = this;
			}
			m_skippedSongs = new List<VSong>();
			m_pausedTime = 0f;
			m_appId = appId;
			m_userId = uid;
			UpdateStartTime();
		}

		public void Stop()
		{
			m_sessionEndTime = ConvertToTimestamp(DateTime.UtcNow);
		}

		public void UpdateStartTime()
		{
			m_sessionStartTime = ConvertToTimestamp(DateTime.UtcNow);
		}

		public double GetStartTime()
		{
			return m_sessionStartTime;
		}

		public double GetEndTime()
		{
			if (m_sessionEndTime > 0.0)
			{
				return m_sessionEndTime;
			}
			return ConvertToTimestamp(DateTime.UtcNow);
		}

		public void AddSkippedSong(VSong song)
		{
			m_skippedSongs.Add(song);
		}

		public List<VSong> GetSkippedSongList()
		{
			return m_skippedSongs;
		}

		public void AddToPausedTime(float t)
		{
			m_pausedTime += t;
		}

		public int GetPausedTime()
		{
			return (int)m_pausedTime;
		}

		public void Reset()
		{
			m_skippedSongs.Clear();
			UpdateStartTime();
		}

		private double ConvertToTimestamp(DateTime value)
		{
			return (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
		}
	}
}
