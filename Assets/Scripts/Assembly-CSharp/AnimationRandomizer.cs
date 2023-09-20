using UnityEngine;

public class AnimationRandomizer : MonoBehaviour
{
	private void Start()
	{
		foreach (AnimationState item in base.animation)
		{
			item.normalizedTime = Random.Range(0f, 1f);
		}
	}
}
