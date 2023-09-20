using UnityEngine;

namespace Game
{
	public class VehicleBase : MonoBehaviour
	{
		protected const float SPEED_SCALE = 3.6f;

		public DudePose pose;

		public string PoseAnimationName;

		public Vehicle vehicleData;

		public float BalanceHelpThrottleWhileFrontUp;

		public float BalanceHelpThrottleWhileRearUp;

		public float BalanceHelpBrakeWhileFrontUp;

		public float BalanceHelpBrakeWhileRearUp;

		public float RotateAnimationAngularVelocity;

		public Vector3 RotateAnimationLimitsRearIdleForward;

		public Vector3 PositionAnimationLimit;

		public Gadget CurrentGadget;

		public Transform GadgetPosition;

		public float MaxJumpingForce
		{
			get
			{
				return vehicleData.MaxJumpingForce;
			}
		}

		public float MaxSpinVelocity
		{
			get
			{
				return vehicleData.CurrentMaxSpinVelocity;
			}
		}

		public float JumpOffForceAbsorb
		{
			get
			{
				return vehicleData.JumpOffForceAbsorb;
			}
		}

		public float MaxJumpOffForce
		{
			get
			{
				return vehicleData.MaxJumpOffForce;
			}
		}

		public float CurrentSpeed { get; protected set; }

		public float TargetSpeed { get; protected set; }

		protected bool ThrottleReleased { get; set; }

		public Vector3 JumpDirectionModifier { get; set; }

		public bool InJumpZone { get; set; }

		public float Throttle { get; protected set; }

		public float Brake { get; protected set; }

		public float EngineRpm { get; protected set; }

		public float AdjustedSpinVelocity
		{
			get
			{
				float num = vehicleData.CurrentMaxSpinVelocity;
				if (CurrentGadget != null)
				{
					num *= CurrentGadget.VehicleSpinMultiplier;
				}
				return num;
			}
		}

		public virtual void SetBrake(float brakingValue)
		{
		}

		public virtual void AddThrottle(float delta)
		{
		}

		public virtual void SetTargetThrottle(float relativeThrottle)
		{
		}

		public virtual void IdleThrottle()
		{
		}

		public virtual void AnimateOnThrottle(ConfigurableJoint connector, Rigidbody playerRb)
		{
		}

		public virtual void AnimateOnBrake(ConfigurableJoint connector)
		{
		}

		public virtual void AnimateOnJump(ConfigurableJoint connector)
		{
		}

		public virtual void AnimateRotationIdle(ConfigurableJoint connector)
		{
		}

		public virtual void AnimatePositionIdle(ConfigurableJoint connector)
		{
		}

		public virtual Vector3 JumpWithVehicle()
		{
			return Vector3.zero;
		}

		protected void RotateRootTowards(ConfigurableJoint connector, float target, float factor)
		{
			if ((bool)connector)
			{
				Vector3 eulerAngles = connector.targetRotation.eulerAngles;
				if (eulerAngles.y > 180f)
				{
					eulerAngles.y -= 360f;
				}
				float num = target - eulerAngles.y;
				RotateRoot(connector, num * factor);
			}
		}

		protected void RotateRoot(ConfigurableJoint connector, float delta)
		{
			if ((bool)connector)
			{
				Vector3 eulerAngles = connector.targetRotation.eulerAngles;
				eulerAngles.y += Time.deltaTime * delta;
				if (eulerAngles.y > RotateAnimationLimitsRearIdleForward.z && eulerAngles.y < 180f)
				{
					eulerAngles.y = RotateAnimationLimitsRearIdleForward.z;
				}
				else if (eulerAngles.y > 180f && eulerAngles.y < RotateAnimationLimitsRearIdleForward.x)
				{
					eulerAngles.y = RotateAnimationLimitsRearIdleForward.x;
				}
				connector.targetRotation = Quaternion.Euler(eulerAngles);
			}
		}

		protected void MoveRootTowards(ConfigurableJoint connector, Vector3 target, float factor)
		{
			if ((bool)connector)
			{
				Vector3 vector = target - connector.targetPosition;
				connector.targetPosition += factor * vector * Time.deltaTime;
			}
		}

		protected void MoveRootWithConstantSpeed(ConfigurableJoint connector, Vector3 target, float speed)
		{
			if ((bool)connector)
			{
				Vector3 vector = target - connector.targetPosition;
				if (vector.sqrMagnitude > 0.0001f)
				{
					connector.targetPosition += speed * vector.normalized * Time.deltaTime;
				}
			}
		}

		public virtual bool EquipGadget(VehiclePart gadget)
		{
			if (CurrentGadget != null)
			{
				if (CurrentGadget.VehiclePart == gadget)
				{
					Object.Destroy(CurrentGadget.gameObject);
					return false;
				}
				Object.Destroy(CurrentGadget.gameObject);
			}
			GameObject gameObject = (GameObject)Resources.Load(gadget.GadgetPrefabResource);
			if (gameObject == null)
			{
				Debug.LogWarning("Vehicle gadget resource load failed: " + gadget.Id);
				return false;
			}
			GameObject gameObject2 = null;
			if (gadget.ItemType == VehiclePartType.VehicleGadget)
			{
				gameObject2 = Object.Instantiate(gameObject, GadgetPosition.position, GadgetPosition.rotation) as GameObject;
				gameObject2.transform.parent = GadgetPosition.parent;
				gameObject2.GetComponent<Gadget>().Equip(this, base.rigidbody, gadget);
			}
			if (gameObject2 == null)
			{
				Debug.LogWarning("Invalid gadget type: " + gadget.ItemType);
				return false;
			}
			CurrentGadget = gameObject2.GetComponent<Gadget>();
			gameObject2.GetComponent<Gadget>().Equip(this, base.rigidbody, gadget);
			return true;
		}

		public virtual void ReleaseThrottle()
		{
			ThrottleReleased = true;
		}

		public virtual void ReturnThrottle()
		{
			ThrottleReleased = false;
		}

		public virtual bool IsGrounded()
		{
			return false;
		}

		public virtual bool IsGrounded(out bool front, out bool rear)
		{
			front = false;
			rear = false;
			return false;
		}

		public virtual void ReleaseConstraints()
		{
		}

		public virtual VehiclePoseIK GetPoseIK()
		{
			return GetComponentInChildren<VehiclePoseIK>();
		}

		public virtual void DisableAllDynamics()
		{
		}

		public virtual void FallApart()
		{
		}
	}
}
