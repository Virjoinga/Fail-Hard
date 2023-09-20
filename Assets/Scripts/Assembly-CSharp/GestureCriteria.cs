using UnityEngine;

public class GestureCriteria : IGestureCriteria
{
	protected bool gestureActive;

	public virtual int fingerCount { get; set; }

	public int requiredFingerCount { get; set; }

	public Vector2 origin { get; set; }

	public Vector2 target { get; set; }

	public float targetDistance { get; set; }

	public bool onlyTracking { get; set; }

	public TrackCriteria trackerCallback { get; set; }

	public Vector2 cumulativeDelta { get; set; }

	public virtual void Began()
	{
		if (trackerCallback != null)
		{
			trackerCallback(origin, fingerCount, CriteriaState.Began);
		}
	}

	public virtual void Ended()
	{
		if (trackerCallback != null)
		{
			trackerCallback(cumulativeDelta, fingerCount, CriteriaState.Ended);
		}
	}

	public virtual void Moved(Vector2 delta)
	{
		cumulativeDelta += delta;
		if (trackerCallback != null)
		{
			trackerCallback(delta, fingerCount, CriteriaState.Moved);
		}
	}

	public virtual void Stationary()
	{
		if (trackerCallback != null)
		{
			trackerCallback(cumulativeDelta, fingerCount, CriteriaState.Stationary);
		}
	}

	public virtual bool Reached()
	{
		return true;
	}
}
