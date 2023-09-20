using UnityEngine;

public class LevelTarget : MonoBehaviour
{
	public TargetZone[] targets;

	private int triggerCount;

	public string targetUuid;

	public float achievementBonus;

	public float achievementScore;

	public string ZoneId;

	public bool IsAchieved()
	{
		return triggerCount == targets.Length;
	}

	public void Reset()
	{
		triggerCount = 0;
	}
}
