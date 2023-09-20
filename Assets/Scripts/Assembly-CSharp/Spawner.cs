using System;
using System.Collections.Generic;
using UnityEngine;

[PBSerialize("Spawner")]
public class Spawner : MonoBehaviour
{
	[PBSerializeField]
	public string spawned;

	[PBSerializeField]
	public bool distanceCheck;

	[PBSerializeField]
	public float distance;

	[PBSerializeField]
	public bool timeCheck;

	[PBSerializeField]
	public bool useDestroyer;

	[PBSerializeField]
	public float delay;

	[PBSerializeField]
	public float spawnPeriod;

	[PBSerializeField]
	public float initialDelay;

	[PBSerializeField]
	public int simultaneousInstances;

	[PBSerializeField]
	public float randomization;

	private List<Transform> controlledObjects;

	private Queue<GameObject> objectPool;

	private Dictionary<GameObject, DateTime> timeTrackers;

	private void Start()
	{
		controlledObjects = new List<Transform>();
		timeTrackers = new Dictionary<GameObject, DateTime>();
		Invoke("Spawn", initialDelay);
		if (distanceCheck)
		{
			Invoke("DistanceCheck", initialDelay + 0.5f);
		}
		if (timeCheck)
		{
			Invoke("TimeCheck", initialDelay + delay);
		}
		objectPool = new Queue<GameObject>();
	}

	private void Spawn()
	{
		if (controlledObjects.Count < simultaneousInstances)
		{
			GameObject gameObject = null;
			if (objectPool.Count > 0)
			{
				gameObject = objectPool.Dequeue();
				gameObject.transform.position = base.transform.position;
				gameObject.transform.rotation = base.transform.rotation;
				gameObject.rigidbody.isKinematic = false;
				gameObject.SetActive(true);
				controlledObjects.Add(gameObject.transform);
			}
			else
			{
				Transform transform = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load(spawned, typeof(GameObject)), base.transform.position, base.transform.rotation)).transform;
				transform.parent = base.transform.parent;
				controlledObjects.Add(transform);
				gameObject = transform.gameObject;
			}
			if (timeCheck)
			{
				timeTrackers.Add(gameObject, DateTime.UtcNow);
			}
		}
		Invoke("Spawn", spawnPeriod + randomization * spawnPeriod * (UnityEngine.Random.value - 0.5f));
	}

	private void TimeCheck()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform controlledObject in controlledObjects)
		{
			GameObject gameObject = controlledObject.gameObject;
			if (timeTrackers[gameObject].AddSeconds(delay) < DateTime.UtcNow)
			{
				objectPool.Enqueue(gameObject);
				gameObject.rigidbody.velocity = Vector3.zero;
				gameObject.rigidbody.angularVelocity = Vector3.zero;
				gameObject.rigidbody.isKinematic = true;
				gameObject.SetActive(false);
				list.Add(controlledObject);
				timeTrackers.Remove(gameObject);
			}
		}
		foreach (Transform item in list)
		{
			controlledObjects.Remove(item);
		}
		Invoke("TimeCheck", spawnPeriod);
	}

	private void DistanceCheck()
	{
		int num = 0;
		while (num < controlledObjects.Count)
		{
			if (distance < Vector3.Distance(controlledObjects[num].position, base.transform.position))
			{
				objectPool.Enqueue(controlledObjects[num].gameObject);
				timeTrackers.Remove(controlledObjects[num].gameObject);
				controlledObjects[num].gameObject.rigidbody.velocity = Vector3.zero;
				controlledObjects[num].gameObject.rigidbody.angularVelocity = Vector3.zero;
				controlledObjects[num].gameObject.rigidbody.isKinematic = true;
				controlledObjects[num].gameObject.SetActive(false);
				controlledObjects.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		Invoke("DistanceCheck", 0.5f);
	}

	public void DestroySpawned(GameObject go)
	{
		if (controlledObjects.Contains(go.transform))
		{
			controlledObjects.Remove(go.transform);
			objectPool.Enqueue(go);
			timeTrackers.Remove(go);
			go.rigidbody.velocity = Vector3.zero;
			go.rigidbody.angularVelocity = Vector3.zero;
			go.rigidbody.isKinematic = true;
			go.SetActive(false);
		}
	}
}
