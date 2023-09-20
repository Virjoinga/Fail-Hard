using Game;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
	private bool m_animating;

	private bool m_colliding;

	public AudioClip SoundEffect;

	public float SoundTriggerVelocity;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		m_colliding = false;
		if (Vector3.Dot(base.transform.right, base.rigidbody.velocity) > SoundTriggerVelocity)
		{
			AudioManager.Instance.Play(base.audio, SoundEffect, 1f, AudioTag.ItemCrashAudio2);
		}
	}

	private void LateUpdate()
	{
		if (!m_colliding)
		{
			base.rigidbody.isKinematic = false;
		}
	}

	private void OnCollisionEnter(Collision coll)
	{
		m_colliding = true;
		if (!m_animating)
		{
			m_animating = true;
			Invoke("FreezeTrampoline", 3f);
		}
	}

	private void OnCollisionStay(Collision coll)
	{
		m_colliding = true;
	}

	private void FreezeTrampoline()
	{
		base.rigidbody.isKinematic = true;
		m_animating = false;
	}
}
