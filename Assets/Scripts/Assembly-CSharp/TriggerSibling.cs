using UnityEngine;

public class TriggerSibling : MonoBehaviour
{
	private Spawner spawner;

	private void Start()
	{
		spawner = base.transform.parent.gameObject.GetComponentInChildren<Spawner>();
	}

	private void OnTriggerEnter(Collider hit)
	{
		spawner.DestroySpawned(hit.gameObject);
	}
}
