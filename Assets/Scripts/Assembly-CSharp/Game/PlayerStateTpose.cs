using UnityEngine;

namespace Game
{
	public class PlayerStateTpose : PlayerState
	{
		public override void AssignVehicle(VehicleBase vehicle)
		{
			EnableColliders(base.transform, false);
			GetComponent<SkinnedMeshRenderer>().enabled = false;
			PositionPlayerInVehicle(vehicle.pose);
			SetPose(vehicle.PoseAnimationName);
		}

		private void SetPose(string poseAnimationName)
		{
			pac.PoseReady += PoseReady;
			pac.PosePhysics(poseAnimationName, WrapMode.ClampForever, 100f);
		}

		private void PoseReady()
		{
			AttachToVehicle(player.handR.gameObject);
			AttachToVehicle(player.handL.gameObject);
			AttachToVehicle(player.footR.gameObject);
			AttachToVehicle(player.footL.gameObject);
			EnableColliders(base.transform, true);
			player.handR.collider.enabled = false;
			player.handL.collider.enabled = false;
			player.footL.collider.enabled = false;
			player.footR.collider.enabled = false;
			GetComponent<SkinnedMeshRenderer>().enabled = true;
			pac.PoseReady -= PoseReady;
			pac.DrivingPhysics(true);
			player.currentState = base.gameObject.GetComponent<PlayerStateIdle>();
		}

		public void PositionPlayerInVehicle(DudePose pose)
		{
			root.LookAt(root.position + pose.root.right);
			Vector3 translation = pose.root.position - base.rigidbody.position;
			root.Translate(translation, Space.World);
		}

		private void EnableColliders(Transform node, bool enabled)
		{
			if ((bool)node.gameObject.collider)
			{
				node.gameObject.collider.enabled = enabled;
			}
			foreach (Transform item in node)
			{
				if ((bool)item.gameObject.collider)
				{
					item.gameObject.collider.enabled = enabled;
				}
				EnableColliders(item, enabled);
			}
		}

		public HingeJoint AttachToVehicle(GameObject obj)
		{
			HingeJoint hingeJoint = obj.AddComponent("HingeJoint") as HingeJoint;
			hingeJoint.axis = Vector3.up;
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.connectedBody = player.currentVehicle.rigidbody;
			return hingeJoint;
		}
	}
}
