using Game;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
	public AudioSource audioSource;

	private void Start()
	{
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (!(hit.relativeVelocity.magnitude > 3f))
		{
			return;
		}
		AJMaterial component = hit.gameObject.GetComponent<AJMaterial>();
		AJMaterial aJMaterial = null;
		MaterialType materialType = MaterialType.Soft;
		MaterialType materialType2 = MaterialType.Hard;
		materialType2 = ((!component) ? MaterialType.Hard : component.materialType);
		ContactPoint[] contacts = hit.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			aJMaterial = contactPoint.thisCollider.gameObject.GetComponent<AJMaterial>();
			if (!aJMaterial || ((bool)aJMaterial && aJMaterial.materialType == MaterialType.Hard))
			{
				materialType = MaterialType.Hard;
				break;
			}
		}
		if (materialType == MaterialType.Soft && materialType2 == MaterialType.Soft)
		{
			if ((bool)aJMaterial && (bool)aJMaterial.softCollision && (bool)base.audio)
			{
				AudioManager.Instance.Play(base.audio, aJMaterial.softCollision, 0.5f, AudioTag.ItemCrashAudio2);
			}
		}
		else if ((bool)aJMaterial && (bool)aJMaterial.hardCollision && (bool)base.audio)
		{
			AudioManager.Instance.Play(base.audio, aJMaterial.hardCollision, 0.5f, AudioTag.ItemCrashAudio1);
		}
	}
}
