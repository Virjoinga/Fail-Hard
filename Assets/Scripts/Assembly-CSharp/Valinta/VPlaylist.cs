using System.Collections.Generic;
using UnityEngine;

namespace Valinta
{
	public class VPlaylist
	{
		private List<VSong> m_songs;

		private int m_currentSong;

		public int Id { get; private set; }

		public string Name { get; private set; }

		public VPlaylist(int id, string name)
		{
			Id = id;
			Name = name;
			m_songs = new List<VSong>();
			m_currentSong = 0;
		}

		public void AddSong(VSong song)
		{
			if (m_songs == null)
			{
				m_songs = new List<VSong>();
			}
			if (!m_songs.Contains(song))
			{
				m_songs.Add(song);
			}
			else
			{
				Debug.Log("Song already in list");
			}
		}

		public VSong GetNextSong()
		{
			if (m_currentSong >= m_songs.Count)
			{
				m_currentSong = 0;
			}
			VSong result = m_songs[m_currentSong];
			m_currentSong++;
			return result;
		}

		public void Clear()
		{
			m_songs.Clear();
		}
	}
}
