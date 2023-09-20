using System;
using UnityEngine;

namespace Game
{
	public class PlayerStateInHCRVehicle : PlayerState
	{
		public enum DrivingState
		{
			Idle = 0,
			Accelerating = 1,
			PreparingJump = 2
		}

		private enum JumpingStyle
		{
			JumpOff = 0,
			SmallJumpOff = 1,
			JumpOffFWSpin = 2,
			JumpOffBWSpin = 3,
			SmallJumpOffFWSpin = 4,
			SmallJumpOffBWSpin = 5,
			Release = 6,
			Jump = 7,
			JumpFWSpin = 8,
			JumpBWSpin = 9,
			SmallJump = 10,
			SmallJumpFWSpin = 11,
			SmallJumpBWSpin = 12
		}

		private DrivingState drivingState = DrivingState.Accelerating;

		private bool fullBrake;

		private bool jumpingState;

		private bool m_jumpedOff;

		private bool m_groundedInPrevFrame;

		private Flip m_currentFlip;

		private float spinVelocityTarget;

		private float magicSpinFactor = 7f;

		private float midAirTorque = 100f;

		private float defaultSpin = -75f;

		private float m_maxThrottleTimeS = 0.1f;

		private float m_maxSlowdownTimeS = 0.05f;

		private UILabel releaseIcon;

		private UILabel smallJumpIcon;

		private UILabel jumpIcon;

		private UILabel spinFWIcon;

		private UILabel spinBWIcon;

		private float DefaultSpin
		{
			get
			{
				if (player.InvertedControls)
				{
					return defaultSpin;
				}
				return defaultSpin;
			}
		}

		public override void UpdateFunc()
		{
		}

		public DrivingState getDrivingState()
		{
			return drivingState;
		}

		private float CalculateSpinVelocity(bool isCcw, float current, bool smooth)
		{
			float num = player.currentVehicle.AdjustedSpinVelocity;
			if (isCcw)
			{
				num = 0f - player.currentVehicle.AdjustedSpinVelocity;
			}
			if (smooth)
			{
				num = ((!(num > 0f)) ? Mathf.Clamp(current + Time.fixedDeltaTime * num / 0.5f, num, 0f) : Mathf.Clamp(current + Time.fixedDeltaTime * num / 0.5f, 0f, num));
			}
			return num;
		}

