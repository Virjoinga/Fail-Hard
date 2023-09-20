using Game;
using UnityEngine;

public class DefaultCollisionSound : MonoBehaviour
{
	public AudioSource defaulCollisionSource;

	public AudioClip[] defaultCollisionClips;

	public float velocityThreshold;

	public static float THRESHOLD = 5f;

	private void OnCollisionEnter(Collision hit)
	{
		if (hit.gameObject.GetComponent<CollisionSound>() == null && defaultCollisionClips.Length > 0 && hit.relativeVelocity.magnitude > velocityThreshold)
		{
			AudioClip clip = defaultCollisionClips[Random.Range(0, defaultCollisionClips.Length)];
			AudioManager.Instance.Play(defaulCollisionSource, clip, 0.5f, AudioTag.BigCrashAudio);
		}
	}
}
