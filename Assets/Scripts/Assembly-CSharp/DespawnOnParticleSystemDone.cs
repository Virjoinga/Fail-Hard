using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnOnParticleSystemDone : MonoBehaviour
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
		if (System != null)
		{
			StartCoroutine(CheckIfAlive());
		}
		else
		{
			GOTools.Despawn(base.gameObject);
		}
	}

	private IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (!System.IsAlive(true))
			{
				GOTools.Despawn(base.gameObject);
			}
		}
	}
}
