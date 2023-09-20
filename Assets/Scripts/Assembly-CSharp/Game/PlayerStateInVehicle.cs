using System;
using UnityEngine;

namespace Game
{
	public class PlayerStateInVehicle : PlayerState
	{
		private DrivingState drivingState = DrivingState.Accelerating;

		private float horizontalWindow_speed;

		private float horizontalWindow_manualSpin;

		private float horizontalWindow_manualSpind;

		private bool fullBrake;

		private float verticalWindow_jump;

		private float horizontalWindow_jump;

		public float rotateAnimationAngularVelocity = 180f;

		private bool jumpingState;

		public float maxManualSpinForce = 300f;

		private float manualSpinForce;

		private bool manualSpinOverride;

		private float spinVelocityTarget;

		private float magicSpinFactor = 7f;

		private float midAirTorque = 100f;

		private float defaultSpin = -75f;

		private bool usingNewThrottleCtrl;

		private bool usingTouchCtrl = true;

		private UILabel releaseIcon;

		private UILabel smallJumpIcon;

		private UILabel jumpIcon;

		private UILabel spinFWIcon;

		private UILabel spinBWIcon;

		private float currentRotateVelocity;

		public override void UpdateFunc()
		{
			if (Input.GetKey(KeyCode.D))
			{
				usingTouchCtrl = false;
			}
			if (usingTouchCtrl)
			{
				return;
			}
			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.I))
			{
				PerformJump(JumpingStyle.JumpOff);
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
			{
				PerformJump(JumpingStyle.SmallJumpOff);
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U))
			{
				PerformJump(JumpingStyle.JumpOffBWSpin);
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
			{
				PerformJump(JumpingStyle.JumpOffFWSpin);
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.J))
			{
				PerformJump(JumpingStyle.SmallJumpOffBWSpin);
			}
			else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
			{
				PerformJump(JumpingStyle.SmallJumpOffFWSpin);
			}
			else if (Input.GetKeyUp(KeyCode.LeftShift))
			{
				PerformJump(JumpingStyle.Release);
			}
			else if (Input.GetKeyDown(KeyCode.I))
			{
				PerformJump(JumpingStyle.Jump);
			}
			else if (Input.GetKeyDown(KeyCode.O))
			{
				PerformJump(JumpingStyle.JumpFWSpin);
			}
			else if (Input.GetKeyDown(KeyCode.U))
			{
				PerformJump(JumpingStyle.JumpBWSpin);
			}
			else if (Input.GetKeyDown(KeyCode.K))
			{
				PerformJump(JumpingStyle.SmallJump);
			}
			else if (Input.GetKeyDown(KeyCode.L))
			{
				PerformJump(JumpingStyle.SmallJumpFWSpin);
			}
			else if (Input.GetKeyDown(KeyCode.J))
			{
				PerformJump(JumpingStyle.SmallJumpBWSpin);
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				player.currentVehicle.SetTargetThrottle(0.2f);
				usingNewThrottleCtrl = true;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				player.currentVehicle.SetTargetThrottle(0.4f);
				usingNewThrottleCtrl = true;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				player.currentVehicle.SetTargetThrottle(0.6f);
				usingNewThrottleCtrl = true;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				player.currentVehicle.SetTargetThrottle(0.8f);
				usingNewThrottleCtrl = true;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				player.currentVehicle.SetTargetThrottle(1f);
				usingNewThrottleCtrl = true;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (Input.GetKey(KeyCode.A))
				{
					PerformJump(JumpingStyle.JumpBWSpin);
				}
				else if (Input.GetKey(KeyCode.D))
				{
					PerformJump(JumpingStyle.JumpFWSpin);
				}
				else
				{
					PerformJump(JumpingStyle.Jump);
				}
			}
		}

		public DrivingState getDrivingState()
		{
			return drivingState;
		}

		public override void FixedUpdateFunc()
		{
			if (!player)
			{
				return;
			}
			bool front;
			bool rear;
			player.currentVehicle.IsGrounded(out front, out rear);
			if (!usingTouchCtrl)
			{
				if (Input.GetKey(KeyCode.A) && (bool)player.currentVehicle)
				{
					RotateRoot(rotateAnimationAngularVelocity);
					player.currentVehicle.SetBrake(0.3f);
					player.currentVehicle.AddThrottle(0f - Time.deltaTime);
					usingNewThrottleCtrl = false;
					spinVelocityTarget = 0f;
					if (!rear)
					{
						manualSpinForce = maxManualSpinForce;
						manualSpinOverride = true;
						spinVelocityTarget = manualSpinForce;
					}
					else
					{
						manualSpinForce = 0f;
					}
				}
				else if (Input.GetKey(KeyCode.D) && (bool)player.currentVehicle)
				{
					RotateRoot(0f - rotateAnimationAngularVelocity);
					player.currentVehicle.AddThrottle(Time.deltaTime);
					usingNewThrottleCtrl = false;
					spinVelocityTarget = 0f;
					if (!front)
					{
						manualSpinForce = 0f - maxManualSpinForce;
						manualSpinOverride = true;
						spinVelocityTarget = manualSpinForce;
					}
					else
					{
						manualSpinForce = 0f;
					}
				}
				else
				{
					manualSpinForce = 0f;
					if (manualSpinOverride)
					{
						spinVelocityTarget = manualSpinForce;
					}
					RotateRootTowards(0f, 3f);
					if ((bool)player.currentVehicle)
					{
						player.currentVehicle.SetBrake(0f);
						if (!usingNewThrottleCtrl)
						{
							player.currentVehicle.IdleThrottle();
						}
					}
				}
				if (Input.GetKey(KeyCode.T))
				{
					DetectTap(null);
				}
			}
			MoveRootWithConstantSpeed(0.05f * Vector3.right, 0.5f);
			if (!player.currentVehicle.IsGrounded())
			{
				float value = spinVelocityTarget - CalculateSpinVelocity();
				value = Mathf.Clamp(value, 0f - midAirTorque, midAirTorque);
				RotateInAir(value * magicSpinFactor);
			}
			else if (!jumpingState)
			{
				spinVelocityTarget = 0f;
			}
		}

		public override void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude)
		{
			if (bodyPartType == BodyPartType.Head || bodyPartType == BodyPartType.Torso)
			{
				JumpOffVehicle(0f, 0f);
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

		private void RotateRootTowards(float target, float factor)
		{
			if ((bool)player.vehicleConnector)
			{
				Vector3 eulerAngles = player.vehicleConnector.targetRotation.eulerAngles;
				if (eulerAngles.y > 180f)
				{
					eulerAngles.y -= 360f;
				}
				float num = target - eulerAngles.y;
				RotateRoot(num * factor);
			}
		}

		private void RotateRoot(float delta)
		{
			if ((bool)player.vehicleConnector)
			{
				Vector3 eulerAngles = player.vehicleConnector.targetRotation.eulerAngles;
				eulerAngles.y += Time.deltaTime * delta;
				if (eulerAngles.y > 40f && eulerAngles.y < 180f)
				{
					eulerAngles.y = 40f;
				}
				else if (eulerAngles.y > 180f && eulerAngles.y < 320f)
				{
					eulerAngles.y = 320f;
				}
				player.vehicleConnector.targetRotation = Quaternion.Euler(eulerAngles);
			}
		}

		private void MoveRootTowards(Vector3 target, float factor)
		{
			if ((bool)player.vehicleConnector)
			{
				Vector3 vector = target - player.vehicleConnector.targetPosition;
				player.vehicleConnector.targetPosition += factor * vector * Time.deltaTime;
			}
		}

		private void MoveRootWithConstantSpeed(Vector3 target, float speed)
		{
			if ((bool)player.vehicleConnector)
			{
				Vector3 vector = target - player.vehicleConnector.targetPosition;
				if (vector.sqrMagnitude > 0.0001f)
				{
					player.vehicleConnector.targetPosition += speed * vector.normalized * Time.deltaTime;
				}
			}
		}

		private void JumpWithVehicle(float mjumpingForce, float mjumpingSpin)
		{
			if (!player.vehicleConnector || jumpingState)
			{
				return;
			}
			foreach (Gadget value in player.CurrentGadgets.Values)
			{
				value.PlayerJumpWithVehicle();
			}
			jumpingState = true;
			manualSpinOverride = false;
			Invoke("ResetJumpingState", 0.5f);
			Quaternion targetRotation = player.vehicleConnector.targetRotation;
			targetRotation.y = 0f;
			player.vehicleConnector.targetRotation = targetRotation;
			player.vehicleConnector.targetPosition = new Vector3(0.5f, 0f, -0.1f);
			Vector3 normalized = (player.currentVehicle.transform.up + player.currentVehicle.JumpDirectionModifier).normalized;
			if (player.currentVehicle.IsGrounded())
			{
				base.rigidbody.AddForce(0.5f * mjumpingForce * normalized, ForceMode.VelocityChange);
				Vector3 position = player.currentVehicle.gameObject.rigidbody.position + new Vector3(mjumpingSpin, 0f, 0f);
				player.currentVehicle.gameObject.rigidbody.AddForceAtPosition(0.5f * mjumpingForce * normalized, position, ForceMode.VelocityChange);
			}
		}

		private void JumpOffVehicle(float mjumpingForce, float mjumpingSpin)
		{
			if (!player.vehicleConnector)
			{
				return;
			}
			foreach (Gadget value in player.CurrentGadgets.Values)
			{
				value.PlayerJumpOff();
			}
			player.currentVehicle.SetTargetThrottle(0f);
			DetachFromVehicle();
			UnityEngine.Object.Destroy(player.vehicleConnector);
			player.currentVehicle.gameObject.rigidbody.constraints = RigidbodyConstraints.None;
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
			usingTouchCtrl = true;
		}

		public override void DetectJump(Gesture g)
		{
			if (drivingState == DrivingState.Accelerating)
			{
				drivingState = DrivingState.PreparingJump;
			}
			if (player.currentVehicle.IsGrounded())
			{
				HandleJumpOnGround(g);
			}
			else
			{
				HandleJumpOnAir(g);
			}
		}

		private void HandleJumpOnGround(Gesture g)
		{
			Vector2 cumulativeDelta = g.nodes[0].criteria.cumulativeDelta;
			if (g.nodes[0].criteria.fingerCount == 2)
			{
				PerformJump(JumpingStyle.JumpOff);
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (cumulativeDelta.y < 0f)
			{
				return;
			}
			if (cumulativeDelta.x > 1f)
			{
				float num = cumulativeDelta.y / cumulativeDelta.x;
				if (num < 0.5f)
				{
					return;
				}
				if (num < 2f)
				{
					flag2 = true;
				}
			}
			else if (cumulativeDelta.x < -1f)
			{
				float num2 = cumulativeDelta.y / cumulativeDelta.x;
				if ((double)num2 > -0.5)
				{
					return;
				}
				if (num2 > -2f)
				{
					flag3 = true;
				}
			}
			if (!flag2 && !flag3)
			{
				PerformJump(JumpingStyle.Jump);
			}
			else if (flag2)
			{
				PerformJump(JumpingStyle.JumpFWSpin);
			}
			else if (flag3)
			{
				PerformJump(JumpingStyle.JumpBWSpin);
			}
		}

		private void HandleJumpOnAir(Gesture g)
		{
			if (g.nodes[0].criteria.fingerCount == 2)
			{
				PerformJump(JumpingStyle.JumpOff);
			}
			else
			{
				HandleJumpOnGround(g);
			}
		}

		public override void DetectCircle(Gesture g)
		{
		}

		public override void DetectTap(Gesture g)
		{
			if (!jumpingState)
			{
				spinVelocityTarget = 0f;
			}
		}

		public override void DetectDualTap(Gesture g)
		{
			PerformJump(JumpingStyle.Release);
		}

		public override void JumpWithSpin(Vector2 delta, int fingerCount, CriteriaState status)
		{
			bool flag = verticalWindow_jump > (float)(Screen.height / 2);
			bool flag2 = false;
			if (!flag)
			{
				flag2 = verticalWindow_jump > (float)(Screen.height / 8);
			}
			bool flag3 = fingerCount == 2;
			bool flag4 = horizontalWindow_jump > (float)(Screen.width / 8);
			bool flag5 = horizontalWindow_jump < (float)(-Screen.width / 8);
			bool flag6 = !flag4 && !flag5;
			switch (status)
			{
			case CriteriaState.Began:
				verticalWindow_jump = 0f;
				horizontalWindow_jump = 0f;
				break;
			case CriteriaState.Ended:
				if (drivingState == DrivingState.PreparingJump)
				{
					if (!flag && !flag2)
					{
						break;
					}
					if (!flag3)
					{
						drivingState = DrivingState.Accelerating;
					}
					if (flag && flag6 && flag3)
					{
						PerformJump(JumpingStyle.JumpOff);
					}
					else if (flag2 && flag6 && flag3)
					{
						PerformJump(JumpingStyle.SmallJumpOff);
					}
					else if (flag && flag4 && flag3)
					{
						PerformJump(JumpingStyle.JumpOffFWSpin);
					}
					else if (flag && flag5 && flag3)
					{
						PerformJump(JumpingStyle.JumpOffBWSpin);
					}
					else if (flag2 && flag4 && flag3)
					{
						PerformJump(JumpingStyle.SmallJumpOffFWSpin);
					}
					else if (flag2 && flag5 && flag3)
					{
						PerformJump(JumpingStyle.SmallJumpOffBWSpin);
					}
					else if (flag && flag6)
					{
						PerformJump(JumpingStyle.Jump);
					}
					else if (flag && flag4)
					{
						PerformJump(JumpingStyle.JumpFWSpin);
					}
					else if (flag && flag5)
					{
						PerformJump(JumpingStyle.JumpBWSpin);
					}
					else if (flag2 && flag6)
					{
						PerformJump(JumpingStyle.SmallJump);
					}
					else if (flag2 && flag4)
					{
						PerformJump(JumpingStyle.SmallJumpFWSpin);
					}
					else if (flag2 && flag5)
					{
						PerformJump(JumpingStyle.SmallJumpBWSpin);
					}
				}
				verticalWindow_jump = 0f;
				horizontalWindow_jump = 0f;
				break;
			case CriteriaState.Moved:
				verticalWindow_jump += delta.y;
				horizontalWindow_jump += delta.x;
				break;
			}
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
			case JumpingStyle.SmallJumpOff:
				JumpOffVehicle(0.5f * player.currentVehicle.MaxJumpOffForce, 0f);
				break;
			case JumpingStyle.JumpOffFWSpin:
				JumpOffVehicle(player.currentVehicle.MaxJumpOffForce, -1f);
				JumpOffWithSpin(-1225f);
				break;
			case JumpingStyle.JumpOffBWSpin:
				JumpOffVehicle(player.currentVehicle.MaxJumpOffForce, 1f);
				JumpOffWithSpin(1225f);
				break;
			case JumpingStyle.SmallJumpOffFWSpin:
				JumpOffVehicle(0.5f * player.currentVehicle.MaxJumpOffForce, -1f);
				JumpOffWithSpin(-1225f);
				break;
			case JumpingStyle.SmallJumpOffBWSpin:
				JumpOffVehicle(0.5f * player.currentVehicle.MaxJumpOffForce, 1f);
				JumpOffWithSpin(1225f);
				break;
			case JumpingStyle.Release:
				JumpOffVehicle(0f, 0f);
				break;
			case JumpingStyle.Jump:
				JumpWithVehicle(player.currentVehicle.MaxJumpingForce, 0f);
				spinVelocityTarget = defaultSpin;
				break;
			case JumpingStyle.JumpFWSpin:
				JumpWithVehicle(player.currentVehicle.MaxJumpingForce, -2f);
				spinVelocityTarget = Mathf.Clamp(spinVelocityTarget - player.currentVehicle.MaxSpinVelocity, 0f - player.currentVehicle.MaxSpinVelocity, player.currentVehicle.MaxSpinVelocity);
				break;
			case JumpingStyle.JumpBWSpin:
				JumpWithVehicle(player.currentVehicle.MaxJumpingForce, 2f);
				spinVelocityTarget = Mathf.Clamp(spinVelocityTarget + player.currentVehicle.MaxSpinVelocity, 0f - player.currentVehicle.MaxSpinVelocity, player.currentVehicle.MaxSpinVelocity);
				break;
			case JumpingStyle.SmallJump:
				JumpWithVehicle(0.5f * player.currentVehicle.MaxJumpingForce, 0f);
				break;
			case JumpingStyle.SmallJumpFWSpin:
				JumpWithVehicle(0.5f * player.currentVehicle.MaxJumpingForce, -2f);
				spinVelocityTarget = Mathf.Clamp(spinVelocityTarget - player.currentVehicle.MaxSpinVelocity, 0f - player.currentVehicle.MaxSpinVelocity, player.currentVehicle.MaxSpinVelocity);
				break;
			case JumpingStyle.SmallJumpBWSpin:
				JumpWithVehicle(0.5f * player.currentVehicle.MaxJumpingForce, 2f);
				spinVelocityTarget = Mathf.Clamp(spinVelocityTarget + player.currentVehicle.MaxSpinVelocity, 0f - player.currentVehicle.MaxSpinVelocity, player.currentVehicle.MaxSpinVelocity);
				break;
			}
		}

		public override void HorizontalMove(Vector2 tDelta, int fingerCount, CriteriaState status)
		{
			switch (status)
			{
			case CriteriaState.Began:
				horizontalWindow_speed = 0f;
				horizontalWindow_manualSpin = 0f;
				horizontalWindow_manualSpind = 0f;
				player.currentVehicle.SetBrake(0f);
				break;
			case CriteriaState.Ended:
				horizontalWindow_speed = 0f;
				horizontalWindow_manualSpin = 0f;
				horizontalWindow_manualSpind = 0f;
				manualSpinForce = 0f;
				player.currentVehicle.SetBrake(0f);
				currentRotateVelocity = 0f - rotateAnimationAngularVelocity;
				break;
			case CriteriaState.Moved:
			{
				horizontalWindow_speed = Mathf.Clamp(horizontalWindow_speed + 3f * tDelta.x, -Screen.width, 10f);
				if (horizontalWindow_speed < -10f)
				{
					player.currentVehicle.SetBrake(-1f * horizontalWindow_speed / (float)Screen.width);
					currentRotateVelocity = rotateAnimationAngularVelocity;
				}
				else if (horizontalWindow_speed > 9f)
				{
					player.currentVehicle.SetBrake(0f);
					currentRotateVelocity = 0f - rotateAnimationAngularVelocity;
				}
				else
				{
					currentRotateVelocity = 0f;
					player.currentVehicle.SetBrake(0f);
				}
				player.currentVehicle.AddThrottle(3f * tDelta.x / (float)Screen.width);
				bool front;
				bool rear;
				player.currentVehicle.IsGrounded(out front, out rear);
				horizontalWindow_manualSpin = Mathf.Clamp(horizontalWindow_manualSpin + 3f * tDelta.x, -Screen.width, Screen.width);
				horizontalWindow_manualSpind = Mathf.Clamp(horizontalWindow_manualSpin + 3f * tDelta.x, -20f, 20f);
				if (horizontalWindow_manualSpind < -19f)
				{
					if (horizontalWindow_manualSpin > 0f)
					{
						horizontalWindow_manualSpin = 0f;
					}
					else if (horizontalWindow_manualSpind > 19f && horizontalWindow_manualSpin < 0f)
					{
						horizontalWindow_manualSpin = 0f;
					}
				}
				if (horizontalWindow_manualSpin < -19f)
				{
					RotateRoot(rotateAnimationAngularVelocity);
					if (!jumpingState)
					{
						spinVelocityTarget = 0f;
					}
					if (!rear)
					{
						manualSpinForce = -1f * (horizontalWindow_manualSpin / (float)Screen.width) * maxManualSpinForce;
						manualSpinOverride = true;
						spinVelocityTarget = manualSpinForce;
					}
					else
					{
						manualSpinForce = 0f;
					}
				}
				else if (horizontalWindow_manualSpin > 19f)
				{
					RotateRoot(0f - rotateAnimationAngularVelocity);
					if (!jumpingState)
					{
						spinVelocityTarget = 0f;
					}
					if (!front)
					{
						manualSpinForce = -1f * (horizontalWindow_manualSpin / (float)Screen.width) * maxManualSpinForce;
						manualSpinOverride = true;
						spinVelocityTarget = manualSpinForce;
					}
					else
					{
						manualSpinForce = 0f;
					}
				}
				else
				{
					manualSpinForce = 0f;
					if (manualSpinOverride)
					{
						spinVelocityTarget = manualSpinForce;
					}
				}
				break;
			}
			}
			if (Mathf.Abs(currentRotateVelocity) > 0.1f)
			{
				RotateRoot(currentRotateVelocity);
			}
		}
	}
}
