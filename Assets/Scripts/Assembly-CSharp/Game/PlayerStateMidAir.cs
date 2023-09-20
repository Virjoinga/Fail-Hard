using UnityEngine;

namespace Game
{
	public class PlayerStateMidAir : PlayerState
	{
		private float midAirTorque = 50f;

		public float spinVelocityTarget;

		private float magicSpinFactor = 2f;

		private float maxSpinVelocity = 200f;

		private float horizontalWindow;

		private float gadgetMultiplier = 1f;

		public bool ManualSpin;

		private MidAirState currentState;

		private void Start()
		{
			root = FindRoot(base.transform);
			pac = root.GetComponentInChildren<PlayerAnimationController>();
			player = root.GetComponentInChildren<Player>();
			base.gameObject.AddComponent<MidAirStateFreefall>();
			base.gameObject.AddComponent<MidAirStateCrashing>();
			base.gameObject.AddComponent<MidAirStateCrashed>();
		}

		public override void Enter()
		{
			ChangeState("MidAirStateFreefall");
		}

		public MidAirState getCurrentState()
		{
			return currentState;
		}

		public void Enter(float hack)
		{
			spinVelocityTarget = hack * maxSpinVelocity;
			gadgetMultiplier = player.GetPlayerSpinMultiplier();
			Enter();
		}

		public void ChangeState(string newState)
		{
			if (currentState != null)
			{
				currentState.Exit();
			}
			switch (newState)
			{
			case "MidAirStateFreefall":
				currentState = base.gameObject.GetComponent<MidAirStateFreefall>();
				break;
			case "MidAirStateCrashing":
				currentState = base.gameObject.GetComponent<MidAirStateCrashing>();
				break;
			case "MidAirStateCrashed":
				currentState = base.gameObject.GetComponent<MidAirStateCrashed>();
				break;
			default:
				Debug.LogError("Invalid MidAir sub-state " + newState);
				break;
			}
			currentState.Enter();
		}

		public override void UpdateFunc()
		{
		}

		public override void FixedUpdateFunc()
		{
			if (ButtonControlScheme.Instance.UsingButtonScheme)
			{
				if (ButtonControlScheme.Instance.LeftPressed)
				{
					spinVelocityTarget = (0f - gadgetMultiplier) * maxSpinVelocity;
					if (player.InvertedControls)
					{
						spinVelocityTarget = 0f - spinVelocityTarget;
					}
					ManualSpin = true;
				}
				else if (ButtonControlScheme.Instance.RightPressed)
				{
					spinVelocityTarget = gadgetMultiplier * maxSpinVelocity;
					if (player.InvertedControls)
					{
						spinVelocityTarget = 0f - spinVelocityTarget;
					}
					ManualSpin = true;
				}
				else
				{
					spinVelocityTarget = 0f;
				}
			}
			if (ButtonControlScheme.Instance.ActionHeadDown)
			{
				player.HandleAction(ButtonControlScheme.VehicleControl.ActionHead);
			}
			if (ButtonControlScheme.Instance.ActionTorsoDown)
			{
				player.HandleAction(ButtonControlScheme.VehicleControl.ActionTorso);
			}
			if (ButtonControlScheme.Instance.ActionVehicleDown)
			{
				player.HandleAction(ButtonControlScheme.VehicleControl.ActionVehicle);
			}
			currentState.FixedUpdateFunc();
		}

		public override void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude, GameObject collidingObject)
		{
			pac.CollisionEnter(bodyPartType, impactMagnitude);
			currentState.CollisionEnter();
		}

		public void ApplyTargetSpin()
		{
			float value = spinVelocityTarget - base.rigidbody.angularVelocity.z * 57.29578f;
			value = Mathf.Clamp(value, 0f - midAirTorque, midAirTorque);
			RotateInAir(value * magicSpinFactor);
		}

		private void RotateInAir(float torque)
		{
			base.rigidbody.AddRelativeTorque((0f - torque) * Vector3.up, ForceMode.Force);
		}

		public bool CheckImpact()
		{
			Vector3 normalized = base.rigidbody.velocity.normalized;
			Vector3 origin = base.rigidbody.position + normalized;
			return Physics.Raycast(origin, normalized, 4f);
		}

		public FlyingOrientation CheckFlyingOrientation()
		{
			Vector3 normalized = base.rigidbody.velocity.normalized;
			if (Vector3.Dot(normalized, -base.transform.right) > 0f)
			{
				return FlyingOrientation.OnHead;
			}
			return FlyingOrientation.OnFeet;
		}

		public override void HorizontalMove(Vector2 tDelta, int fingerCount, CriteriaState status)
		{
			switch (status)
			{
			case CriteriaState.Began:
				horizontalWindow = 0f;
				break;
			case CriteriaState.Ended:
				horizontalWindow = 0f;
				break;
			case CriteriaState.Moved:
				horizontalWindow = Mathf.Clamp(horizontalWindow + tDelta.x, -200f, 200f);
				if (horizontalWindow < -5f)
				{
					spinVelocityTarget = -1f * (horizontalWindow / (float)Screen.width) * maxSpinVelocity;
					ManualSpin = true;
				}
				else if (horizontalWindow > 5f)
				{
					spinVelocityTarget = -1f * (horizontalWindow / (float)Screen.width) * maxSpinVelocity;
					ManualSpin = true;
				}
				else
				{
					spinVelocityTarget = 0f;
					ManualSpin = true;
				}
				break;
			}
		}
	}
}
