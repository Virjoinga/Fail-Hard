using UnityEngine;

public class GestureCriteriaTarget : GestureCriteria
{
	private const float distanceLimit = 0.5f;

	public static float GRIDSIZE = 200f;

	public override bool Reached()
	{
		if (onlyTracking)
		{
			return false;
		}
		return Vector2.Distance(cumulativeDelta, target) < targetDistance;
	}

	public static GestureCriteriaTarget Up()
	{
		GestureCriteriaTarget gestureCriteriaTarget = new GestureCriteriaTarget();
		gestureCriteriaTarget.target = new Vector2(0f, GRIDSIZE);
		gestureCriteriaTarget.targetDistance = 0.5f * GRIDSIZE;
		return gestureCriteriaTarget;
	}

	public static GestureCriteriaTarget Down()
	{
		GestureCriteriaTarget gestureCriteriaTarget = new GestureCriteriaTarget();
		gestureCriteriaTarget.target = new Vector2(0f, 0f - GRIDSIZE);
		gestureCriteriaTarget.targetDistance = 0.5f * GRIDSIZE;
		return gestureCriteriaTarget;
	}

	public static GestureCriteriaTarget Right()
	{
		GestureCriteriaTarget gestureCriteriaTarget = new GestureCriteriaTarget();
		gestureCriteriaTarget.target = new Vector2(GRIDSIZE, 0f);
		gestureCriteriaTarget.targetDistance = 0.5f * GRIDSIZE;
		return gestureCriteriaTarget;
	}

	public static GestureCriteriaTarget Left()
	{
		GestureCriteriaTarget gestureCriteriaTarget = new GestureCriteriaTarget();
		gestureCriteriaTarget.target = new Vector2(0f - GRIDSIZE, 0f);
		gestureCriteriaTarget.targetDistance = 0.5f * GRIDSIZE;
		return gestureCriteriaTarget;
	}

	public static GestureCriteriaTarget DiffTracker()
	{
		GestureCriteriaTarget gestureCriteriaTarget = new GestureCriteriaTarget();
		gestureCriteriaTarget.onlyTracking = true;
		return gestureCriteriaTarget;
	}
}
