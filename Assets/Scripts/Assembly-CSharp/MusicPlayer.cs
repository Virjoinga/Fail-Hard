using Game;
using Game.Progress;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public enum MusicTheme
	{
		None = 0,
		Beach = 1,
		City = 2,
		Garage = 3,
		Gravelpit = 4,
		Suburb = 5,
		Winter = 6,
		Jungle = 7
	}

	public AudioClip beachClip;

	public AudioClip cityClip;

	public AudioClip garageClip;

	public AudioClip suburbClip;

	public AudioClip gravelpitClip;

	public AudioClip winterClip;

	public AudioClip jungleClip;

	public AudioSource musicSource;

	private AudioClip nextClip;

	private MusicTheme currentTheme;

	private bool changingMusic;

	public static float FADE_SPEED = 0.9f;

	private AudioManager m_audioManager;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		m_audioManager = AudioManager.Instance;
	}

	public void setTheme(MusicTheme theme)
	{
		if (theme != currentTheme)
		{
			currentTheme = theme;
			changingMusic = true;
			switch (currentTheme)
			{
			case MusicTheme.Beach:
				nextClip = beachClip;
				break;
			case MusicTheme.City:
				nextClip = cityClip;
				break;
			case MusicTheme.Garage:
				nextClip = garageClip;
				break;
			case MusicTheme.Suburb:
				nextClip = suburbClip;
				break;
			case MusicTheme.Gravelpit:
				nextClip = gravelpitClip;
				break;
			case MusicTheme.Winter:
				nextClip = winterClip;
				break;
			case MusicTheme.Jungle:
				nextClip = jungleClip;
				break;
			case MusicTheme.None:
				nextClip = null;
				break;
			}
		}
	}

	private void Update()
	{
		if (m_audioManager.MusicMuted)
		{
			setTheme(MusicTheme.None);
		}
		else if (GameController.Instance.state() == GameController.GameState.Menu)
		{
			setTheme(MusicTheme.Garage);
		}
		else
		{
			switch (GameController.Instance.MyCurrentTheme)
			{
			case ThemeCategory.Movie:
				setTheme(MusicTheme.Beach);
				break;
			case ThemeCategory.ShoppingMall:
				setTheme(MusicTheme.City);
				break;
			case ThemeCategory.Backyard:
				setTheme(MusicTheme.Suburb);
				break;
			case ThemeCategory.PrivateEvents:
				setTheme(MusicTheme.Gravelpit);
				break;
			case ThemeCategory.Charity:
				setTheme(MusicTheme.Winter);
				break;
			case ThemeCategory.Jungle:
				setTheme(MusicTheme.Jungle);
				break;
			default:
				setTheme(MusicTheme.Winter);
				break;
			}
		}
		if (!changingMusic)
		{
			return;
		}
		if (musicSource.clip == null)
		{
			changingMusic = false;
			musicSource.clip = nextClip;
			if (nextClip == null)
			{
				musicSource.Stop();
			}
			else
			{
				m_audioManager.Play(musicSource, AudioTag.BackgroundMusic);
			}
			return;
		}
		musicSource.volume -= Time.deltaTime * FADE_SPEED;
		if (musicSource.volume <= 0f)
		{
			changingMusic = false;
			m_audioManager.ReMix(musicSource, 1f, AudioTag.BackgroundMusic);
			musicSource.clip = nextClip;
			m_audioManager.Play(musicSource, AudioTag.BackgroundMusic);
			nextClip = null;
		}
	}
}
