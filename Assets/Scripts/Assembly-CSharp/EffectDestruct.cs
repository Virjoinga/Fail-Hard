using UnityEngine;

public class EffectDestruct : MonoBehaviour
{
	private void Start()
	{
		Object.Destroy(base.gameObject, base.particleSystem.duration * 2f);
	}
}
