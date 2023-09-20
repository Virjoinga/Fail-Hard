using System.Collections.Generic;
using Game;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private List<AudioPlaybackEvent> nowPlaying;

	public static float UIAudioVolume;

	private MixingInfo[] mixingInfo = new MixingInfo[13];

	public AudioSource uiAudioSource;

	public AudioSource[] audioSources;

	private int selectedSource;

	private static AudioManager instance;

	private static bool initialized;

	public bool SfxMuted { get; set; }

	public bool MusicMuted { get; set; }

	public static AudioManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = GameObject.Find("AudioManager");
				if (gameObject == null)
				{
					Debug.LogWarning("No audio manager in scene! Creating one...");
					gameObject = Object.Instantiate(Resources.Load("AudioManager"), Vector3.zero, Quaternion.identity) as GameObject;
				}
				instance = gameObject.GetComponent<AudioManager>();
			}
			return instance;
		}
		private set
		{
		}
	}

	private void Awake()
	{
		if (initialized)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		initialized = true;
		Object.DontDestroyOnLoad(base.gameObject);
		nowPlaying = new List<AudioPlaybackEvent>();
		for (int i = 0; i < 13; i++)
		{
			mixingInfo[i] = new MixingInfo();
		}
		initializeMixingInfo();
		Object.Instantiate(Resources.Load("MusicPlayer"), Vector3.zero, Quaternion.identity);
	}

	private void Start()
	{
		int @int = PlayerPrefs.GetInt("MuteSfx");
		if (@int > 0)
		{
			ToggleSFXMute();
		}
		ToggleMusicMute();
	}

	private void initializeMixingInfo()
	{
		mixingInfo[6].VolumeFactor = 0.8f;
		mixingInfo[4].VolumeFactor = 0.5f;
		mixingInfo[1].Type = Game.AudioType.Music;
	}

	public AudioSource PlayClipAt(AudioClip clip, Vector3 pos, AudioTag tag)
	{
		GameObject gameObject = new GameObject("TempAudio");
		gameObject.transform.position = pos;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		Play(audioSource, clip, 0.5f, tag);
		Object.Destroy(gameObject, clip.length);
		return audioSource;
	}

	private void MixSource(AudioSource source, AudioTag tag)
	{
		mixingInfo[(int)tag].Mix(source);
	}

	public void Play(AudioSource source, AudioTag tag)
	{
		MixSource(source, tag);
		source.Play();
	}

	public AudioSource NGPlay(AudioClip clip)
	{
		if (clip != null)
		{
			MixSource(uiAudioSource, AudioTag.UIAudio);
			if (uiAudioSource.isPlaying)
			{
				uiAudioSource.Stop();
			}
			uiAudioSource.clip = clip;
			uiAudioSource.Play();
		}
		else
		{
			Debug.LogWarning("Trying to play nonexistent audio clip!");
		}
		return uiAudioSource;
	}

	public void Play(AudioClip clip, AudioTag tag)
	{
		if (clip != null)
		{
			MixSource(audioSources[selectedSource], AudioTag.UIAudio);
			if (audioSources[selectedSource].isPlaying)
			{
				audioSources[selectedSource].Stop();
			}
			audioSources[selectedSource].clip = clip;
			audioSources[selectedSource].Play();
			selectedSource = ++selectedSource % audioSources.Length;
		}
		else
		{
			Debug.LogWarning("Trying to play nonexistent audio clip!");
		}
	}

	public void Play(AudioSource source, AudioClip clip, float filteringTime, AudioTag tag)
	{
		MixSource(source, tag);
		if (!source.enabled)
		{
			return;
		}
		bool flag = true;
		foreach (AudioPlaybackEvent item in nowPlaying)
		{
			if (item.clipName == clip.name && Time.time < item.filterTime)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			AudioPlaybackEvent audioPlaybackEvent = new AudioPlaybackEvent();
			audioPlaybackEvent.clipName = clip.name;
			audioPlaybackEvent.filterTime = Time.time + filteringTime;
			nowPlaying.Add(audioPlaybackEvent);
			source.clip = clip;
			source.Play();
		}
		Cleanup();
	}

	public void ReMix(AudioSource source, float newVolume, AudioTag tag)
	{
		mixingInfo[(int)tag].Volume = newVolume;
		MixSource(source, tag);
	}

	public bool ToggleSFXMute()
	{
		if (SfxMuted)
		{
			MixingInfo[] array = this.mixingInfo;
			foreach (MixingInfo mixingInfo in array)
			{
				if (mixingInfo.Type == Game.AudioType.Sfx)
				{
					mixingInfo.UnMute();
				}
			}
			SfxMuted = false;
			NGUITools.soundVolume = this.mixingInfo[2].Volume;
			PlayerPrefs.SetInt("MuteSfx", 0);
		}
		else
		{
			MixingInfo[] array2 = this.mixingInfo;
			foreach (MixingInfo mixingInfo2 in array2)
			{
				if (mixingInfo2.Type == Game.AudioType.Sfx)
				{
					mixingInfo2.Mute();
				}
			}
			SfxMuted = true;
			NGUITools.soundVolume = this.mixingInfo[2].Volume;
			PlayerPrefs.SetInt("MuteSfx", 1);
		}
		PlayerPrefs.Save();
		return SfxMuted;
	}

	public bool ToggleMusicMute()
	{
		if (MusicMuted)
		{
			MixingInfo[] array = this.mixingInfo;
			foreach (MixingInfo mixingInfo in array)
			{
				if (mixingInfo.Type == Game.AudioType.Music)
				{
					mixingInfo.UnMute();
				}
			}
			MusicMuted = false;
			PlayerPrefs.SetInt("MuteMusic", 0);
		}
		else
		{
			MixingInfo[] array2 = this.mixingInfo;
			foreach (MixingInfo mixingInfo2 in array2)
			{
				if (mixingInfo2.Type == Game.AudioType.Music)
				{
					mixingInfo2.Mute();
				}
			}
			MusicMuted = true;
			PlayerPrefs.SetInt("MuteMusic", 1);
		}
		PlayerPrefs.Save();
		return MusicMuted;
	}

	public bool IsSFXMuted()
	{
		return SfxMuted;
	}

	public bool IsMusicMuted()
	{
		return MusicMuted;
	}

	public void Cleanup()
	{
		int num = 0;
		while (num < nowPlaying.Count)
		{
			AudioPlaybackEvent audioPlaybackEvent = nowPlaying[num];
			if (Time.time >= audioPlaybackEvent.filterTime)
			{
				nowPlaying.Remove(audioPlaybackEvent);
			}
			else
			{
				num++;
			}
		}
	}
}
