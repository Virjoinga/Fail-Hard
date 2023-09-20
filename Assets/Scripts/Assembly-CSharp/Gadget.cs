using Game;
using UnityEngine;

public class Gadget : MonoBehaviour
{
	public enum GadgetState
	{
		GadgetOff = 0,
		GadgetOn = 1
	}

	public float PlayerSpinMultiplier;

	public float VehicleSpinMultiplier;

	protected Player m_player;

	public GadgetState State { get; protected set; }

	public VehiclePart VehiclePart { get; protected set; }

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void PreviewEquip(Transform player)
	{
	}

	public virtual void PreviewUnequip(Transform player)
	{
	}

	public virtual void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		VehiclePart = vp;
	}

	public virtual void Equip(VehicleBase vehicle, Rigidbody rb, VehiclePart vp)
	{
		VehiclePart = vp;
	}

	public virtual void PlayerMoving()
	{
	}

	public virtual void Action()
	{
	}

	public virtual void PlayerJumpWithVehicle()
	{
	}

	public virtual void PlayerJumpOff()
	{
	}
}
