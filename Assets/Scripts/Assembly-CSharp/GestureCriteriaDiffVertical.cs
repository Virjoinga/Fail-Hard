public class GestureCriteriaDiffVertical : GestureCriteria
{
	public override bool Reached()
	{
		if (onlyTracking)
		{
			return false;
		}
		return cumulativeDelta.y > target.y;
	}
}
