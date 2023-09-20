using UnityEngine;

[PBSerialize("CameraFlying")]
public class CameraFlying : Cameraman
{
	public CameraRoute route;

	[PBSerializeField]
	public float minY;

	[PBSerializeField]
	public float maxY;

	[PBSerializeField]
	public float smoothingFactor;

	[PBSerializeField]
	public float speedFactor;

	[PBSerializeField]
	public float zoomFactor;

	[PBSerializeField]
	public float minZoomLevel;

	[PBSerializeField]
	public float maxZoomLevel;

	[PBSerializeField]
	public Vector3 lookAtTargetOffset;

	[PBSerializeField]
	public float cameraOffset;

	[PBSerializeField]
	public float leftLimit;

	[PBSerializeField]
	public float rightLimit;

	private float u;

	private Vector3 slerpStartVector;

	private Vector3 slerpStartPosition;

	private Vector3 slerpStartLookAtPosition;

	private Vector3 currentLookAtPosition;

	private Vector3 velocityAdjustmentTarget;

	private Vector3 currentVelocityAdjustment;

	private AnimationCurve easingCurve;

	[PBSerializeField]
	public float CameraHeightFromGround = 0.5f;

	private float m_smoothGroundAvoidance;

	private void Start()
	{
		easingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		currentLookAtPosition = base.camera.transform.position + base.camera.transform.forward;
	}

	private void CalculateTargetsForTrackingMode(out Vector3 camPos, out Vector3 targetPos)
	{
		currentVelocityAdjustment = CalculateVelocityAdjustment();
		Vector3 vector = CalculateAvgPosition();
		targetPos = vector + lookAtTargetOffset + currentVelocityAdjustment;
		vector.x += cameraOffset;
		u = route.GetU(vector);
		camPos = route.GetPosition(u);
		camPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
	}

	private float CalculateFOVDeltaForTrackingMode()
	{
		float num = 0f;
		if (lookAtTargets[0].rigidbody != null)
		{
			num = lookAtTargets[0].rigidbody.velocity.x / 15f;
		}
		float num2 = 0f;
		if (lookAtTargets.Count > 1)
		{
			num2 = (lookAtTargets[0].position - lookAtTargets[1].position).magnitude;
		}
		float num3 = minZoomLevel + zoomFactor * num2;
		if (num3 > maxZoomLevel)
		{
			num3 = maxZoomLevel;
		}
		num = num3 + num * (maxZoomLevel - num3);
		return 0.4f * smoothingFactor * Time.fixedDeltaTime * (num - base.camera.fieldOfView);
	}

	private void FixedUpdate()
	{
		if (lookAtTargets.Count != 0)
		{
			Vector3 camPos;
			Vector3 targetPos;
			if (m_isTracking)
			{
				CalculateTargetsForTrackingMode(out camPos, out targetPos);
				base.camera.fieldOfView += CalculateFOVDeltaForTrackingMode();
			}
			else
			{
				camPos = CopyFromCamera.transform.position;
				targetPos = camPos + 10f * CopyFromCamera.transform.forward;
				base.camera.fieldOfView += 2f * Time.fixedDeltaTime * (CopyFromCamera.fieldOfView - base.camera.fieldOfView);
			}
			camPos.y += CalculateSmoothGroundAvoidance();
			if (currentSlerpToTarget == 0f)
			{
				slerpStartPosition = base.camera.transform.position;
				slerpStartLookAtPosition = currentLookAtPosition;
			}
			if (currentSlerpToTarget < 1f)
			{
				base.camera.transform.position = MoveWithEasing(slerpStartPosition, camPos, easingCurve, currentSlerpToTarget);
				currentLookAtPosition = MoveWithEasing(slerpStartLookAtPosition, targetPos, easingCurve, currentSlerpToTarget);
				currentSlerpToTarget += Time.fixedDeltaTime / slerpToTargetTime;
			}
			else
			{
				base.camera.transform.position += smoothingFactor * Time.fixedDeltaTime * (camPos - base.camera.transform.position);
				currentLookAtPosition += 0.5f * smoothingFactor * Time.fixedDeltaTime * (targetPos - currentLookAtPosition);
			}
			base.camera.transform.LookAt(currentLookAtPosition);
			if (m_isTracking)
			{
				ApplyRotationLimits();
			}
		}
	}

