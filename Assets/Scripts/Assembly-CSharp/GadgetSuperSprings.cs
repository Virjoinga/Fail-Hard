using Game;
using UnityEngine;

public class GadgetSuperSprings : Gadget
{
	private const float BOOST_COOLING_PERIOD = 0.5f;

	public AudioSource AudioSource;

	public float BoostForce;

	private int m_boostCooling;

	private Rigidbody connectedBody;

	private Transform gadgetPosition;

	private bool m_jumpedOff;

	private bool m_broken;

	private void Start()
	{
		base.State = GadgetState.GadgetOff;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (m_boostCooling > 0)
		{
			m_boostCooling--;
		}
		else
		{
			base.State = GadgetState.GadgetOff;
		}
	}

	public override void Equip(VehicleBase vehicle, Rigidbody rb, VehiclePart vp)
	{
		connectedBody = rb;
		base.VehiclePart = vp;
		BoostForce = vp.AccelerationDelta;
		VehicleSpinMultiplier = vp.SpinDelta;
	}

	public override void Action()
	{
		if (!m_broken && base.State != GadgetState.GadgetOn)
		{
			Fire();
		}
	}

	public override void PlayerMoving()
	{
	}

	public override void PlayerJumpOff()
	{
		m_jumpedOff = true;
	}

	private void Fire()
	{
		if (!m_jumpedOff && connectedBody.GetComponent<VehicleCar>().SmoothGrounded())
		{
			AudioManager.Instance.ReMix(AudioSource, 1f, AudioTag.Other);
			AudioManager.Instance.Play(AudioSource, AudioSource.clip, 0.1f, AudioTag.Other);
			m_boostCooling = (int)(0.5f / Time.fixedDeltaTime);
			base.State = GadgetState.GadgetOn;
			connectedBody.AddForce(BoostForce * base.transform.forward, ForceMode.VelocityChange);
		}
	}
}