		public override void FixedUpdateFunc()
		{
			if (!player)
			{
				return;
			}
			bool front;
			bool rear;
			if (!player.currentVehicle.IsGrounded(out front, out rear))
			{
				player.BigAirFrames += 1f;
			}
			else
			{
				player.BigAirFrames = 0f;
			}
			HandleFlipRecognition();
			if (ButtonControlScheme.Instance.JumpDown)
			{
				HandleJumpButton();
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
			if (ButtonControlScheme.Instance.UsingButtonScheme)
			{
				if (ButtonControlScheme.Instance.LeftPressed && (bool)player.currentVehicle)
				{
					player.currentVehicle.AnimateOnBrake(player.vehicleConnector);
					player.currentVehicle.SetBrake(1f);
					if (ButtonControlScheme.Instance.RightPressed)
					{
						player.currentVehicle.AddThrottle(Time.fixedDeltaTime / m_maxThrottleTimeS);
					}
					else
					{
						player.currentVehicle.AddThrottle((0f - Time.fixedDeltaTime) / m_maxSlowdownTimeS);
					}
					if (!front)
					{
						if (rear)
						{
							player.currentVehicle.rigidbody.AddTorque(player.currentVehicle.BalanceHelpBrakeWhileFrontUp * Vector3.forward);
							spinVelocityTarget = 0f - player.currentVehicle.AdjustedSpinVelocity;
						}
						else if (jumpingState)
						{
							spinVelocityTarget = CalculateSpinVelocity(!player.InvertedControls, spinVelocityTarget, false);
						}
						else
						{
							spinVelocityTarget = CalculateSpinVelocity(!player.InvertedControls, spinVelocityTarget, true);
						}
					}
					else if (!rear && !player.currentVehicle.InJumpZone)
					{
						spinVelocityTarget = 0.5f * player.currentVehicle.AdjustedSpinVelocity;
					}
				}
				else if (ButtonControlScheme.Instance.RightPressed && (bool)player.currentVehicle)
				{
					player.currentVehicle.AnimateOnThrottle(player.vehicleConnector, base.rigidbody);
					player.currentVehicle.AddThrottle(Time.fixedDeltaTime / m_maxThrottleTimeS);
					player.currentVehicle.SetBrake(0f);
					if (!rear)
					{
						if (front)
						{
							spinVelocityTarget = player.currentVehicle.AdjustedSpinVelocity;
							if (player.currentVehicle.rigidbody.angularVelocity.z < 0f)
							{
								player.currentVehicle.rigidbody.AddTorque(player.currentVehicle.BalanceHelpThrottleWhileRearUp * Vector3.forward);
							}
						}
						else if (jumpingState)
						{
							spinVelocityTarget = CalculateSpinVelocity(player.InvertedControls, spinVelocityTarget, false);
						}
						else
						{
							spinVelocityTarget = CalculateSpinVelocity(player.InvertedControls, spinVelocityTarget, true);
						}
					}
					else if (!front && player.currentVehicle.rigidbody.angularVelocity.z > 0f)
					{
						player.currentVehicle.rigidbody.AddTorque(player.currentVehicle.BalanceHelpThrottleWhileFrontUp * Vector3.forward);
					}
				}
				else
				{
					spinVelocityTarget = DefaultSpin;
					player.currentVehicle.AnimateRotationIdle(player.vehicleConnector);
					if ((bool)player.currentVehicle)
					{
						player.currentVehicle.SetBrake(0f);
						player.currentVehicle.IdleThrottle();
					}
				}
			}
			player.currentVehicle.AnimatePositionIdle(player.vehicleConnector);
			if (!player.currentVehicle.IsGrounded())
			{
				float value = spinVelocityTarget - CalculateSpinVelocity();
				value = Mathf.Clamp(value, 0f - midAirTorque, midAirTorque);
				RotateInAir(value * magicSpinFactor);
			}
			else if (!jumpingState)
			{
				spinVelocityTarget = DefaultSpin;
			}
			if (m_jumpedOff)
			{
				player.currentVehicle.SetTargetThrottle(0f);
			}
		}

		private void HandleFlipRecognition()
		{
			bool front;
			bool rear;
			if (!player.currentVehicle.IsGrounded(out front, out rear))
			{
				if (!m_groundedInPrevFrame)
				{
					if (m_currentFlip == null)
					{
						m_currentFlip = new Flip();
					}
					m_currentFlip.Reset(true, player.currentVehicle.transform);
				}
				m_groundedInPrevFrame = true;
			}
			else
			{
				if (m_groundedInPrevFrame && m_currentFlip != null)
				{
					m_currentFlip = null;
				}
				m_groundedInPrevFrame = false;
			}
			if (m_currentFlip != null)
			{
				if (m_currentFlip.IsCw && ButtonControlScheme.Instance.RightPressed && (bool)player.currentVehicle)
				{
					m_currentFlip.Reset(false, player.currentVehicle.transform);
				}
				else if (!m_currentFlip.IsCw && ButtonControlScheme.Instance.LeftPressed && (bool)player.currentVehicle)
				{
					m_currentFlip.Reset(true, player.currentVehicle.transform);
				}
				if (m_currentFlip.Update(player.currentVehicle.transform))
				{
					player.Flip();
					m_currentFlip = new Flip();
				}
			}
		}

		private void HandleJumpButton()
		{
			if (jumpingState)
			{
				PerformJump(JumpingStyle.JumpOff);
			}
			else if (player.currentVehicle.IsGrounded())
			{
				PerformJump(JumpingStyle.Jump);
			}
			else
			{
				PerformJump(JumpingStyle.JumpOff);
			}
		}

		public override void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude)
		{
			if ((bodyPartType != 0 || !player.CurrentGadgets.ContainsKey(VehiclePartType.PlayerGadgetHead) || !(player.CurrentGadgets[VehiclePartType.PlayerGadgetHead].VehiclePart.Id == "60")) && (bodyPartType == BodyPartType.Head || bodyPartType == BodyPartType.Torso))
			{
				JumpOffVehicle(0f, 0f);
			}
		}

		public override void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude, GameObject collidingObject)
		{
			if (collidingObject.tag == "Vehicle")
			{
				return;
			}
			if (bodyPartType == BodyPartType.Head && player.CurrentGadgets.ContainsKey(VehiclePartType.PlayerGadgetHead))
			{
				if (player.CurrentGadgets[VehiclePartType.PlayerGadgetHead].VehiclePart.HasProtection)
				{
					return;
				}
			}
			else if (bodyPartType == BodyPartType.Torso && player.CurrentGadgets.ContainsKey(VehiclePartType.PlayerGadgetBack) && player.CurrentGadgets[VehiclePartType.PlayerGadgetBack].VehiclePart.HasProtection)
			{
				return;
			}
			if (bodyPartType == BodyPartType.Head || bodyPartType == BodyPartType.Torso)
			{
				Rigidbody component = collidingObject.GetComponent<Rigidbody>();
				if (component == null || component.mass > 2f || component.isKinematic)
				{
					JumpOffVehicle(0f, 0f, true);
				}
			}
		}

		private float CalculateSpinVelocity()
		{
			Vector3 vector = base.rigidbody.position - player.currentVehicle.rigidbody.position;
			Vector3 vector2 = Vector3.Project(player.currentVehicle.rigidbody.velocity, player.currentVehicle.transform.forward);
			Vector3 vector3 = Vector3.Project(base.rigidbody.velocity, player.currentVehicle.transform.forward);
			Vector3 lhs = vector2 - vector3;
			float num = lhs.magnitude / (vector.magnitude * (float)Math.PI) * 360f;
			if (Vector3.Dot(lhs, player.currentVehicle.transform.forward) < 0f)
			{
				return 0f - num;
			}
			return num;
		}

