using UnityEngine;

public interface IGestureCriteria
{
	int fingerCount { get; set; }

	Vector2 origin { get; set; }

	Vector2 cumulativeDelta { get; set; }

	Vector2 target { get; set; }

	float targetDistance { get; set; }

	bool onlyTracking { get; set; }

	TrackCriteria trackerCallback { get; set; }

	bool Reached();

	void Began();

	void Ended();

	void Moved(Vector2 delta);

	void Stationary();
}
