public class GestureCriteriaDiffHorizontal : GestureCriteria
{
	public override bool Reached()
	{
		if (onlyTracking)
		{
			return false;
		}
		return cumulativeDelta.x > target.x;
	}
}
