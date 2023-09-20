using Game;
using UnityEngine;

public class GadgetStigaPropel : Gadget
{
	public AudioSource StartSource;

	public AudioSource AudioSource;

	public float BoostForce;

	private Rigidbody connectedBody;

	private Transform gadgetPosition;

	public Transform Rotor;

	private float drag;

	private bool m_jumpedOff;

	private bool m_broken;

	private void Start()
	{
		EffectsState(false);
		base.State = GadgetState.GadgetOff;
	}

	private void Update()
	{
		if (base.State == GadgetState.GadgetOn && !m_jumpedOff)
		{
			Rotor.Rotate(0f, 1000f * Time.smoothDeltaTime, 0f);
		}
	}

	private void FixedUpdate()
	{
		if (base.State == GadgetState.GadgetOn && !m_jumpedOff)
		{
			ApplyBoost();
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
		else
		{
			EndBoost();
		}
	}

	public override void PlayerMoving()
	{
	}

	public override void PlayerJumpOff()
	{
		m_jumpedOff = true;
		EndBoost();
	}

	private void Fire()
	{
		if (!m_jumpedOff)
		{
			base.State = GadgetState.GadgetOn;
			drag = connectedBody.drag;
			EffectsState(true);
		}
	}

	private void ApplyBoost()
	{
		connectedBody.AddForce((0f - BoostForce) * base.transform.up, ForceMode.Force);
	}

	private void EndBoost()
	{
		base.State = GadgetState.GadgetOff;
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
		if (AudioSource != null && StartSource != null)
		{
			if (isOn)
			{
				AudioManager.Instance.ReMix(StartSource, 1f, AudioTag.Other);
				AudioManager.Instance.Play(StartSource, StartSource.clip, 0.1f, AudioTag.Other);
				AudioManager.Instance.Play(AudioSource, AudioSource.clip, 0.1f, AudioTag.Other);
			}
			else
			{
				AudioSource.Stop();
			}
		}
	}

	private void Break()
	{
	}
}
