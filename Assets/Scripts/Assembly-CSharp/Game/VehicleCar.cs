using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class VehicleCar : VehicleBase
	{
		private const int CONTACT_SMOOTHING = 10;

		public List<Transform> WheelTransforms;

		public List<float> WheelRpms;

		public ParticleSystem SnowEffect;

		public ParticleSystem ExhaustSmoke;

		private float m_targetExhaustEmission;

		public float WheelRadius;

		public GameObject PremiumModel;

		public GameObject NormalModel;

		public string MyId;

		public float maxThrottle = 1f;

		private int m_boostCooling;

		public float BoostCoolingPeriod;

		private int m_smoothContact;

		public List<WheelGroundContact> FrontExtraColliders;

		public List<WheelGroundContact> RearExtraColliders;

		public bool IsFrontWheelDrive;

		public bool IsRearWheelDrive;

		public float FrontDrivePosition;

		public float RearDrivePosition;

		private float m_currentSpeedWithDir;

		public bool HcrMode = true;

		private bool m_fallenApart;

		private bool m_jumpedOff;

		private void Start()
		{
			vehicleData = GameController.Instance.VehicleModel.GetVehicle(MyId);
			base.Throttle = 0f;
			if (vehicleData.MaxCondition < 0)
			{
				PremiumModel.SetActive(true);
				NormalModel.SetActive(false);
			}
			else
			{
				PremiumModel.SetActive(false);
				NormalModel.SetActive(true);
			}
		}

		private void Update()
		{
			if (!m_fallenApart)
			{
				int num = 0;
				foreach (Transform wheelTransform in WheelTransforms)
				{
					wheelTransform.Rotate(Vector3.right * 6f * WheelRpms[num] * Time.deltaTime, Space.Self);
					num++;
				}
				UpdateSnowEffect();
			}
			ExhaustSmoke.emissionRate += 0.5f * (m_targetExhaustEmission - ExhaustSmoke.emissionRate);
		}

		private void UpdateSnowEffect()
		{
			if (IsGrounded())
			{
				if (base.CurrentSpeed > 0f && RearExtraColliders[0].IsSnow)
				{
					SnowEffect.Play();
					float emissionRate = 100f * base.Throttle;
					SnowEffect.emissionRate = emissionRate;
				}
				else
				{
					SnowEffect.Stop();
					SnowEffect.emissionRate = 0f;
				}
			}
			else
			{
				SnowEffect.emissionRate = 3f;
			}
		}

		private void UpdateRpms()
		{
			bool front;
			bool rear;
			IsGrounded(out front, out rear);
			if (front)
			{
				float num = (float)Math.PI * 2f * WheelRadius;
				float num2 = Vector3.Dot(FrontExtraColliders[0].rigidbody.velocity, FrontExtraColliders[0].ForwardDir);
				float num3 = num2 / num * 60f;
				if (IsFrontWheelDrive)
				{
					float num4 = base.Throttle;
					if (base.Throttle > 0.9f && base.Brake > 0.9f)
					{
						num4 = 0f;
					}
					float num5 = 10f * vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude) * num4;
					num3 += num5;
				}
				WheelRpms[2] = num3;
				WheelRpms[3] = num3;
			}
			else
			{
				List<float> wheelRpms;
				List<float> list = (wheelRpms = WheelRpms);
				int index;
				int index2 = (index = 2);
				float num6 = wheelRpms[index];
				list[index2] = num6 * 0.99f;
				WheelRpms[3] = WheelRpms[2];
			}
			if (rear)
			{
				float num7 = (float)Math.PI * 2f * WheelRadius;
				float num8 = Vector3.Dot(RearExtraColliders[0].rigidbody.velocity, RearExtraColliders[0].ForwardDir);
				float num9 = num8 / num7 * 60f;
				float num10 = base.Throttle;
				if (base.Throttle > 0.9f && base.Brake > 0.9f)
				{
					num10 = 0f;
				}
				float num11 = 10f * vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude) * num10;
				num9 += num11;
				WheelRpms[0] = num9;
				WheelRpms[1] = num9;
			}
			else
			{
				List<float> wheelRpms2;
				List<float> list2 = (wheelRpms2 = WheelRpms);
				int index;
				int index3 = (index = 0);
				float num6 = wheelRpms2[index];
				list2[index3] = num6 * 0.99f;
				List<float> wheelRpms3;
				List<float> list3 = (wheelRpms3 = WheelRpms);
				int index4 = (index = 1);
				num6 = wheelRpms3[index];
				list3[index4] = num6 * 0.99f;
			}
			base.EngineRpm = Mathf.Abs(WheelRpms[0]) + 50f * CurrentBoostCooling();
		}

		private void FixedUpdate()
		{
			if (m_fallenApart)
			{
				return;
			}
			if (m_boostCooling > 0)
			{
				m_boostCooling--;
			}
			if (IsGrounded())
			{
				m_smoothContact = 10;
			}
			else
			{
				m_smoothContact--;
			}
			base.CurrentSpeed = base.rigidbody.velocity.magnitude * 3.6f;
			if (Vector3.Dot(base.rigidbody.velocity, base.transform.forward) >= 0f)
			{
				m_currentSpeedWithDir = base.CurrentSpeed;
			}
			else
			{
				m_currentSpeedWithDir = 0f - base.CurrentSpeed;
			}
			if (!m_jumpedOff)
			{
				if (base.Throttle >= 0.9f && base.Brake >= 0.9f)
				{
					base.rigidbody.AddForce(CalculateBrakingForce(1f), ForceMode.Acceleration);
					m_targetExhaustEmission = 5f;
				}
				else if (base.Throttle > base.Brake)
				{
					if (m_currentSpeedWithDir < -1f)
					{
						base.rigidbody.AddForce(CalculateBrakingForce(base.Throttle), ForceMode.Acceleration);
						m_targetExhaustEmission = 5f;
					}
					else
					{
						ApplyThrottleForces();
					}
				}
				else if (m_currentSpeedWithDir > 1f)
				{
					base.rigidbody.AddForce(CalculateBrakingForce(base.Brake), ForceMode.Acceleration);
					m_targetExhaustEmission = 5f;
				}
				else
				{
					ApplyBrakeForces();
				}
			}
			else
			{
				base.rigidbody.AddForce(CalculateBrakingForce(0.05f), ForceMode.Acceleration);
				m_targetExhaustEmission = 5f;
			}
			base.rigidbody.AddForce(CalculateDrag(), ForceMode.Force);
			UpdateRpms();
		}

		public void ApplyThrottleForces()
		{
			bool front;
			bool rear;
			IsGrounded(out front, out rear);
			float num = 1f;
			if (IsFrontWheelDrive && IsRearWheelDrive && front && rear)
			{
				num = 0.5f;
			}
			if (front && IsFrontWheelDrive)
			{
				base.rigidbody.AddForceAtPosition(num * CalculateThrottleForce(FrontExtraColliders[0].ForwardDir, base.Throttle), base.rigidbody.worldCenterOfMass + FrontDrivePosition * base.transform.forward, ForceMode.Acceleration);
			}
			if (rear && IsRearWheelDrive)
			{
				base.rigidbody.AddForceAtPosition(num * CalculateThrottleForce(RearExtraColliders[0].ForwardDir, base.Throttle), base.rigidbody.worldCenterOfMass + RearDrivePosition * base.transform.forward, ForceMode.Acceleration);
			}
		}

		public void ApplyBrakeForces()
		{
			bool front;
			bool rear;
			IsGrounded(out front, out rear);
			float num = 1f;
			if (IsFrontWheelDrive && IsRearWheelDrive && front && rear)
			{
				num = 0.5f;
			}
			if (front && IsFrontWheelDrive)
			{
				base.rigidbody.AddForceAtPosition(0.8f * num * CalculateThrottleForce(-FrontExtraColliders[0].ForwardDir, base.Brake), base.rigidbody.worldCenterOfMass + FrontDrivePosition * base.transform.forward, ForceMode.Acceleration);
			}
			if (rear && IsRearWheelDrive)
			{
				base.rigidbody.AddForceAtPosition(0.8f * num * CalculateThrottleForce(-RearExtraColliders[0].ForwardDir, base.Brake), base.rigidbody.worldCenterOfMass + RearDrivePosition * base.transform.forward, ForceMode.Acceleration);
			}
		}

		public Vector3 CalculateThrottleForce(Vector3 fDir, float Control)
		{
			float num = Control * vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude);
			Vector3 result = num * fDir;
			m_targetExhaustEmission = 10f * num + 5f;
			if (fDir.y > 0f)
			{
				Vector3 vector = 0.5f * Control * fDir.y * Physics.gravity.y * fDir;
				result -= vector;
				m_targetExhaustEmission *= 1.5f;
			}
			return result;
		}

		public Vector3 CalculateThrottleForce()
		{
			Vector3 result = Vector3.zero;
			Vector3 vector = Vector3.zero;
			bool front;
			bool rear;
			IsGrounded(out front, out rear);
			bool flag = true;
			if (IsFrontWheelDrive && IsRearWheelDrive)
			{
				if (rear && front)
				{
					vector = 0.5f * (RearExtraColliders[0].ForwardDir + FrontExtraColliders[0].ForwardDir);
				}
				else if (front)
				{
					vector = FrontExtraColliders[0].ForwardDir;
				}
				else if (rear)
				{
					vector = RearExtraColliders[0].ForwardDir;
				}
				else
				{
					flag = false;
				}
			}
			else if (IsFrontWheelDrive)
			{
				vector = FrontExtraColliders[0].ForwardDir;
				if (!front)
				{
					flag = false;
				}
			}
			else
			{
				vector = RearExtraColliders[0].ForwardDir;
				if (!rear)
				{
					flag = false;
				}
			}
			if (flag)
			{
				float currentVelocity = 0f;
				if (m_currentSpeedWithDir >= 0f)
				{
					currentVelocity = base.rigidbody.velocity.magnitude;
				}
				float num = base.Throttle * vehicleData.GetMaxAcceleration(currentVelocity);
				result = num * vector;
				m_targetExhaustEmission = 10f * num + 5f;
				if (vector.y > 0f)
				{
					Vector3 vector2 = 0.5f * base.Throttle * vector.y * Physics.gravity.y * vector;
					result -= vector2;
					m_targetExhaustEmission *= 1.5f;
				}
			}
			return result;
		}

		public Vector3 CalculateBrakeForce()
		{
			Vector3 result = Vector3.zero;
			bool front;
			bool rear;
			IsGrounded(out front, out rear);
			if (rear)
			{
				float currentVelocity = 0f;
				if (m_currentSpeedWithDir <= 0f)
				{
					currentVelocity = base.rigidbody.velocity.magnitude;
				}
				result = -0.8f * base.Brake * RearExtraColliders[0].ForwardDir * vehicleData.GetMaxAcceleration(currentVelocity);
			}
			else if (!front)
			{
			}
			return result;
		}

		public Vector3 CalculateBrakingForce(float val)
		{
			Vector3 zero = Vector3.zero;
			bool front;
			bool rear;
			if (IsGrounded(out front, out rear) && base.CurrentSpeed < 2f && base.Brake > 0.9f)
			{
				base.rigidbody.velocity = Vector3.zero;
				return zero;
			}
			if (front)
			{
				zero = FrontExtraColliders[0].ForwardDir;
			}
			else
			{
				if (!rear)
				{
					return zero;
				}
				zero = RearExtraColliders[0].ForwardDir;
			}
			if (base.rigidbody.velocity.x > 0f)
			{
				return zero * (-25f * val);
			}
			return zero * (25f * val);
		}

		public Vector3 CalculateDrag()
		{
			Vector3 vector = Vector3.zero;
			bool front;
			bool rear;
			if (IsGrounded(out front, out rear) && (FrontExtraColliders[0].IsSnow || RearExtraColliders[0].IsSnow))
			{
				float num = 500f;
				float num2 = 0f * vehicleData.GetTopSpeed();
				float num3 = 0f;
				if (base.CurrentSpeed >= vehicleData.GetTopSpeed())
				{
					num3 = 1f;
				}
				else if (base.CurrentSpeed < num2)
				{
					num3 = 0f;
				}
				else
				{
					num3 = (base.CurrentSpeed - num2) / (vehicleData.GetTopSpeed() - num2);
					num3 *= num3;
				}
				if (base.CurrentSpeed > 1f)
				{
					vector = -base.rigidbody.velocity.normalized * num3 * num;
					vector -= Vector3.Dot(vector, base.transform.up) * base.transform.up;
				}
			}
			return vector;
		}

		public override void SetBrake(float brakingValue)
		{
			base.Brake = brakingValue;
		}

		public override void AddThrottle(float delta)
		{
			base.Throttle += delta;
			if (base.Throttle > maxThrottle)
			{
				base.Throttle = maxThrottle;
			}
			else if (base.Throttle < 0f)
			{
				base.Throttle = 0f;
			}
			base.TargetSpeed += vehicleData.UltimateMaxSpeed * delta;
			if (base.TargetSpeed < 0f)
			{
				base.TargetSpeed = 0f;
			}
			else if (base.TargetSpeed > vehicleData.UltimateMaxSpeed)
			{
				base.TargetSpeed = vehicleData.UltimateMaxSpeed;
			}
		}

		public override void SetTargetThrottle(float relativeThrottle)
		{
			base.Throttle = relativeThrottle * maxThrottle;
			base.TargetSpeed = relativeThrottle * vehicleData.UltimateMaxSpeed;
		}

		public override void IdleThrottle()
		{
			base.Throttle *= 0.2f;
		}

		public override bool IsGrounded()
		{
			return FrontExtraColliders[0].IsContact || FrontExtraColliders[1].IsContact || RearExtraColliders[0].IsContact || RearExtraColliders[1].IsContact;
		}

		public override bool IsGrounded(out bool front, out bool rear)
		{
			front = FrontExtraColliders[0].IsContact || FrontExtraColliders[1].IsContact;
			rear = RearExtraColliders[0].IsContact || RearExtraColliders[1].IsContact;
			return front || rear;
		}

		public override void ReleaseConstraints()
		{
			base.rigidbody.constraints = RigidbodyConstraints.None;
			WheelGroundContact[] componentsInChildren = base.transform.parent.GetComponentsInChildren<WheelGroundContact>();
			foreach (WheelGroundContact wheelGroundContact in componentsInChildren)
			{
				wheelGroundContact.rigidbody.constraints = RigidbodyConstraints.None;
				wheelGroundContact.SwitchToAccurateCollider(true);
			}
			Collider[] componentsInChildren2 = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren2)
			{
				collider.material.dynamicFriction = 1f;
			}
			m_jumpedOff = true;
			base.Throttle = 0f;
			base.Brake = 0f;
		}

		public override void DisableAllDynamics()
		{
			AudioSource[] components = GetComponents<AudioSource>();
			foreach (AudioSource audioSource in components)
			{
				audioSource.enabled = false;
			}
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.gameObject.SetActive(false);
			}
			base.rigidbody.isKinematic = true;
			FrontExtraColliders[0].rigidbody.isKinematic = true;
			FrontExtraColliders[1].rigidbody.isKinematic = true;
			RearExtraColliders[0].rigidbody.isKinematic = true;
			RearExtraColliders[1].rigidbody.isKinematic = true;
		}

		public override void FallApart()
		{
			Joint[] components = GetComponents<Joint>();
			foreach (Joint obj in components)
			{
				UnityEngine.Object.Destroy(obj);
			}
			WheelGroundContact[] componentsInChildren = base.transform.parent.GetComponentsInChildren<WheelGroundContact>();
			foreach (WheelGroundContact wheelGroundContact in componentsInChildren)
			{
				wheelGroundContact.SwitchToAccurateCollider(true);
				wheelGroundContact.SetFriction(1f);
			}
			m_fallenApart = true;
		}

		public override void AnimateOnThrottle(ConfigurableJoint connector, Rigidbody playerRb)
		{
			RotateRoot(connector, 0f - RotateAnimationAngularVelocity);
		}

		public override void AnimateOnBrake(ConfigurableJoint connector)
		{
			RotateRoot(connector, RotateAnimationAngularVelocity);
		}

		public override void AnimateOnJump(ConfigurableJoint connector)
		{
			Quaternion targetRotation = connector.targetRotation;
			targetRotation.y = 0f;
			connector.targetRotation = targetRotation;
			connector.targetPosition = PositionAnimationLimit;
		}

		public override void AnimateRotationIdle(ConfigurableJoint connector)
		{
			RotateRootTowards(connector, RotateAnimationLimitsRearIdleForward.y, 3f);
		}

		public override void AnimatePositionIdle(ConfigurableJoint connector)
		{
			MoveRootWithConstantSpeed(connector, 0.05f * Vector3.right, 0.5f);
		}

		public override Vector3 JumpWithVehicle()
		{
			Vector3 result = Vector3.zero;
			Vector3 vector = (((!(base.Throttle > 0f) && !(m_currentSpeedWithDir >= 0f)) || !(base.Brake <= 0f)) ? (-base.transform.forward + 1.5f * base.transform.up).normalized : (base.transform.forward + 1.5f * base.transform.up).normalized);
			if (IsGrounded())
			{
				float num = base.MaxJumpingForce * (1f - CurrentBoostCooling());
				result = 0.5f * num * vector;
				base.rigidbody.AddForceAtPosition(0.5f * num * vector, base.rigidbody.position + 0.8f * base.transform.forward, ForceMode.VelocityChange);
			}
			m_boostCooling = (int)(BoostCoolingPeriod / Time.fixedDeltaTime);
			return result;
		}

		private float CurrentBoostCooling()
		{
			float num = BoostCoolingPeriod / Time.fixedDeltaTime;
			return (float)m_boostCooling / num;
		}

		public bool SmoothGrounded()
		{
			return m_smoothContact > 0;
		}
	}
}
