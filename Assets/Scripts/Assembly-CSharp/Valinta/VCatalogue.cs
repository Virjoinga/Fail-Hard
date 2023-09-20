using System.Collections.Generic;

namespace Valinta
{
	public class VCatalogue
	{
		private List<VPlaylist> m_playlists;

		public VCatalogue()
		{
			m_playlists = new List<VPlaylist>();
		}

		public void AddPlaylist(VPlaylist playlist)
		{
			if (!m_playlists.Contains(playlist))
			{
				m_playlists.Add(playlist);
			}
		}

		public VPlaylist GetPlaylistByID(int id)
		{
			foreach (VPlaylist playlist in m_playlists)
			{
				if (playlist.Id == id)
				{
					return playlist;
				}
			}
			return null;
		}

		public VPlaylist GetPlaylistByIndex(int index)
		{
			if (index < m_playlists.Count)
			{
				return m_playlists[index];
			}
			return m_playlists[0];
		}

		public List<VPlaylist> GetAllPlaylists()
		{
			return m_playlists;
		}

		public void RemovePlaylist(int index)
		{
			m_playlists.RemoveAt(index);
		}

		public void ClearCatalogue()
		{
			m_playlists.Clear();
		}
	}
}
