using UnityEngine;

namespace Game
{
	public class VehicleStiga : VehicleBase
	{
		private const int CONTACT_SMOOTHING = 10;

		private const float BOOST_COOLING_PERIOD = 0.6f;

		public string MyId;

		public AudioSource RidingSound;

		public Collider FrontSki;

		public Collider RearSkis;

		public ParticleSystem SnowEffect;

		private bool m_isSnow;

		private bool m_wasSnow;

		private float m_currentLiftFactor;

		public ParticleSystem LiftEffect;

		private int m_smoothContact;

		private int m_boostCooling;

		private bool m_isRearContact;

		private bool m_isFrontContact;

		private bool m_newRearContact;

		private bool m_newFrontContact;

		public Vector3 ForwardDir { get; private set; }

		private void Start()
		{
			vehicleData = GameController.Instance.VehicleModel.GetVehicle(MyId);
			base.Throttle = 0f;
			m_isRearContact = false;
			m_isFrontContact = false;
			m_newRearContact = false;
			m_newFrontContact = false;
			AudioManager.Instance.ReMix(RidingSound, 0f, AudioTag.VehicleAudio);
			AudioManager.Instance.Play(RidingSound, RidingSound.clip, 0.1f, AudioTag.VehicleAudio);
		}

		private void Update()
		{
			float newVolume = 0f;
			if (IsGrounded())
			{
				if (base.CurrentSpeed > 0f && m_wasSnow)
				{
					SnowEffect.Play();
					float emissionRate = 100f * Mathf.Lerp(0f, 1f, base.CurrentSpeed / 50f);
					SnowEffect.emissionRate = emissionRate;
				}
				else
				{
					SnowEffect.Stop();
					SnowEffect.emissionRate = 0f;
				}
				newVolume = Mathf.Clamp(base.CurrentSpeed / 50f, 0f, 1f);
			}
			else
			{
				SnowEffect.emissionRate = 3f;
			}
			AudioManager.Instance.ReMix(RidingSound, newVolume, AudioTag.VehicleAudio);
			LiftEffect.emissionRate = m_currentLiftFactor * 25f;
		}

		private void FixedUpdate()
		{
			if (m_smoothContact > 0)
			{
				m_smoothContact--;
			}
			if (m_boostCooling > 0)
			{
				m_boostCooling--;
			}
			m_isRearContact = m_newRearContact;
			m_newRearContact = false;
			m_isFrontContact = m_newFrontContact;
			m_newFrontContact = false;
			m_wasSnow = m_isSnow;
			m_isSnow = false;
			m_currentLiftFactor = 0f;
			base.CurrentSpeed = base.rigidbody.velocity.magnitude * 3.6f;
			if (base.rigidbody.constraints != 0)
			{
				if (!IsGrounded() && base.CurrentSpeed > 5f)
				{
					base.rigidbody.AddForce(CalculateLiftingForce(), ForceMode.Force);
				}
				base.rigidbody.AddForce(CalculateDrivingForce(), ForceMode.Acceleration);
				base.rigidbody.AddForce(CalculateBrakingForce(), ForceMode.Acceleration);
				base.rigidbody.AddForce(CalculateDrag(), ForceMode.Force);
			}
		}

		public Vector3 CalculateLiftingForce()
		{
			float num = Vector3.Dot(3.6f * base.rigidbody.velocity, -base.transform.forward);
			float num2 = 15f;
			float currentMaxLift = vehicleData.CurrentMaxLift;
			float num3 = Mathf.Clamp(Mathf.Abs(num) / num2, 0.1f, 1f);
			float num4 = Vector3.Dot(base.rigidbody.velocity.normalized, base.transform.forward);
			if (num4 > 0.7f)
			{
				float num5 = 3.3333333f;
				float num6 = -2.3333333f;
				m_currentLiftFactor = num5 * num4 + num6;
				if (Vector3.Dot(base.rigidbody.velocity, base.transform.up) > 0f)
				{
					m_currentLiftFactor = 0f - m_currentLiftFactor;
				}
			}
			Vector3 result = (0f - currentMaxLift) * num * num3 * m_currentLiftFactor * base.transform.up;
			result.z = 0f;
			return result;
		}

