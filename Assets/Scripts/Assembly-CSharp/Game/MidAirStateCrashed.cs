using UnityEngine;

namespace Game
{
	public class MidAirStateCrashed : MidAirState
	{
		private PlayerStatus bodyStatus;

		private float stuntOverTimeout = 3f;

		private float crashTime;

		private float stuntOverVelocityThreshold = 0.1f;

		private bool stuntOver;

		private bool prepareForStuntOver;

		public override void Enter()
		{
			midair.player.BigAirFrames = 0f;
			bodyStatus = GetComponent<PlayerStatus>();
			crashTime = Time.time;
			midair.player.SetPhysicsConstraints(RigidbodyConstraints.None);
			midair.player.SignalPlayerCrashed();
		}

		public override void Exit()
		{
		}

		public override void FixedUpdateFunc()
		{
			if (!stuntOver)
			{
				if (!bodyStatus.IsContact())
				{
					midair.ChangeState("MidAirStateCrashing");
				}
				else if (midair.player.rigidbody.velocity.sqrMagnitude > stuntOverVelocityThreshold)
				{
					crashTime = Time.time;
				}
				if (Time.time > crashTime + stuntOverTimeout)
				{
					stuntOver = true;
					midair.player.gigController.PlayerNotMoving();
				}
				else if (!prepareForStuntOver && Time.time > crashTime + 0.25f * stuntOverTimeout)
				{
					prepareForStuntOver = true;
					midair.player.gigController.PlayerCrashed();
				}
			}
		}

		public override void CollisionExit()
		{
		}
	}
}
