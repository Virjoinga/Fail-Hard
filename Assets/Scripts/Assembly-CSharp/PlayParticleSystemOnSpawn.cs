using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystemOnSpawn : MonoBehaviour
{
	public ParticleSystem System;

	public void Start()
	{
		if (System == null)
		{
			List<ParticleSystem> list = GOTools.FindAllComponents<ParticleSystem>(base.gameObject);
			if (list.Count > 0)
			{
				System = list[0];
			}
		}
		if (System == null)
		{
			GOTools.Despawn(base.gameObject);
		}
	}

	public void OnSpawned()
	{
		if (System != null)
		{
			System.Play(true);
		}
	}
}
