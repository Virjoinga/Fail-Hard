using UnityEngine;

namespace Game
{
	public class RuntimePrefabInstantiator : IPrefabInstantiator
	{
		public GameObject Instantiate(Object obj)
		{
			return Object.Instantiate(obj) as GameObject;
		}
	}
}
