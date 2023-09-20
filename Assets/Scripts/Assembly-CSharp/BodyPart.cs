using System.Runtime.CompilerServices;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
	public BodyPartType bodyPartType;

	public float currentHitPoints = 10f;

	public float maxHitPoints = 10f;

	public float safetyMultiplier = 1f;

	public float damageVelocityThreshold = 1f;

	public float damageScaler = 1f;

	public Equipment equipment;

	public int contactCount;

	[method: MethodImpl(32)]
	public event ImpactEventHandler Impact;

	public bool Wear(Equipment item)
	{
		if (item == null)
		{
			Object.Destroy(equipment.gameObject);
		}
		else
		{
			if (item.bodyPartType != bodyPartType)
			{
				Debug.LogError(string.Concat("Trying to assign wrong type (", item.bodyPartType, ") of equipment to ", base.gameObject.name));
				return false;
			}
			item.gameObject.transform.parent = base.transform;
			item.gameObject.transform.localPosition = Vector3.zero;
			item.gameObject.transform.localRotation = Quaternion.identity;
			equipment = item;
		}
		return true;
	}

	public string GetEquipmentName()
	{
		if (!equipment)
		{
			return string.Empty;
		}
		return equipment.gameObject.name;
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (hit.gameObject.tag == "Player")
		{
			return;
		}
		if (hit.contacts == null || hit.contacts.GetLength(0) == 0)
		{
			Debug.LogWarning("This shouldn't happen. Collision enter with no contact!? " + base.gameObject.name);
			return;
		}
		float num = Vector3.Dot(hit.contacts[0].normal, hit.relativeVelocity);
		if (num < 0f)
		{
			num = 0f;
		}
		contactCount++;
		if ((bool)hit.gameObject.GetComponent<AJMaterial>())
		{
			num *= hit.gameObject.GetComponent<AJMaterial>().damageMultiplier;
		}
		if (this.Impact != null)
		{
			ImpactEventArgs e = new ImpactEventArgs(bodyPartType, num, hit.gameObject);
			this.Impact(this, e);
		}
	}

	private void OnCollisionExit(Collision hit)
	{
		if (!(hit.gameObject.tag == "Player") && !(hit.gameObject.tag == "Vehicle"))
		{
			contactCount--;
		}
	}

	public bool IsContact()
	{
		return contactCount > 0;
	}
}
