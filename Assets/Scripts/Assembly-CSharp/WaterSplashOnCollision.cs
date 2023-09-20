using Game;
using UnityEngine;

public class WaterSplashOnCollision : MonoBehaviour
{
	public AudioSource audioSource;

	public GameObject splashEffect;

	private void OnTriggerEnter(Collider hit)
	{
		if (hit.rigidbody != null && hit.rigidbody.velocity.y < -2f && hit.rigidbody.mass > 1f)
		{
			if (this.audioSource != null)
			{
				AudioSource audioSource = AudioManager.Instance.PlayClipAt(this.audioSource.clip, base.transform.position, AudioTag.BigCrashAudio);
				audioSource.rolloffMode = AudioRolloffMode.Linear;
			}
			PoolManager.Pools["Main"].Spawn(splashEffect.transform, hit.transform.position, base.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
		}
	}
}
