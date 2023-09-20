using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Valinta
{
	public class VUI : MonoBehaviour
	{
		[SerializeField]
		protected GameObject m_playlistPrefab;

		[SerializeField]
		protected RectTransform m_playlistParent;

		[SerializeField]
		protected VPlayerStatus m_playerStatus;

		[SerializeField]
		protected VButtonHandler m_playerButtons;

		[SerializeField]
		protected VVolume m_volume;

		[SerializeField]
		protected GameObject m_playerWindow;

		[SerializeField]
		protected GameObject m_playlistWindow;

		[SerializeField]
		protected Button m_playerToggle;

		[SerializeField]
		protected Image m_imagePlayerToggle;

		[SerializeField]
		protected Sprite m_spritePlayerOn;

		[SerializeField]
		protected Sprite m_spritePlayerOff;

		protected bool isPlayerWindowVisible;

		protected bool isPlaylistWindowVisible;

		protected bool isFirstRun = true;

		protected virtual void Awake()
		{
			m_playerButtons.OnButtonClick += OnControlsClicked;
			ValintaMain.OnPlayerPaused += OnPlayerPaused;
			ValintaMain.OnStatusChanged += OnStatusChanged;
			ValintaMain.OnPlayerError += OnPlayerError;
			ValintaMain.OnPlayerLocked += OnPlayerLocked;
		}

		protected virtual void Start()
		{
			m_playerToggle.onClick.AddListener(TogglePlayerWindow);
		}

		protected virtual void OnDestroy()
		{
			m_playerButtons.OnButtonClick -= OnControlsClicked;
			ValintaMain.OnPlayerPaused -= OnPlayerPaused;
			ValintaMain.OnStatusChanged -= OnStatusChanged;
			ValintaMain.OnPlayerError -= OnPlayerError;
			ValintaMain.OnPlayerLocked -= OnPlayerLocked;
		}

		public virtual void Initialize()
		{
			CreatePlaylists();
		}

		private void CreatePlaylists()
		{
			List<VPlaylist> allPlaylists = ValintaMain.Instance.GetCatalogue().GetAllPlaylists();
			for (int i = 0; i < allPlaylists.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(m_playlistPrefab) as GameObject;
				if (m_playlistParent != null)
				{
					gameObject.transform.SetParent(m_playlistParent, false);
				}
				ValintaSelectable playlistInfo = gameObject.GetComponent<ValintaSelectable>();
				playlistInfo.SetInfo(allPlaylists[i]);
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					ValintaMain.Instance.Play(playlistInfo.AssignedPlaylist);
					SetPlaylistWindowVisible(false);
				});
			}
		}

		private void RemovePlaylists()
		{
			foreach (Transform item in m_playlistParent.transform)
			{
				Object.Destroy(item.gameObject);
			}
		}

		private void TogglePlayerWindow()
		{
			if (m_playlistParent.childCount <= 0)
			{
				RemovePlaylists();
				CreatePlaylists();
			}
			isPlayerWindowVisible = !isPlayerWindowVisible;
			m_playerWindow.SetActive(isPlayerWindowVisible);
			m_imagePlayerToggle.sprite = ((!isPlayerWindowVisible) ? m_spritePlayerOff : m_spritePlayerOn);
			if (isFirstRun)
			{
				TogglePlaylistWindow();
				isFirstRun = false;
			}
			else if (m_playlistWindow.activeInHierarchy)
			{
				TogglePlaylistWindow();
			}
		}

		public virtual void TogglePlaylistWindow()
		{
			isPlaylistWindowVisible = !isPlaylistWindowVisible;
			SetPlaylistWindowVisible(isPlaylistWindowVisible);
		}

		public void SetPlaylistWindowVisible(bool visible)
		{
			isPlaylistWindowVisible = visible;
			m_playlistWindow.SetActive(isPlaylistWindowVisible);
		}

		private void OnControlsClicked(ButtonType type)
		{
			if (ValintaMain.Instance.CurrentPlayerState == ValintaMain.PlayerState.Loading)
			{
				return;
			}
			switch (type)
			{
			case ButtonType.Play:
				ValintaMain.Instance.Play();
				SetPlaylistWindowVisible(false);
				if (m_volume != null)
				{
					m_volume.MuteValinta(false);
				}
				break;
			case ButtonType.Pause:
				ValintaMain.Instance.Pause();
				break;
			case ButtonType.Skip:
				ValintaMain.Instance.Skip();
				SetPlaylistWindowVisible(false);
				break;
			case ButtonType.Menu:
				TogglePlaylistWindow();
				break;
			case ButtonType.Share:
				ValintaMain.Instance.Share();
				break;
			case ButtonType.Status:
				OpenURLInBrowser();
				break;
			}
		}

		public virtual void OnLinkClicked(string url)
		{
			if (!string.IsNullOrEmpty(url))
			{
				Application.OpenURL(url);
			}
		}

		private void OpenURLInBrowser()
		{
			if (!ValintaMain.Instance.AdInProgress)
			{
				if (ValintaMain.Instance.CurrentSong != null)
				{
					string site = ValintaMain.Instance.CurrentSong.Site;
					OnLinkClicked(site);
				}
			}
			else
			{
				StartCoroutine(WaitForAdTracking());
			}
		}

		private IEnumerator WaitForAdTracking()
		{
			string site = ValintaMain.Instance.GetCurrentAdUrl();
			yield return new WaitForSeconds(0.3f);
			OnLinkClicked(site);
		}

		private void OnPlayerPaused(bool isPaused)
		{
			m_playerButtons.ChangeControlState(isPaused);
		}

		private void OnStatusChanged(string s)
		{
			m_playerStatus.UpdateText(s);
		}

		private void OnPlayerError()
		{
			m_playerButtons.ChangeControlState(true);
		}

		private void OnPlayerLocked(bool isLocked)
		{
			if (isLocked)
			{
				isPlaylistWindowVisible = false;
				m_playlistWindow.gameObject.SetActive(false);
				m_playerButtons.Disable();
			}
			else
			{
				m_playerButtons.Enable();
			}
		}
	}
}
