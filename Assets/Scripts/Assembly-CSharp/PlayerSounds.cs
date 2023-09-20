using Game;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
	public AudioSource playerSource;

	public AudioClip[] mildPain;

	public AudioClip[] mediumPain;

	public AudioClip[] hardPain;

	public AudioClip gameOver;

	public AudioClip[] jumpClips;

	public AudioSource windSource;

	private AudioClip lastPlayedClip;

	private AudioManager m_audioManager;

	public bool StuntActive { get; set; }

	private void Start()
	{
		m_audioManager = AudioManager.Instance;
	}

	private void Update()
	{
		if (!GameController.Instance.Character.Vehicle)
		{
			return;
		}
		if (!GameController.Instance.Character.Vehicle.IsGrounded())
		{
			m_audioManager.ReMix(windSource, windSource.volume - Time.deltaTime * 0.5f, AudioTag.Other);
			return;
		}
		float magnitude = base.rigidbody.velocity.magnitude;
		float newVolume = 0f;
		if (magnitude > 1f)
		{
			newVolume = magnitude / 9f - 0.5f;
		}
		m_audioManager.ReMix(windSource, newVolume, AudioTag.Other);
	}

	public void playJumpSound()
	{
		if (jumpClips.Length > 0 && (bool)playerSource)
		{
			AudioManager.Instance.Play(playerSource, jumpClips[Random.Range(0, jumpClips.Length)], 0.5f, AudioTag.PlayerAudio);
		}
	}

	public void playPainSound(int painLevel)
	{
		if (!StuntActive)
		{
			return;
		}
		AudioClip[] pain = null;
		if (painLevel == 0)
		{
			pain = mildPain;
		}
		if (painLevel == 1)
		{
			pain = mediumPain;
		}
		if (painLevel == 2)
		{
			pain = hardPain;
		}
		if (!playerSource.isPlaying)
		{
			AudioClip painSound = getPainSound(pain);
			if (painSound != null)
			{
				lastPlayedClip = painSound;
				AudioManager.Instance.Play(playerSource, painSound, 1f, AudioTag.PlayerAudio);
			}
		}
	}

	private AudioClip getPainSound(AudioClip[] pain)
	{
		if (pain.Length == 0)
		{
			return null;
		}
		AudioClip audioClip;
		do
		{
			int num = Random.Range(0, pain.Length);
			audioClip = pain[num];
		}
		while (audioClip == lastPlayedClip);
		return audioClip;
	}
}
