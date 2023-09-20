using UnityEngine;

public class GestureCriteriaAbsHorizontal : GestureCriteria
{
	public override bool Reached()
	{
		if (onlyTracking)
		{
			return false;
		}
		return Mathf.Abs(cumulativeDelta.x) > target.x;
	}

	public static GestureCriteria Small()
	{
		GestureCriteria gestureCriteria = new GestureCriteriaAbsHorizontal();
		gestureCriteria.target = new Vector2(10f, 0f);
		return gestureCriteria;
	}
}
