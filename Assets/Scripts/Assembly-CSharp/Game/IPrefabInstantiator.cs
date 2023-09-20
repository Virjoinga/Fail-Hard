using UnityEngine;

namespace Game
{
	public interface IPrefabInstantiator
	{
		GameObject Instantiate(Object obj);
	}
}