		public Vector3 CalculateDrag()
		{
			Vector3 vector = Vector3.zero;
			bool front;
			bool rear;
			if (IsGrounded(out front, out rear))
			{
				float num = 3000f;
				float num2 = 0.5f * vehicleData.GetTopSpeed();
				if (!m_wasSnow)
				{
					num *= 1f;
					num2 = 0f;
				}
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

		public Vector3 CalculateDrivingForce()
		{
			Vector3 result = Vector3.zero;
			Vector3 forward = base.transform.forward;
			if (IsGrounded())
			{
				float maxAcceleration = vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude);
				result = base.Throttle * forward * maxAcceleration;
			}
			return result;
		}

		public bool Boost(Rigidbody applyToThis)
		{
			Vector3 zero = Vector3.zero;
			if (IsGrounded() && base.Brake <= 0f && base.Throttle > 0f && m_boostCooling == 0 && base.CurrentSpeed < 10f)
			{
				Vector3 forward = base.transform.forward;
				float maxAcceleration = vehicleData.GetMaxAcceleration(base.rigidbody.velocity.magnitude);
				float num = 3000f + (maxAcceleration - 5f) * 500f;
				forward += 0.5f * base.transform.up;
				m_boostCooling = (int)(0.6f / Time.fixedDeltaTime);
				zero = forward * num;
				applyToThis.AddForce(zero);
				return true;
			}
			return false;
		}

		public Vector3 CalculateBrakingForce()
		{
			Vector3 vector = Vector3.zero;
			bool front;
			bool rear;
			if (IsGrounded(out front, out rear) && base.Brake > 0f)
			{
				if (base.CurrentSpeed < 2f)
				{
					base.rigidbody.velocity = vector;
				}
				else
				{
					float num = Vector3.Dot(base.rigidbody.velocity, base.transform.forward) * 3.6f;
					float num2 = Mathf.Clamp(-15f * num, -200f, 200f);
					vector = num2 * base.transform.forward;
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
			if (base.Throttle > 1f)
			{
				base.Throttle = 1f;
			}
			else if (base.Throttle < 0f)
			{
				base.Throttle = 0f;
			}
		}

		public override void SetTargetThrottle(float relativeThrottle)
		{
			base.Throttle = relativeThrottle;
		}

		public override void IdleThrottle()
		{
			if (base.Throttle < 0.1f)
			{
				base.Throttle = 0f;
			}
			else
			{
				base.Throttle *= 0.2f;
			}
		}

		public override bool IsGrounded()
		{
			return m_smoothContact > 0;
		}

		public override bool IsGrounded(out bool front, out bool rear)
		{
			front = m_isFrontContact;
			rear = m_isRearContact;
			return m_isRearContact || m_isFrontContact;
		}

		public override void ReleaseConstraints()
		{
			base.rigidbody.constraints = RigidbodyConstraints.None;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				collider.material.dynamicFriction = 0.6f;
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
		}

		private void RefreshGroundContactState(Collision hit)
		{
			if (!(hit.gameObject.tag != "Player") || hit.contacts.Length <= 0)
			{
				return;
			}
			ContactPoint[] contacts = hit.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				if (contactPoint.thisCollider == FrontSki)
				{
					m_smoothContact = 10;
					m_isFrontContact = true;
					m_newFrontContact = true;
					AJMaterial component = contactPoint.otherCollider.gameObject.GetComponent<AJMaterial>();
					if (component != null && component.surfaceMaterialType == SurfaceMaterialType.Snow)
					{
						m_isSnow = true;
						m_wasSnow = true;
					}
				}
				else if (contactPoint.thisCollider == RearSkis)
				{
					m_smoothContact = 10;
					m_isRearContact = true;
					m_newRearContact = true;
					AJMaterial component2 = contactPoint.otherCollider.gameObject.GetComponent<AJMaterial>();
					if (component2 != null && component2.surfaceMaterialType == SurfaceMaterialType.Snow)
					{
						m_isSnow = true;
						m_wasSnow = true;
					}
				}
			}
		}

		private void OnCollisionEnter(Collision hit)
		{
			RefreshGroundContactState(hit);
		}

		private void OnCollisionStay(Collision hit)
		{
			RefreshGroundContactState(hit);
		}

		public override void AnimateOnThrottle(ConfigurableJoint connector, Rigidbody playerRb)
		{
			RotateRoot(connector, 0f - RotateAnimationAngularVelocity);
			if (Boost(playerRb))
			{
				connector.targetPosition = 0.7f * PositionAnimationLimit;
			}
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
