using System.Collections.Generic;
using UnityEngine;

public static class GOTools
{
	public enum InstantiateOptions
	{
		None = 0,
		MatchLayerWithParent = 1
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, string poolId = "Main")
	{
		Transform transform = PoolManager.Pools[poolId].Spawn(prefab.transform, position, rotation);
		return transform.gameObject;
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, string poolId = "Main")
	{
		Transform transform = PoolManager.Pools[poolId].Spawn(prefab.transform, position, Quaternion.identity);
		return transform.gameObject;
	}

	public static GameObject SpawnAsChild(GameObject prefab, Transform parent, string poolId = "Main")
	{
		Transform transform = PoolManager.Pools[poolId].Spawn(prefab.transform);
		if (transform != null && parent != null)
		{
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			transform.gameObject.layer = parent.gameObject.layer;
		}
		return transform.gameObject;
	}

	public static void Despawn(GameObject go, string poolId = "Main")
	{
		SpawnPool spawnPool = null;
		if (PoolManager.Pools.TryGetValue(poolId, out spawnPool))
		{
			spawnPool.Despawn(go.transform);
			return;
		}
		Debug.LogWarning("No spawn pool " + poolId + " was found!");
		Object.Destroy(go);
	}

	public static GameObject Instantiate(GameObject prefab, GameObject parent)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		Transform transform = gameObject.transform;
		if (transform != null && parent != null)
		{
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			transform.gameObject.layer = parent.gameObject.layer;
		}
		return gameObject;
	}

	public static GameObject Instantiate(GameObject prefab, GameObject parent, InstantiateOptions options)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		Transform transform = gameObject.transform;
		if (transform != null && parent != null)
		{
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			if ((options & InstantiateOptions.MatchLayerWithParent) == InstantiateOptions.MatchLayerWithParent)
			{
				transform.gameObject.layer = parent.gameObject.layer;
			}
		}
		return gameObject;
	}

	public static GameObject InstantiateWithOriginalScale(GameObject parent, GameObject prefab)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		if (gameObject != null && parent != null)
		{
			Transform transform = gameObject.transform;
			Vector3 localScale = transform.localScale;
			transform.parent = parent.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = localScale;
			gameObject.layer = parent.layer;
		}
		return gameObject;
	}

	public static GameObject Instantiate(string name, GameObject parent)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = parent.transform;
		return gameObject;
	}

	public static List<GameObject> FindChildren(GameObject parent)
	{
		List<GameObject> list = new List<GameObject>();
		Transform transform = parent.transform;
		foreach (Transform item in transform)
		{
			list.Add(item.gameObject);
		}
		return list;
	}

	public static void DestroyChildren(GameObject gameObject)
	{
		Transform transform = gameObject.transform;
		foreach (Transform item in transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public static T FindFromChildren<T>(GameObject parent) where T : Component
	{
		return RecurseFindComponent<T>(parent.transform);
	}

	public static List<GameObject> FindAllWithComponent<T>(GameObject parent) where T : Component
	{
		HashSet<GameObject> objectSet = new HashSet<GameObject>();
		List<T> list = FindAllComponents<T>(parent);
		list.ForEach(delegate(T x)
		{
			objectSet.Add(x.gameObject);
		});
		return new List<GameObject>(objectSet);
	}

	public static List<GameObject> FindAllChldrenWithComponent<T>(GameObject parent) where T : Component
	{
		HashSet<GameObject> objectSet = new HashSet<GameObject>();
		List<T> list = FindAllComponents<T>(parent, true);
		list.ForEach(delegate(T x)
		{
			objectSet.Add(x.gameObject);
		});
		return new List<GameObject>(objectSet);
	}

	public static List<T> FindAllComponents<T>(GameObject root, bool excludeParent = false) where T : Component
	{
		HashSet<T> componentSet = new HashSet<T>();
		RecurseAppendComponent(root, ref componentSet, excludeParent);
		return new List<T>(componentSet);
	}

	public static List<T> FindAllFromChildren<T>(GameObject root) where T : Component
	{
		List<T> list = new List<T>();
		Transform transform = root.transform;
		foreach (Transform item in transform)
		{
			list.AddRange(item.GetComponents<T>());
		}
		return list;
	}

	public static void RemoveAllComponents<T>(GameObject root, bool removeGuttedGameObjects = false) where T : Component
	{
		List<T> list = FindAllComponents<T>(root);
		foreach (T item in list)
		{
			T current = item;
			GameObject gameObject = current.gameObject;
			Object.DestroyImmediate(current);
			if (removeGuttedGameObjects && gameObject.GetComponents<Component>().Length <= 1 && gameObject.transform.childCount == 0)
			{
				Object.DestroyImmediate(gameObject);
			}
		}
	}

	private static void RecurseAppendComponent<T>(GameObject go, ref HashSet<T> componentSet, bool excludeParent = false) where T : Component
	{
		if (!excludeParent)
		{
			T[] componentsInChildren = go.GetComponentsInChildren<T>(true);
			T[] array = componentsInChildren;
			foreach (T item in array)
			{
				componentSet.Add(item);
			}
		}
		foreach (Transform item2 in go.transform)
		{
			RecurseAppendComponent(item2.gameObject, ref componentSet);
		}
	}

	private static T RecurseFindComponent<T>(Transform tf) where T : Component
	{
		T val = tf.gameObject.GetComponent<T>();
		if (!(Object)val)
		{
			foreach (Transform item in tf)
			{
				val = RecurseFindComponent<T>(item);
				if ((bool)(Object)val)
				{
					break;
				}
			}
		}
		return val;
	}
}