	private Vector3 MoveWithEasing(Vector3 from, Vector3 to, AnimationCurve curve, float slerper)
	{
		return from + curve.Evaluate(slerper) * (to - from);
	}

	private float CalculateSmoothGroundAvoidance()
	{
		float num = 0f;
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, -Vector3.up, out hitInfo, CameraHeightFromGround))
		{
			num = CameraHeightFromGround - hitInfo.distance;
		}
		if (num > m_smoothGroundAvoidance)
		{
			m_smoothGroundAvoidance += Time.fixedDeltaTime;
		}
		else if (num < m_smoothGroundAvoidance)
		{
			m_smoothGroundAvoidance -= Time.fixedDeltaTime;
		}
		if (Mathf.Abs(num - m_smoothGroundAvoidance) < 0.05f)
		{
			m_smoothGroundAvoidance = num;
		}
		return m_smoothGroundAvoidance;
	}

	private Vector3 CalculateAvgPosition()
	{
		Vector3 result = lookAtTargets[0].position;
		if (lookAtTargets.Count > 1)
		{
			float value = Mathf.Abs(lookAtTargets[0].position.x - lookAtTargets[1].position.x);
			float num = Mathf.Clamp(value, 5f, 10f) / 10f;
			result = num * lookAtTargets[0].position + (1f - num) * lookAtTargets[1].position;
		}
		return result;
	}

	private Vector3 CalculateVelocityAdjustment()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < lookAtTargets.Count; i++)
		{
			if ((bool)lookAtTargets[i].rigidbody)
			{
				zero += speedFactor * lookAtTargets[i].rigidbody.velocity;
				zero.y = 0f;
			}
		}
		zero *= 1f / (float)lookAtTargets.Count;
		if (zero.x > 3f)
		{
			zero.x = 3f;
		}
		velocityAdjustmentTarget = zero;
		return currentVelocityAdjustment + 0.1f * (velocityAdjustmentTarget - currentVelocityAdjustment);
	}

	private void ApplyRotationLimits()
	{
		Vector3 eulerAngles = base.camera.transform.rotation.eulerAngles;
		if (eulerAngles.y < leftLimit && eulerAngles.y > 180f)
		{
			eulerAngles.y = leftLimit;
			base.camera.transform.rotation = Quaternion.Euler(eulerAngles);
		}
		else if (eulerAngles.y > rightLimit && eulerAngles.y < 180f)
		{
			eulerAngles.y = rightLimit;
			base.camera.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}

	public override bool LimitsCheck(out bool left, out bool right, out bool up, out bool down)
	{
		left = false;
		right = false;
		up = false;
		down = false;
		bool left2;
		bool right2;
		RotationLimits(out left2, out right2);
		if (u < 0.02f && left2)
		{
			left = true;
		}
		if (u > 0.98f && right2)
		{
			right = true;
		}
		if (base.transform.position.y < minY + 0.1f)
		{
			down = true;
		}
		if (base.transform.position.y > maxY - 0.1f)
		{
			up = true;
		}
		return left || right || up || down;
	}

	public override Vector3 CurrentLookAtPosition()
	{
		return currentLookAtPosition;
	}

	private bool RotationLimits(out bool left, out bool right)
	{
		left = false;
		right = false;
		Vector3 eulerAngles = base.camera.transform.rotation.eulerAngles;
		if (eulerAngles.y <= leftLimit && eulerAngles.y > 180f)
		{
			left = true;
		}
		else if (eulerAngles.y >= rightLimit && eulerAngles.y < 180f)
		{
			right = true;
		}
		return left || right;
	}
}
