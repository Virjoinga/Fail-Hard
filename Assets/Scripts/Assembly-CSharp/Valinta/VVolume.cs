using UnityEngine;
using UnityEngine.UI;

namespace Valinta
{
	public class VVolume : MonoBehaviour
	{
		private AudioSource m_valintaAudioSource;

		[SerializeField]
		private Button m_buttonVolumeUp;

		[SerializeField]
		private Button m_buttonVolumeDown;

		[SerializeField]
		private Text m_volumeText;

		[SerializeField]
		private float VolumeStep = 0.1f;

		[SerializeField]
		private bool PlayGameMusicWhenValintaMuted;

		[SerializeField]
		private AudioSource[] m_mutedAudioSources;

		private void Start()
		{
			m_valintaAudioSource = ValintaMain.Instance.GetAudioSource();
			m_buttonVolumeUp.onClick.AddListener(VolumeUp);
			m_buttonVolumeDown.onClick.AddListener(VolumeDown);
		}

		public void MuteValinta(bool mute)
		{
			m_valintaAudioSource.mute = mute;
			if (PlayGameMusicWhenValintaMuted || !mute)
			{
				AudioSource[] mutedAudioSources = m_mutedAudioSources;
				foreach (AudioSource audioSource in mutedAudioSources)
				{
					audioSource.mute = !mute;
				}
			}
		}

		private void AdjustVolume(float adjustBy)
		{
			m_valintaAudioSource.volume += adjustBy;
			if (m_valintaAudioSource.volume < 0f)
			{
				m_valintaAudioSource.volume = 0f;
			}
			if (m_valintaAudioSource.volume > 1f)
			{
				m_valintaAudioSource.volume = 1f;
			}
			float num = Mathf.Round(m_valintaAudioSource.volume * 100f);
			m_volumeText.text = num.ToString();
		}

		private void VolumeUp()
		{
			AdjustVolume(VolumeStep);
		}

		private void VolumeDown()
		{
			AdjustVolume(0f - VolumeStep);
		}
	}
}
