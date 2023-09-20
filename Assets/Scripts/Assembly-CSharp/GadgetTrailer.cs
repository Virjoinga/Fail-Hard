using Game;
using UnityEngine;

public class GadgetTrailer : Gadget
{
	public ConfigurableJoint VehicleConnector;

	public override void Equip(VehicleBase vehicle, Rigidbody rb, VehiclePart vp)
	{
		base.State = GadgetState.GadgetOff;
		VehicleConnector.connectedBody = rb;
		base.VehiclePart = vp;
	}
}
