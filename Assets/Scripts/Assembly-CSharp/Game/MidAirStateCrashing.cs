namespace Game
{
	public class MidAirStateCrashing : MidAirState
	{
		public override void Enter()
		{
			if (midair.CheckFlyingOrientation() == FlyingOrientation.OnHead)
			{
				if (midair.player.AnimationStates.ContainsKey(Player.AnimationType.CrashOnHead))
				{
					midair.pac.AnimatePhysics(midair.player.AnimationStates[Player.AnimationType.CrashOnHead]);
				}
				else
				{
					midair.pac.AnimatePhysics(midair.player.DefaultAnimationStates[Player.AnimationType.CrashOnHead]);
				}
			}
			else if (midair.player.AnimationStates.ContainsKey(Player.AnimationType.CrashOnFeet))
			{
				midair.pac.AnimatePhysics(midair.player.AnimationStates[Player.AnimationType.CrashOnFeet]);
			}
			else
			{
				midair.pac.AnimatePhysics(midair.player.DefaultAnimationStates[Player.AnimationType.CrashOnFeet]);
			}
		}

		public override void Exit()
		{
		}

		public override void UpdateFunc()
		{
		}

		public override void FixedUpdateFunc()
		{
			midair.player.BigAirFrames += 1f;
			midair.ApplyTargetSpin();
			if (!midair.CheckImpact())
			{
				midair.ChangeState("MidAirStateFreefall");
			}
		}

		public override void CollisionEnter()
		{
			midair.ChangeState("MidAirStateCrashed");
		}
	}
}
