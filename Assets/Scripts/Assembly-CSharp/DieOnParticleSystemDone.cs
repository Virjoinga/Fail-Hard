using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DieOnParticleSystemDone : MonoBehaviour
{
	private ParticleSystem m_system;

	private void Start()
	{
		m_system = GetComponent<ParticleSystem>();
		StartCoroutine(CheckIfAlive());
	}

	private IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (!m_system.IsAlive(true))
			{
				GOTools.Despawn(base.gameObject);
			}
		}
	}
}
