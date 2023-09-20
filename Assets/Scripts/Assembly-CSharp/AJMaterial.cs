using UnityEngine;

public class AJMaterial : MonoBehaviour
{
	public float damageMultiplier = 1f;

	public MaterialType materialType = MaterialType.Hard;

	public SurfaceMaterialType surfaceMaterialType;

	public AudioClip softCollision;

	public AudioClip hardCollision;
}
