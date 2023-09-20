using UnityEngine;

public class BodyPartStatus
{
	public BodyPartType bodyPartType;

	public float currentHitPoints = 10f;

	public float maxHitPoints = 10f;

	public float safetyMultiplier = 1f;

	public float damageScaler = 1f;

	public string equipmentId;

	public bool Wear(Equipment item)
	{
		if (item == null)
		{
			equipmentId = string.Empty;
			safetyMultiplier = 1f;
		}
		else
		{
			if (item.bodyPartType != bodyPartType)
			{
				Debug.LogError(string.Concat("Trying to assign wrong type (", item.bodyPartType, ") of equipment to ", bodyPartType));
				return false;
			}
			safetyMultiplier = item.safetyMultiplier;
			equipmentId = item.equipmentId;
		}
		return true;
	}
}
