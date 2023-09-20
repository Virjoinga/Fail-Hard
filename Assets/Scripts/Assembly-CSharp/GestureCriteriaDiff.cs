public class GestureCriteriaDiff : GestureCriteria
{
	public override bool Reached()
	{
		if (onlyTracking)
		{
			return false;
		}
		return cumulativeDelta.sqrMagnitude > target.sqrMagnitude;
	}
}
