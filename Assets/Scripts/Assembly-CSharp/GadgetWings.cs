using Game;
using UnityEngine;

public class GadgetWings : Gadget
{
	public float Lift;

	public float MaxLiftVel;

	public float StallSpeedKmh;

	public float SmoothingDuration = 0.6f;

	private float m_smoothing;

	private Rigidbody connectedBody;

	public GameObject OkWings;

	public GameObject BrokenWings;

	private bool m_broken;

	private float m_flapBoost;

	public float FlapDuration = 0.5f;

	public float MaxFlapForce = 100f;

	private void Start()
	{
		base.State = GadgetState.GadgetOff;
		GetComponentInChildren<Animation>()["Default Take"].normalizedTime = 1f;
		GetComponentInChildren<Animation>().Play();
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
		if (base.State == GadgetState.GadgetOn && !m_broken)
		{
			ApplyLift();
		}
	}

	public override void Equip(Player player, Rigidbody rb, VehiclePart vp)
	{
		connectedBody = rb;
		base.VehiclePart = vp;
		m_player = player;
		Lift = vp.AccelerationDelta;
		FlapDuration = vp.UpgradeDelay;
		PlayerSpinMultiplier = vp.SpinDelta;
		MaxLiftVel = vp.TopSpeedDelta;
		player.BindAnimationTypeToState(Player.AnimationType.Freefall, "fly");
	}

	public override void PlayerJumpOff()
	{
	}

	public override void Action()
	{
		if (!m_broken)
		{
			if (base.State == GadgetState.GadgetOn && m_flapBoost <= 0f)
			{
				m_flapBoost = 1f;
			}
			m_player.JumpOffVehicle();
			base.State = GadgetState.GadgetOn;
			GetComponent<Collider>().enabled = true;
			GetComponentInChildren<Animation>()["Default Take"].speed = -1f;
			GetComponentInChildren<Animation>()["Default Take"].normalizedTime = 1f;
			GetComponentInChildren<Animation>().Play("Default Take");
			connectedBody.drag = 0.2f;
			connectedBody.angularDrag = 0.1f;
		}
	}

	private void ApplyLift()
	{
		float f = Vector3.Dot(3.6f * connectedBody.velocity, -base.transform.right);
		float num = Mathf.Clamp(Mathf.Abs(f) / StallSpeedKmh, 0.1f, 1f);
		float num2 = Mathf.Clamp(Vector3.Dot(3.6f * connectedBody.velocity, base.transform.forward), 0f - MaxLiftVel, MaxLiftVel);
		Vector3 force = (0f - Lift) * num2 * num * base.transform.forward;
		if (m_smoothing < 1f)
		{
			m_smoothing += Time.fixedDeltaTime / SmoothingDuration;
			float num3 = Vector3.Angle(base.transform.right, Vector3.right);
			if (base.transform.right.y > 0f)
			{
				connectedBody.AddTorque(-2f * num3 * base.transform.up);
			}
			else
			{
				connectedBody.AddTorque(2f * num3 * base.transform.up);
			}
		}
		force *= m_smoothing * m_smoothing;
		force.z = 0f;
		connectedBody.AddForce(force, ForceMode.Force);
		if (m_flapBoost > 0f)
		{
			m_flapBoost -= Time.fixedDeltaTime / FlapDuration;
			float num4 = -4f * MaxFlapForce * (m_flapBoost * m_flapBoost - m_flapBoost);
			Vector3 force2 = (0f - num4) * (base.transform.forward + 0.1f * base.transform.right);
			connectedBody.AddForce(force2, ForceMode.Force);
			connectedBody.AddTorque(10f * (m_flapBoost * m_flapBoost - m_flapBoost) * base.transform.up);
		}
		connectedBody.AddTorque(60f * (1f - num) * base.transform.up);
	}

	private void Flap()
	{
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (hit.gameObject.tag != "Player" && hit.gameObject.tag != "Vehicle" && !hit.isTrigger && !m_broken && base.State == GadgetState.GadgetOn)
		{
			OkWings.SetActive(false);
			BrokenWings.SetActive(true);
			m_broken = true;
		}
	}
}
