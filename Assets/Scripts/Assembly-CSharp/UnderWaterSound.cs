using Game;
using UnityEngine;

public class UnderWaterSound : MonoBehaviour
{
	public AudioSource underWaterSource;

	private bool isUnderWater;

	public GameObject bubble;

	public static int BUBBLE_PROB = 10;

	private void Update()
	{
		if (isUnderWater && (bool)bubble && Random.Range(0, BUBBLE_PROB) == 1)
		{
			Transform transform = PoolManager.Pools["Main"].Spawn(bubble.transform, base.transform.position, Quaternion.identity);
			if ((bool)transform)
			{
				transform.localScale *= Random.Range(0.3f, 0.8f);
			}
		}
	}

	private void EnterWater()
	{
		isUnderWater = true;
		AudioSource[] componentsInChildren = base.gameObject.transform.root.GetComponentsInChildren<AudioSource>();
		ParticleSystem[] componentsInChildren2 = base.gameObject.transform.root.GetComponentsInChildren<ParticleSystem>();
		AudioSource[] array = componentsInChildren;
		foreach (AudioSource audioSource in array)
		{
			audioSource.enabled = false;
		}
		ParticleSystem[] array2 = componentsInChildren2;
		foreach (ParticleSystem particleSystem in array2)
		{
			particleSystem.enableEmission = false;
		}
		underWaterSource.enabled = true;
		AudioManager.Instance.Play(underWaterSource, underWaterSource.clip, 0.5f, AudioTag.Other);
	}

	private void OnTriggerEnter(Collider hit)
	{
		if ((bool)hit.gameObject.GetComponent<WaterSplashOnCollision>())
		{
			EnterWater();
		}
	}
}
