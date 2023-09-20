using Game;
using UnityEngine;

public class BoatEffect : MonoBehaviour
{
	public AudioSource boatSource;

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.gameObject.rigidbody.mass < 1f) && !(collision.gameObject.rigidbody.velocity.magnitude < 1f))
		{
			if (!base.transform.parent.animation.isPlaying)
			{
				base.transform.parent.animation.Play();
			}
			AudioManager.Instance.Play(boatSource, boatSource.clip, 2f, AudioTag.AudienceAudio);
		}
	}
}
