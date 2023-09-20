using UnityEngine;
using UnityEngine.UI;

namespace Valinta
{
	public class ValintaSelectable : MonoBehaviour
	{
		public VPlaylist AssignedPlaylist;

		[SerializeField]
		private Text PlaylistName;

		public void SetInfo(VPlaylist playlist)
		{
			AssignedPlaylist = playlist;
			PlaylistName.text = AssignedPlaylist.Name;
			base.gameObject.name = AssignedPlaylist.Name;
		}
	}
}
