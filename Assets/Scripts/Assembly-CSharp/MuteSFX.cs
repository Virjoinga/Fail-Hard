using UnityEngine;

public class MuteSFX : MonoBehaviour
{
	public UISprite ButtonSprite;

	private AudioManager m_audioManager;

	private void Start()
	{
		m_audioManager = AudioManager.Instance;
		if (m_audioManager.IsSFXMuted())
		{
			ButtonSprite.spriteName = "sound_off";
		}
	}

	private void OnClick()
	{
		if (m_audioManager.ToggleSFXMute())
		{
			ButtonSprite.spriteName = "sound_off";
		}
		else
		{
			ButtonSprite.spriteName = "sound_on";
		}
	}
}
