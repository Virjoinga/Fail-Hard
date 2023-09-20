using System.Collections.Generic;
using Game;
using UnityEngine;

public class SurfaceEffect : MonoBehaviour
{
	public enum EffectData
	{
		NORMAL = 0,
		NO_SPEED_MODIFY = 1,
		PLAY_ALWAYS = 2
	}

	public WheelGroundContact wheel;

	public ParticleSystem sandEffect;

	public ParticleSystem grassEffect;

	public ParticleSystem grassEffect2;

	public ParticleSystem waterEffect;

	private float lastFrameSpeed;

	private List<ParticleSystem> particles = new List<ParticleSystem>();

	private VehicleBase vehicle;

	private Dictionary<ParticleSystem, float> startingSpeeds = new Dictionary<ParticleSystem, float>();

	private Dictionary<ParticleSystem, EffectData> effectData = new Dictionary<ParticleSystem, EffectData>();

	private Vector3 groundLevelPoint;

	public Transform trailPoint;

	private static float START_TRAIL_INTERVAL = 0.1f;

	private static float MIN_DELTA_SPEED = 10f;

	private float trailInterval = START_TRAIL_INTERVAL;

	private GameObject trail;

	private SurfaceMaterialType currentMaterial;

	private void Awake()
	{
		startingSpeeds.Add(sandEffect, sandEffect.startSpeed);
		startingSpeeds.Add(grassEffect, grassEffect.startSpeed);
		startingSpeeds.Add(grassEffect2, grassEffect2.startSpeed);
		startingSpeeds.Add(waterEffect, waterEffect.startSpeed);
		effectData.Add(waterEffect, EffectData.PLAY_ALWAYS);
		effectData.Add(grassEffect, EffectData.NO_SPEED_MODIFY);
	}

	private SurfaceMaterialType getSurfaceMaterial()
	{
		Ray ray = new Ray(wheel.transform.position, Vector3.down);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 0.2f))
		{
			groundLevelPoint = hitInfo.point;
			AJMaterial component = hitInfo.collider.gameObject.GetComponent<AJMaterial>();
			if (component != null)
			{
				return component.surfaceMaterialType;
			}
		}
		return SurfaceMaterialType.None;
	}

	private void updateTrail()
	{
		if (wheel.IsContact)
		{
			trailInterval -= Time.deltaTime;
			if (trailInterval <= 0f)
			{
				trailInterval = START_TRAIL_INTERVAL;
				Trail component = trail.GetComponent<Trail>();
				component.addVertex(groundLevelPoint + Vector3.up * 0.01f);
			}
		}
	}

	private float getDeltaSpeed()
	{
		float num = vehicle.TargetSpeed;
		if (num > vehicle.vehicleData.GetTopSpeed())
		{
			num = vehicle.vehicleData.GetTopSpeed();
		}
		return num - vehicle.CurrentSpeed;
	}

	private void checkTrail(float deltaSpeed)
	{
		if (deltaSpeed < MIN_DELTA_SPEED || !wheel.IsContact)
		{
			trail = null;
			return;
		}
		if (!trail)
		{
			trail = (GameObject)Object.Instantiate(Resources.Load("SurfaceTrail"), trailPoint.position, Quaternion.identity);
		}
		trail.GetComponent<Trail>().setMaterial(currentMaterial);
	}

	private void playParticles(ParticleSystem particle, float deltaSpeed)
	{
		bool flag = true;
		EffectData effectData = (this.effectData.ContainsKey(particle) ? this.effectData[particle] : EffectData.NORMAL);
		float num = startingSpeeds[particle];
		if (deltaSpeed < MIN_DELTA_SPEED && effectData != EffectData.PLAY_ALWAYS)
		{
			flag = false;
		}
		if (!wheel.IsContact)
		{
			flag = false;
		}
		if (!flag)
		{
			particle.Stop();
			return;
		}
		if (effectData != EffectData.NO_SPEED_MODIFY)
		{
			float num2 = deltaSpeed / 15f;
			particle.startSpeed = num2 * num;
			if (particle.startSpeed > 2.5f)
			{
				particle.startSpeed = 2.5f;
			}
		}
		if (effectData == EffectData.PLAY_ALWAYS)
		{
			float currentSpeed = GameController.Instance.Character.Vehicle.CurrentSpeed;
			float num3 = currentSpeed / 20f;
			particle.startSpeed = num3 * num;
			if (particle.startSpeed > 2f)
			{
				particle.startSpeed = 2f;
			}
		}
		particle.Play();
	}

	private void Update()
	{
		if (!vehicle)
		{
			vehicle = GameController.Instance.Character.Vehicle;
		}
		if (!vehicle)
		{
			return;
		}
		bool flag = false;
		currentMaterial = getSurfaceMaterial();
		switch (currentMaterial)
		{
		case SurfaceMaterialType.ShallowWater:
			if (!particles.Contains(waterEffect))
			{
				addParticleEffect(waterEffect);
			}
			break;
		case SurfaceMaterialType.Sand:
			if (!particles.Contains(sandEffect))
			{
				addParticleEffect(sandEffect);
			}
			break;
		case SurfaceMaterialType.Grass:
			flag = true;
			if (!particles.Contains(grassEffect))
			{
				addParticleEffect(grassEffect, grassEffect2);
			}
			break;
		case SurfaceMaterialType.Asphalt:
			flag = true;
			stopParticles();
			break;
		default:
			trail = null;
			stopParticles();
			break;
		}
		float deltaSpeed = 0f;
		if (vehicle.Throttle > 0.2f)
		{
			deltaSpeed = 20f;
		}
		foreach (ParticleSystem particle in particles)
		{
			playParticles(particle, deltaSpeed);
		}
		if (flag)
		{
			checkTrail(deltaSpeed);
			if ((bool)trail)
			{
				updateTrail();
			}
		}
	}

	private void addParticleEffect(ParticleSystem effect, ParticleSystem effect2 = null)
	{
		stopParticles();
		particles.Add(effect);
		if ((bool)effect2)
		{
			particles.Add(effect2);
		}
	}

	private void stopParticles()
	{
		if (particles.Count > 0)
		{
			foreach (ParticleSystem particle in particles)
			{
				particle.Stop();
			}
		}
		particles.Clear();
	}
}
