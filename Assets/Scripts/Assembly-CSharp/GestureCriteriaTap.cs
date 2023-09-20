public class GestureCriteriaTap : GestureCriteria
{
	private bool reached;

	private int maxFingerCount;

	private int fingerCount_;

	public override int fingerCount
	{
		get
		{
			return fingerCount_;
		}
		set
		{
			fingerCount_ = value;
			if (fingerCount_ > maxFingerCount)
			{
				maxFingerCount = fingerCount_;
			}
		}
	}

	public override bool Reached()
	{
		if (reached)
		{
			reached = false;
			return true;
		}
		return false;
	}

	public override void Ended()
	{
		base.Ended();
		if (fingerCount_ == 0)
		{
			if (cumulativeDelta.sqrMagnitude < targetDistance * targetDistance)
			{
				reached = maxFingerCount == base.requiredFingerCount;
			}
			maxFingerCount = 0;
		}
	}

	public static GestureCriteria SingleFinger()
	{
		GestureCriteria gestureCriteria = new GestureCriteriaTap();
		gestureCriteria.requiredFingerCount = 1;
		gestureCriteria.targetDistance = 50f;
		return gestureCriteria;
	}

	public static GestureCriteria DualFinger()
	{
		GestureCriteria gestureCriteria = new GestureCriteriaTap();
		gestureCriteria.requiredFingerCount = 2;
		gestureCriteria.targetDistance = 50f;
		return gestureCriteria;
	}
}
