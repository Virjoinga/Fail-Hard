using System.Collections.Generic;
using Game;
using UnityEngine;

public class TreeCollision : MonoBehaviour
{
	private ParticleSystem[] leafEffects;

	private float timer;

	private static float EFFECT_INTERVAL = 5f;

	private static float ACTIVE_SYSTEMS = 3f;

	private bool activated;

	private Vector3 startRotation;

	public Transform pivotPoint;

	public AudioSource treeSource;

	private void Start()
	{
		timer = 0f;
		leafEffects = base.gameObject.GetComponentsInChildren<ParticleSystem>();
	}

	private void Update()
	{
		if (activated)
		{
			timer += Time.deltaTime;
			float num = 0f;
			if (timer < 0.2f)
			{
				num = 20f;
			}
			if (timer >= 0.2f && timer < 0.4f)
			{
				num = -20f;
			}
			if (num != 0f)
			{
				base.gameObject.transform.RotateAround(pivotPoint.position, new Vector3(1f, 0f, 0f), Time.deltaTime * num);
			}
		}
		if (timer >= EFFECT_INTERVAL)
		{
			activated = false;
			timer = 0f;
		}
	}

	private void emitParticles(Collision collision)
	{
		activated = true;
		AudioManager.Instance.Play(treeSource, AudioTag.Other);
		List<ParticleSystem> list = new List<ParticleSystem>();
		do
		{
			ParticleSystem item = leafEffects[Random.Range(0, leafEffects.Length)];
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		while ((float)list.Count < ACTIVE_SYSTEMS);
		foreach (ParticleSystem item2 in list)
		{
			item2.Play();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!activated && collision.relativeVelocity.magnitude > 5f)
		{
			emitParticles(collision);
		}
	}
}
