using UnityEngine;

[PBSerialize("RigidbodyConstraintWrapper")]
public class RigidbodyConstraintWrapper : MonoBehaviour
{
	[PBSerializeField]
	public bool freezePositionX;

	[PBSerializeField]
	public bool freezePositionY;

	[PBSerializeField]
	public bool freezePositionZ;

	[PBSerializeField]
	public bool freezeRotationX;

	[PBSerializeField]
	public bool freezeRotationY;

	[PBSerializeField]
	public bool freezeRotationZ;

	private void Start()
	{
		if (base.rigidbody != null)
		{
			base.rigidbody.constraints = RigidbodyConstraints.None;
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezePositionX ? 2 : 0);
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezePositionY ? 4 : 0);
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezePositionZ ? 8 : 0);
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezeRotationX ? 16 : 0);
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezeRotationY ? 32 : 0);
			base.rigidbody.constraints |= (RigidbodyConstraints)(freezeRotationZ ? 64 : 0);
		}
	}

	public void ApplyConstraints(RigidbodyConstraints constraints)
	{
		base.rigidbody.constraints = constraints;
	}
}
