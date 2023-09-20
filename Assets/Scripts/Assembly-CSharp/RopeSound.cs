using Game;
using UnityEngine;

public class RopeSound : MonoBehaviour
{
	public static float MIN_VELOCITY = 1.2f;

	public static int ROPE_SOUND_PROB = 7;

	public AudioSource ropeSource;

	public AudioClip[] ropeClips;

	private void Start()
	{
	}

	private void Update()
	{
		float magnitude = base.rigidbody.velocity.magnitude;
		if (magnitude > MIN_VELOCITY && Random.Range(0, ROPE_SOUND_PROB) == 0)
		{
			AudioManager.Instance.Play(ropeSource, ropeClips[Random.Range(0, ropeClips.Length)], 0.5f, AudioTag.AudienceAudio);
		}
	}
}
