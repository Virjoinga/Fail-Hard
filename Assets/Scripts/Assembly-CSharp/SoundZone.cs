using Game;
using UnityEngine;

public class SoundZone : MonoBehaviour
{
	public AudioSource zoneSource;

	public AudioClip[] zoneSounds;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter()
	{
		AudioManager.Instance.Play(zoneSource, zoneSounds[Random.Range(0, zoneSounds.Length)], 0.5f, AudioTag.AudienceAudio);
	}
}
