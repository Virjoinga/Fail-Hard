using Game;
using UnityEngine;

public class GadgetFlyingCap : Gadget
{
	public GameObject FixedCap;

	public GameObject DynamicCap;

	public float FlyingThresholdKmh;

	private Rigidbody connectedBody;

	public override void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		base.State = GadgetState.GadgetOn;
		connectedBody = rb;
		base.VehiclePart = vp;
		m_player = player;
	}

	private void Update()
	{
		if (base.State != 0)
		{
			Vector3 lhs = connectedBody.GetPointVelocity(base.transform.position) * 3.6f;
			if (lhs.magnitude > FlyingThresholdKmh && Vector3.Dot(lhs, base.transform.up) > 0f)
			{
				base.State = GadgetState.GadgetOff;
				FixedCap.SetActive(false);
				DynamicCap.SetActive(true);
				DynamicCap.transform.parent = null;
				DynamicCap.rigidbody.AddForceAtPosition(base.transform.up * 10f, base.transform.position + 0.1f * base.transform.forward, ForceMode.Impulse);
			}
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(DynamicCap);
	}
}
