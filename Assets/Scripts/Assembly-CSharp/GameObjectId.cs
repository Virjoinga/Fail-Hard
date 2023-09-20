using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameObjectId : MonoBehaviour
{
	public int Id;

	private static Dictionary<int, GameObject> s_foundGameObjects;

	private void Awake()
	{
		while (Id == 0)
		{
			Id = GetHash(Guid.NewGuid().ToString());
		}
	}

	private void OnDestroy()
	{
		s_foundGameObjects = null;
	}

	public static GameObject Find(GameObjectId goId)
	{
		if (goId == null)
		{
			return null;
		}
		return Find(goId.Id);
	}

	public static GameObject Find(int id)
	{
		if (id == 0)
		{
			Debug.LogWarning("Tried to find gameObject with zero ID!. Invalid ID!");
			return null;
		}
		if (s_foundGameObjects == null)
		{
			PopulateCache();
		}
		GameObject value = null;
		if (!s_foundGameObjects.TryGetValue(id, out value))
		{
			Debug.LogError("Cannot find gameObject with Id: " + id);
		}
		return value;
	}

	private static void PopulateCache()
	{
		s_foundGameObjects = new Dictionary<int, GameObject>();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObjectId));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			GameObjectId gameObjectId = @object as GameObjectId;
			if ((bool)gameObjectId)
			{
				s_foundGameObjects.Add(gameObjectId.Id, gameObjectId.gameObject);
			}
		}
	}

	private static int GetHash(string input)
	{
		int num = 1114111;
		foreach (char c in input)
		{
			num = num * 1114111 + c;
		}
		return num;
	}
}
