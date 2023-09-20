using UnityEngine;

namespace Game
{
	public class MidAirState : MonoBehaviour
	{
		public PlayerStateMidAir midair;

		private void Start()
		{
			midair = base.gameObject.GetComponent<PlayerStateMidAir>();
		}

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void UpdateFunc()
		{
		}

		public virtual void FixedUpdateFunc()
		{
		}

		public virtual void CollisionEnter()
		{
		}

		public virtual void CollisionExit()
		{
		}
	}
}
