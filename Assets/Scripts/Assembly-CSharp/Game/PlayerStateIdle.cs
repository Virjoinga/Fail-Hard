using UnityEngine;

namespace Game
{
	public class PlayerStateIdle : PlayerState
	{
		public override void UpdateFunc()
		{
			if (player.ControlsEnabled)
			{
				if (ButtonControlScheme.Instance.RightPressed)
				{
					ButtonControlScheme.Instance.Reset();
					Go();
				}
				else if (ButtonControlScheme.Instance.JumpPressed)
				{
					Go();
				}
			}
		}

		public override void FixedUpdateFunc()
		{
		}

		public override void DetectSpeed(Gesture g)
		{
			Go();
		}

		private void Go()
		{
			pac.DisableAnimatedPhysics();
			base.rigidbody.isKinematic = false;
			player.SetPhysicsConstraints((RigidbodyConstraints)56);
			player.currentVehicle.rigidbody.isKinematic = false;
			player.currentVehicle.rigidbody.constraints = (RigidbodyConstraints)56;
			player.gigController.PlayerMoving();
			if (player.currentVehicle.CurrentGadget != null)
			{
				player.currentVehicle.CurrentGadget.PlayerMoving();
			}
			player.currentState = base.gameObject.GetComponent<PlayerStateInHCRVehicle>();
			player.currentState.Enter();
		}
	}
}
