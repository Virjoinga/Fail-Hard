using UnityEngine;

public class NpcStateTpose : NpcState
{
	public override void AssignObject(GameObject obj)
	{
		EnableColliders(base.transform, false);
		GetComponent<SkinnedMeshRenderer>().enabled = false;
		SetPose("PoseMoped_01");
		Invoke("PoseReady", 0.5f);
	}

	private void SetPose(string poseAnimationName)
	{
		pac.AnimatePhysics(poseAnimationName, WrapMode.ClampForever);
	}

	private void PoseReady()
	{
		AttachToVehicle(Npc.handR.gameObject);
		AttachToVehicle(Npc.handL.gameObject);
		EnableColliders(base.transform, true);
		GetComponent<SkinnedMeshRenderer>().enabled = true;
		Npc.currentState = base.gameObject.GetComponent<NpcStateIdle>();
		Npc.currentState.Enter();
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
		hingeJoint.connectedBody = Npc.ConnectedObject.rigidbody;
		hingeJoint.breakForce = 8f;
		return hingeJoint;
	}
}
