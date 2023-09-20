using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Valinta
{
	public class VButtonHandler : MonoBehaviour
	{
		public delegate void VButtonDelegate(ButtonType type);

		[SerializeField]
		private Button m_buttonPlay;

		[SerializeField]
		private Button m_buttonSkip;

		[SerializeField]
		private Button m_buttonMenu;

		[SerializeField]
		private Button m_buttonShare;

		[SerializeField]
		private Button m_buttonStatus;

		private Image m_imagePlayButton;

		[SerializeField]
		private Sprite m_spritePlay;

		[SerializeField]
		private Sprite m_spritePause;

		private bool isPaused = true;

		[method: MethodImpl(32)]
		public event VButtonDelegate OnButtonClick;

		private void Awake()
		{
			m_imagePlayButton = m_buttonPlay.GetComponent<Image>();
		}

		private void Start()
		{
			m_buttonPlay.onClick.AddListener(OnPlayClicked);
			m_buttonSkip.onClick.AddListener(OnSkipClicked);
			m_buttonMenu.onClick.AddListener(OnMenuClicked);
			m_buttonStatus.onClick.AddListener(OnStatusClicked);
		}

		public void Show()
		{
			ActivateButtons(true);
		}

		public void Hide()
		{
			ActivateButtons(false);
		}

		public void Disable()
		{
			SetButtonsInteractable(false);
		}

		public void Enable()
		{
			SetButtonsInteractable(true);
		}

		private void ActivateButtons(bool activated)
		{
			m_buttonPlay.gameObject.SetActive(activated);
			m_buttonSkip.gameObject.SetActive(activated);
			m_buttonMenu.gameObject.SetActive(activated);
		}

		private void SetButtonsInteractable(bool interactable)
		{
			m_buttonPlay.interactable = interactable;
			m_buttonSkip.interactable = interactable;
			m_buttonMenu.interactable = interactable;
		}

		public void ChangeControlState(bool isPlayerPaused)
		{
			isPaused = isPlayerPaused;
			m_imagePlayButton.sprite = ((!isPlayerPaused) ? m_spritePause : m_spritePlay);
		}

		private void OnPlayClicked()
		{
			isPaused = !isPaused;
			if (isPaused)
			{
				Send(ButtonType.Pause);
			}
			else
			{
				Send(ButtonType.Play);
			}
		}

		private void OnSkipClicked()
		{
			Send(ButtonType.Skip);
		}

		private void OnMenuClicked()
		{
			Send(ButtonType.Menu);
		}

		private void OnShareClicked()
		{
			Send(ButtonType.Share);
		}

		private void OnStatusClicked()
		{
			Send(ButtonType.Status);
		}

		private void Send(ButtonType type)
		{
			if (this.OnButtonClick != null)
			{
				this.OnButtonClick(type);
			}
		}
	}
}
