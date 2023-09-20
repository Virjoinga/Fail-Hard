using System.Collections.Generic;
using UnityEngine;

public class Cameraman : MonoBehaviour
{
	private Transform lookAtPoint;

	public List<Transform> lookAtTargets;

	protected float currentSlerpToTarget;

	public float slerpToTargetTime;

	public Camera CopyFromCamera;

	protected bool m_isTracking = true;

	public virtual bool LimitsCheck(out bool left, out bool right, out bool up, out bool down)
	{
		left = false;
		right = false;
		up = false;
		down = false;
		return false;
	}

	public virtual void ConfigWithCamera(Camera cam, Transform target)
	{
		CopyFromCamera = cam;
		SetLookAtTarget(target);
		m_isTracking = false;
	}

	private void SetLookAtTarget(Transform target)
	{
		lookAtPoint = target;
		lookAtTargets.Clear();
		if (target != null)
		{
			lookAtTargets.Add(target);
		}
		currentSlerpToTarget = 0f;
	}

	public virtual void StartTracking(Transform target)
	{
		m_isTracking = true;
		SetLookAtTarget(target);
	}

	public virtual void AddLookAtTarget(Transform target)
	{
		lookAtTargets.Add(target);
		currentSlerpToTarget = 0f;
	}

	public virtual Transform LookAtTarget()
	{
		return lookAtPoint;
	}

	public virtual Vector3 CurrentLookAtPosition()
	{
		return Vector3.zero;
	}
}
