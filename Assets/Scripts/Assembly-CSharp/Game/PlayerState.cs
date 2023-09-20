using UnityEngine;

namespace Game
{
	public class PlayerState : MonoBehaviour
	{
		protected Transform root;

		public PlayerAnimationController pac;

		public Player player;

		private void Start()
		{
			root = FindRoot(base.transform);
			pac = root.GetComponentInChildren<PlayerAnimationController>();
			player = root.GetComponentInChildren<Player>();
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

		public virtual void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude)
		{
		}

		public virtual void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude, GameObject collidingObject)
		{
		}

		public virtual void AssignVehicle(VehicleBase vehicle)
		{
			Debug.LogWarning("AssignVehicle dummy implementation called.");
		}

		public virtual void JumpOffVehicle(bool releaseOnly)
		{
		}

		protected Transform FindRoot(Transform node)
		{
			while (node != null)
			{
				if (node.parent == null)
				{
					return node;
				}
				node = node.parent;
			}
			return node;
		}

		public virtual void DetectSpeed(Gesture g)
		{
		}

		public virtual void DetectJump(Gesture g)
		{
		}

		public virtual void DetectCircle(Gesture g)
		{
		}

		public virtual void DetectTap(Gesture g)
		{
		}

		public virtual void DetectDualTap(Gesture g)
		{
		}

		public virtual void Release(Vector2 delta, int fingerCount, CriteriaState status)
		{
		}

		public virtual void JumpWithSpin(Vector2 delta, int fingerCount, CriteriaState status)
		{
		}

		public virtual void HorizontalMove(Vector2 tDelta, int fingerCount, CriteriaState status)
		{
		}
	}
}