		private void JumpWithVehicle(float mjumpingForce, float mjumpingSpin)
		{
			if ((bool)player.vehicleConnector && !jumpingState)
			{
				player.SignalPlayerJumped();
				jumpingState = true;
				Invoke("ResetJumpingState", 0.5f);
				player.currentVehicle.AnimateOnJump(player.vehicleConnector);
				Vector3 force = player.currentVehicle.JumpWithVehicle();
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			}
		}

		public override void JumpOffVehicle(bool releaseOnly)
		{
			if (releaseOnly)
			{
				PerformJump(JumpingStyle.Release);
			}
			else
			{
				PerformJump(JumpingStyle.JumpOff);
			}
		}

		private void JumpOffVehicle(float mjumpingForce, float mjumpingSpin, bool fallApart = false)
		{
			if ((bool)player.vehicleConnector)
			{
				player.SignalPlayerJumpedOff();
				m_jumpedOff = true;
				player.currentVehicle.SetTargetThrottle(0f);
				DetachFromVehicle();
				player.handR.collider.enabled = true;
				player.handL.collider.enabled = true;
				player.footR.collider.enabled = true;
				player.footL.collider.enabled = true;
				if (fallApart || (player.currentVehicle.vehicleData.MaxCondition > 0 && player.currentVehicle.vehicleData.CurrentCondition <= 1))
				{
					player.currentVehicle.FallApart();
				}
				UnityEngine.Object.Destroy(player.vehicleConnector);
				player.currentVehicle.ReleaseConstraints();
				Vector3 up = player.currentVehicle.transform.up;
				float num = Vector3.Angle(Vector3.up, up);
				float num2 = 1f;
				float num3 = (0f - num2) * num / 180f + num2 + 1f;
				base.rigidbody.drag = 0.5f;
				base.rigidbody.AddForce((1f - player.currentVehicle.JumpOffForceAbsorb) * num3 * mjumpingForce * up, ForceMode.VelocityChange);
				player.currentVehicle.gameObject.rigidbody.AddForce((0f - player.currentVehicle.JumpOffForceAbsorb) * mjumpingForce * up, ForceMode.VelocityChange);
				HeadTracking componentInChildren = GetComponentInChildren<HeadTracking>();
				componentInChildren.DisableTracking();
				UnityEngine.Object.Destroy(componentInChildren);
				pac.DrivingPhysics(false);
				player.currentState = base.gameObject.GetComponent<PlayerStateMidAir>();
				if (player.currentVehicle.IsGrounded())
				{
					player.gameObject.GetComponent<PlayerStateMidAir>().Enter(mjumpingSpin);
				}
				else
				{
					player.gameObject.GetComponent<PlayerStateMidAir>().Enter(0f);
				}
			}
		}

		private void DetachFromVehicle()
		{
			UnityEngine.Object.Destroy(player.footR.GetComponent<HingeJoint>());
			UnityEngine.Object.Destroy(player.footL.GetComponent<HingeJoint>());
			UnityEngine.Object.Destroy(player.handR.GetComponent<HingeJoint>());
			UnityEngine.Object.Destroy(player.handL.GetComponent<HingeJoint>());
		}

		private void JumpOffWithSpin(float torque)
		{
			base.rigidbody.AddTorque(torque * Vector3.forward, ForceMode.VelocityChange);
		}

		private void RotateInAir(float torque)
		{
			player.currentVehicle.gameObject.rigidbody.AddRelativeTorque((0f - torque) * Vector3.right);
		}

		public override void DetectSpeed(Gesture g)
		{
		}

		public override void DetectJump(Gesture g)
		{
		}

		public override void DetectCircle(Gesture g)
		{
		}

		public override void DetectTap(Gesture g)
		{
		}

		public override void DetectDualTap(Gesture g)
		{
			PerformJump(JumpingStyle.Release);
		}

		public override void JumpWithSpin(Vector2 delta, int fingerCount, CriteriaState status)
		{
		}

		private void ResetJumpingState()
		{
			jumpingState = false;
		}

		private void PerformJump(JumpingStyle style)
		{
			GameController.Instance.Character.Player.GetComponentInChildren<PlayerSounds>().playJumpSound();
			switch (style)
			{
			case JumpingStyle.JumpOff:
				JumpOffVehicle(player.currentVehicle.MaxJumpOffForce, 0f);
				break;
			case JumpingStyle.Release:
				JumpOffVehicle(0f, 0f);
				break;
			case JumpingStyle.Jump:
				JumpWithVehicle(player.currentVehicle.MaxJumpingForce, 0f);
				spinVelocityTarget = DefaultSpin;
				break;
			default:
				Debug.LogError("Invalid Jump style!");
				break;
			}
		}

		public override void HorizontalMove(Vector2 tDelta, int fingerCount, CriteriaState status)
		{
		}
	}
}
