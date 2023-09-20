using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class VehicleMoped : VehicleBase
	{
		public Transform frontWheelTransform;

		public Transform rearWheelTransform;

		public List<Transform> WheelTransforms;

		public ParticleSystem SnowEffect;

		public float WheelRadius;

		public float FrontRpm;

		public float RearRpm;

		public GameObject PremiumModel;

		public GameObject NormalModel;

		public string MyId;

		public float maxThrottle = 1f;

		public float throttleHelper = 100f;

		public Transform playerRoot;

		public WheelGroundContact FrontExtraCollider;

		public WheelGroundContact RearExtraCollider;

		public bool HcrMode = true;

		private bool m_fallenApart;

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
			if (m_fallenApart)
			{
				return;
			}
			if (WheelTransforms.Count > 0)
			{
				foreach (Transform wheelTransform in WheelTransforms)
				{
					wheelTransform.Rotate(Vector3.right * 6f * FrontRpm * Time.deltaTime, Space.Self);
				}
			}
			else
			{
				frontWheelTransform.Rotate(Vector3.right * 6f * FrontRpm * Time.deltaTime, Space.Self);
				rearWheelTransform.Rotate(Vector3.right * 6f * RearRpm * Time.deltaTime, Space.Self);
			}
			UpdateSnowEffect();
		}

		private void UpdateSnowEffect()
		{
			if (IsGrounded())
			{
				if (base.CurrentSpeed > 0f && RearExtraCollider.IsSnow)
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
			if (FrontExtraCollider.IsContact)
			{
				float num = (float)Math.PI * 2f * WheelRadius;
				float num2 = Vector3.Dot(FrontExtraCollider.rigidbody.velocity, FrontExtraCollider.ForwardDir);
				FrontRpm = num2 / num * 60f;
			}
			else
			{
				FrontRpm *= 0.99f;
			}
			if (RearExtraCollider.IsContact)
			{
				float num3 = (float)Math.PI * 2f * WheelRadius;
				float num4 = Vector3.Dot(RearExtraCollider.rigidbody.velocity, RearExtraCollider.ForwardDir);
				RearRpm = num4 / num3 * 60f;
				float num5 = base.Throttle;
				if (base.Throttle > 0.9f && base.Brake > 0.9f)
				{
					num5 = 0f;
				}
				float num6 = 10f * vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude) * num5;
				RearRpm += num6;
			}
			else
			{
				RearRpm *= 0.99f;
			}
			base.EngineRpm = RearRpm;
		}

		private void FixedUpdate()
		{
			if (!m_fallenApart)
			{
				base.CurrentSpeed = base.rigidbody.velocity.magnitude * 3.6f;
				bool front;
				bool rear;
				IsGrounded(out front, out rear);
				if (rear)
				{
					base.rigidbody.AddForce(CalculateDrivingForce(), ForceMode.Acceleration);
				}
				base.rigidbody.AddForce(CalculateBrakingForce(), ForceMode.Acceleration);
				base.rigidbody.AddForce(CalculateDrag(), ForceMode.Force);
				UpdateRpms();
			}
		}

		public Vector3 CalculateDrivingForce()
		{
			Vector3 result = Vector3.zero;
			if (base.Brake >= 0.9f)
			{
				return result;
			}
			if (RearExtraCollider.IsContact)
			{
				float maxAcceleration = vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude);
				if (RearExtraCollider.IsSnow)
				{
				}
				result = base.Throttle * RearExtraCollider.ForwardDir * maxAcceleration;
				if (RearExtraCollider.ForwardDir.y > 0f)
				{
					Vector3 vector = 0.5f * base.Throttle * RearExtraCollider.ForwardDir.y * Physics.gravity.y * RearExtraCollider.ForwardDir;
					result -= vector;
				}
			}
			return result;
		}

		public Vector3 CalculateBrakingForce()
		{
			Vector3 vector = Vector3.zero;
			if (RearExtraCollider.IsContact || FrontExtraCollider.IsContact)
			{
				if (!(base.CurrentSpeed < 2f) || !(base.Brake > 0.9f))
				{
					vector = ((!(base.rigidbody.velocity.x > 0f)) ? (12f * base.Brake * RearExtraCollider.ForwardDir) : (-12f * base.Brake * RearExtraCollider.ForwardDir));
				}
				else
				{
					base.rigidbody.velocity = vector;
				}
			}
			return vector;
		}

		public Vector3 CalculateDrag()
		{
			Vector3 vector = Vector3.zero;
			bool front;
			bool rear;
			if (IsGrounded(out front, out rear) && (FrontExtraCollider.IsSnow || RearExtraCollider.IsSnow))
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
			return FrontExtraCollider.IsContact || RearExtraCollider.IsContact;
		}

		public override bool IsGrounded(out bool front, out bool rear)
		{
			front = FrontExtraCollider.IsContact;
			rear = RearExtraCollider.IsContact;
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
			FrontExtraCollider.rigidbody.isKinematic = true;
			RearExtraCollider.rigidbody.isKinematic = true;
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
			Vector3 normalized = (base.transform.up + base.JumpDirectionModifier).normalized;
			if (IsGrounded())
			{
				result = 0.5f * base.MaxJumpingForce * normalized;
				base.rigidbody.AddForce(0.5f * base.MaxJumpingForce * normalized, ForceMode.VelocityChange);
			}
			return result;
		}
	}
}
