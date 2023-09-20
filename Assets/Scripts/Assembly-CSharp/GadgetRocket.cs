using Game;
using UnityEngine;

public class GadgetRocket : Gadget
{
	public AudioSource rocketSource;

	public float boostForce;

	public float boostDuration;

	private Rigidbody connectedBody;

	private Transform gadgetPosition;

	public ParticleSystem boostEffect;

	public ParticleSystem flameEffect;

	private float drag;

	public float BoostLeft { get; private set; }

	private void Start()
	{
		boostEffect = GetComponentInChildren<ParticleSystem>();
		boostEffect.Stop();
		flameEffect.Stop();
		BoostLeft = 1f;
		base.State = GadgetState.GadgetOff;
	}

	private void OnDestroy()
	{
		if (m_player != null)
		{
			m_player.RemoveAnimationTypeFromState(Player.AnimationType.Freefall);
		}
	}

	private void FixedUpdate()
	{
		if (base.State == GadgetState.GadgetOn)
		{
			ApplyBoost();
			BoostLeft -= Time.fixedDeltaTime / base.VehiclePart.UpgradeDelay;
			if (BoostLeft <= 0f)
			{
				EndBoost();
			}
		}
	}

	public override void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		connectedBody = rb;
		base.VehiclePart = vp;
		m_player = player;
		boostForce = vp.AccelerationDelta;
		boostDuration = vp.UpgradeDelay;
		PlayerSpinMultiplier = vp.SpinDelta;
		player.BindAnimationTypeToState(Player.AnimationType.Freefall, "fly");
	}

	public override void Action()
	{
		if (base.State == GadgetState.GadgetOff && base.VehiclePart.CurrentCondition > 0)
		{
			m_player.JumpOffVehicle();
			Fire();
		}
		else if (base.VehiclePart.CurrentCondition > 0)
		{
			base.VehiclePart.CurrentCondition--;
			BoostLeft += 1f;
		}
	}

	public override void PlayerJumpWithVehicle()
	{
	}

	public override void PlayerJumpOff()
	{
	}

	private void Fire()
	{
		base.State = GadgetState.GadgetOn;
		drag = connectedBody.drag;
		connectedBody.drag = 1f;
		base.VehiclePart.CurrentCondition--;
		boostEffect.Play();
		flameEffect.Play();
		if (rocketSource != null)
		{
			AudioManager.Instance.ReMix(rocketSource, 1f, AudioTag.Other);
			AudioManager.Instance.Play(rocketSource, rocketSource.clip, 1f, AudioTag.Other);
		}
	}

	private void ApplyBoost()
	{
		connectedBody.AddForce((0f - base.VehiclePart.AccelerationDelta) * base.transform.right, ForceMode.Force);
		connectedBody.AddTorque(-10f * base.transform.up, ForceMode.Force);
	}

	private void EndBoost()
	{
		base.State = GadgetState.GadgetOff;
		BoostLeft = 1f;
		connectedBody.drag = drag;
		flameEffect.Stop();
		boostEffect.Stop();
		m_player.RemoveAnimationTypeFromState(Player.AnimationType.Freefall);
	}
}
