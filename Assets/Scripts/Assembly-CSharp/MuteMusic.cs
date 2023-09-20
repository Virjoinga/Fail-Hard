using UnityEngine;

public class MuteMusic : MonoBehaviour
{
	public UISprite ButtonSprite;

	private AudioManager m_audioManager;

	private void Start()
	{
		m_audioManager = AudioManager.Instance;
		if (m_audioManager.IsMusicMuted())
		{
			ButtonSprite.spriteName = "music_off";
		}
	}

	private void OnClick()
	{
		if (m_audioManager.ToggleMusicMute())
		{
			ButtonSprite.spriteName = "music_off";
		}
		else
		{
			ButtonSprite.spriteName = "music_on";
		}
	}
}
