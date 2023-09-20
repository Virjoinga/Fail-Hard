namespace Game
{
	public class MidAirStateFreefall : MidAirState
	{
		public override void Enter()
		{
			midair.player.AnimationStatesModified += player_AnimationStatesModified;
			Invoke("JumpDelayOver", 0.2f);
		}

		private void player_AnimationStatesModified(Player.AnimationType animType, bool added)
		{
			if (!added && animType == Player.AnimationType.Freefall)
			{
				midair.pac.AnimatePhysics(midair.player.DefaultAnimationStates[Player.AnimationType.Freefall]);
			}
		}

		public override void Exit()
		{
		}

		private void JumpDelayOver()
		{
			if (midair.player.AnimationStates.ContainsKey(Player.AnimationType.Freefall))
			{
				midair.pac.AnimatePhysics(midair.player.AnimationStates[Player.AnimationType.Freefall]);
			}
			else
			{
				midair.pac.AnimatePhysics(midair.player.DefaultAnimationStates[Player.AnimationType.Freefall]);
			}
		}

		public override void FixedUpdateFunc()
		{
			midair.player.BigAirFrames += 1f;
			midair.ApplyTargetSpin();
			if (midair.CheckImpact())
			{
				midair.ChangeState("MidAirStateCrashing");
			}
		}

		public override void CollisionEnter()
		{
		}
	}
}
