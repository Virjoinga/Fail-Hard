using Game;
using UnityEngine;

public class GadgetWithBones : Gadget
{
	private Rigidbody m_connectedBody;

	public override void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		base.State = GadgetState.GadgetOff;
		m_connectedBody = base.transform.parent.rigidbody;
		base.VehiclePart = vp;
		m_player = player;
		GameObject gameObject = GetComponentInChildren<Rigidbody>().gameObject;
		ConfigurableJoint configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		gameObject.GetComponent<Rigidbody>().isKinematic = false;
		gameObject.GetComponent<ConfigurableJoint>().connectedBody = m_connectedBody;
	}
}
