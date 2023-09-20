using Game;
using UnityEngine;

public class GadgetVehicleRocket : Gadget
{
	public AudioSource rocketSource;

	public float boostForce;

	public float boostDuration;

	private Rigidbody connectedBody;

	private Transform gadgetPosition;

	public GameObject OkRockets;

	public GameObject BrokenRockets;

	public Rigidbody Rocket1;

	public Rigidbody Rocket2;

	private float drag;

	private bool m_jumpedOff;

	private bool m_broken;

	public float BoostLeft { get; private set; }

	private void Start()
	{
		EffectsState(false);
		BoostLeft = 1f;
		base.State = GadgetState.GadgetOff;
	}

	private void FixedUpdate()
	{
		if (base.State == GadgetState.GadgetOn)
		{
			ApplyBoost();
			BoostLeft -= Time.fixedDeltaTime / boostDuration;
			if (BoostLeft <= 0f)
			{
				EndBoost();
			}
		}
	}

	public override void Equip(VehicleBase vehicle, Rigidbody rb, VehiclePart vp)
	{
		connectedBody = rb;
		base.VehiclePart = vp;
		boostForce = vp.AccelerationDelta;
		boostDuration = vp.UpgradeDelay;
		VehicleSpinMultiplier = vp.SpinDelta;
	}

	public override void Action()
	{
		if (!m_broken && base.State != GadgetState.GadgetOn && base.VehiclePart.CurrentCondition > 0)
		{
			Fire();
		}
		else if (!m_broken && base.VehiclePart.CurrentCondition > 0)
		{
			base.VehiclePart.CurrentCondition--;
			BoostLeft += 1f;
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
		base.State = GadgetState.GadgetOn;
		base.VehiclePart.CurrentCondition--;
		drag = connectedBody.drag;
		connectedBody.drag = 1f;
		EffectsState(true);
		if (rocketSource != null)
		{
			AudioManager.Instance.ReMix(rocketSource, 1f, AudioTag.Other);
			AudioManager.Instance.Play(rocketSource, rocketSource.clip, 1f, AudioTag.Other);
		}
	}

	private void ApplyBoost()
	{
		if (m_broken)
		{
			Rocket1.AddRelativeForce((0f - boostForce) * Vector3.up + Random.Range(-0.1f, 0.1f) * Vector3.forward, ForceMode.Force);
			Rocket1.AddRelativeTorque(Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1f, 1f) * Vector3.forward);
			Rocket2.AddRelativeForce((0f - boostForce) * Vector3.up + Random.Range(-0.1f, 0.1f) * Vector3.forward, ForceMode.Force);
			Rocket2.AddRelativeTorque(Random.Range(-1f, 1f) * Vector3.up + Random.Range(-1f, 1f) * Vector3.forward);
		}
		else
		{
			connectedBody.AddForce((0f - boostForce) * base.transform.up, ForceMode.Force);
		}
	}

	private void EndBoost()
	{
		base.State = GadgetState.GadgetOff;
		BoostLeft = 1f;
		connectedBody.drag = drag;
		EffectsState(false);
	}

	private void EffectsState(bool isOn)
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			if (isOn)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	private void Break()
	{
		if (!m_broken && m_jumpedOff)
		{
			OkRockets.SetActive(false);
			BrokenRockets.SetActive(true);
			Rocket1.velocity = connectedBody.velocity;
			Rocket2.velocity = connectedBody.velocity;
			m_broken = true;
			connectedBody.drag = drag;
			if (base.State == GadgetState.GadgetOn)
			{
				EffectsState(true);
			}
			else
			{
				EffectsState(false);
			}
		}
	}
}
