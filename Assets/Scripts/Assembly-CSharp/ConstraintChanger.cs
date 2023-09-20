using UnityEngine;

[PBSerialize("ConstraintChanger")]
public class ConstraintChanger : MonoBehaviour
{
	[PBSerializeField]
	public bool triggerOnPlayer = true;

	[PBSerializeField]
	public bool triggerOnLevelObject = true;

	[PBSerializeField]
	public bool targetSelf;

	[PBSerializeField]
	public bool triggerOnce = true;

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

	private RigidbodyConstraints appliedConstraints;

	private bool triggered;

	private void Start()
	{
		appliedConstraints = RigidbodyConstraints.None;
		appliedConstraints |= (RigidbodyConstraints)(freezePositionX ? 2 : 0);
		appliedConstraints |= (RigidbodyConstraints)(freezePositionY ? 4 : 0);
		appliedConstraints |= (RigidbodyConstraints)(freezePositionZ ? 8 : 0);
		appliedConstraints |= (RigidbodyConstraints)(freezeRotationX ? 16 : 0);
		appliedConstraints |= (RigidbodyConstraints)(freezeRotationY ? 32 : 0);
		appliedConstraints |= (RigidbodyConstraints)(freezeRotationZ ? 64 : 0);
	}

	private void OnTriggerExit(Collider hit)
	{
		OnTriggerEnter(hit);
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (triggerOnce && triggered)
		{
			return;
		}
		if (targetSelf)
		{
			if (triggerOnPlayer)
			{
				Debug.LogWarning("Target self + trigger on player doesn't make sense.");
			}
			if (triggerOnLevelObject)
			{
				RigidbodyConstraintWrapper component = hit.gameObject.transform.GetComponent<RigidbodyConstraintWrapper>();
				if (hit.gameObject.tag == "DynamicLevelObject")
				{
					component.ApplyConstraints(appliedConstraints);
					triggered = true;
				}
			}
			else
			{
				Debug.LogWarning("Target self without trigger on level object doesn't make sense.");
			}
			return;
		}
		RigidbodyConstraintWrapper[] componentsInChildren = base.transform.parent.GetComponentsInChildren<RigidbodyConstraintWrapper>();
		foreach (RigidbodyConstraintWrapper rigidbodyConstraintWrapper in componentsInChildren)
		{
			if (triggerOnPlayer && (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player"))
			{
				rigidbodyConstraintWrapper.ApplyConstraints(appliedConstraints);
				triggered = true;
			}
			if (triggerOnLevelObject && hit.gameObject.tag == "DynamicLevelObject")
			{
				rigidbodyConstraintWrapper.ApplyConstraints(appliedConstraints);
				triggered = true;
			}
		}
	}
}
