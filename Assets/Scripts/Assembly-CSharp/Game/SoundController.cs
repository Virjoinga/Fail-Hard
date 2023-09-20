using UnityEngine;

namespace Game
{
	public class SoundController : MonoBehaviour
	{
		public AudioSource idleSource;

		public AudioSource runningSource1;

		public AudioSource runningSource2;

		public AudioSource enginePopSource;

		public AudioSource suspensionSource;

		public AudioSource startSource;

		public AudioClip[] enginePops;

		public AudioClip[] suspensionClips;

		public VehicleBase engine;

		public ParticleSystem DarkSmokeEffect;

		public static float IDLE_FACTOR = 0.1f;

		public static float RUNNING2_FACTOR = 0.5f;

		public static float ON_AIR_ACCELERATION_TIME = 0.7f;

		public static float MAX_RPM_FACTOR = 1f;

		public static float DEFAULT_RPM_MODIFIER;

		public static float RPM_MODIFIER_CAP = 0.3f;

		public Transform suspensionDistancePoint;

		private float oldSuspensionDistance;

		public float enginePopInterval;

		public int enginePopProb;

		private bool onAir;

		private float popTimer = 1f;

		private int suspensionInterval;

		private float onAirTimer;

		private float rpmModifier = DEFAULT_RPM_MODIFIER;

		public static float SUSPENSION_THRESHOLD = 0.004f;

		public static int SUSPENSION_INTERVAL = 5;

		private bool engineStarted;

		private AudioManager m_audioManager;

		private bool stuntActive;

		public bool StuntActive
		{
			get
			{
				return stuntActive;
			}
			set
			{
				stuntActive = value;
				if (!stuntActive)
				{
					stopEngine();
				}
			}
		}

		private void Start()
		{
			m_audioManager = AudioManager.Instance;
			m_audioManager.ReMix(runningSource1, 0f, AudioTag.VehicleAudio);
			m_audioManager.ReMix(runningSource2, 0f, AudioTag.VehicleAudio);
			m_audioManager.ReMix(idleSource, 0f, AudioTag.VehicleAudio);
			suspensionInterval = SUSPENSION_INTERVAL;
			oldSuspensionDistance = getSuspensionDistance();
		}

		private float getSuspensionDistance()
		{
			return (engine.transform.position - suspensionDistancePoint.position).magnitude;
		}

		private bool doEnginePop()
		{
			bool result = false;
			if (enginePops.Length > 0)
			{
				popTimer += Time.deltaTime;
				if (popTimer > enginePopInterval && Random.Range(0, enginePopProb) == 0)
				{
					result = true;
					m_audioManager.ReMix(enginePopSource, Random.Range(0.6f, 1f), AudioTag.VehicleAudio);
					enginePopSource.pitch = Random.Range(0.8f, 1.2f);
					AudioClip clip = enginePops[Random.Range(0, enginePops.Length)];
					AudioManager.Instance.Play(enginePopSource, clip, 0.1f, AudioTag.VehicleAudio);
					popTimer = 0f;
				}
			}
			return result;
		}

		public void stopEngine()
		{
			runningSource1.Stop();
			runningSource2.Stop();
			idleSource.Stop();
			startSource.Stop();
		}

		public void startEngine()
		{
			engineStarted = true;
			AudioManager.Instance.Play(startSource, startSource.clip, 0.1f, AudioTag.VehicleAudio);
			runningSource1.Play();
			runningSource2.Play();
			idleSource.Play();
		}

		private void Update()
		{
			if (!StuntActive)
			{
				return;
			}
			if (!engineStarted)
			{
				PlayerState playerState = null;
				if ((bool)GameController.Instance.Character.Player)
				{
					playerState = GameController.Instance.Character.Player.currentState;
				}
				if (playerState == null || playerState.GetType() != typeof(PlayerStateInHCRVehicle))
				{
					return;
				}
				startEngine();
			}
			bool front;
			bool rear;
			engine.IsGrounded(out front, out rear);
			suspensionInterval--;
			if (suspensionInterval == 0 && (bool)suspensionSource)
			{
				float suspensionDistance = getSuspensionDistance();
				if (oldSuspensionDistance - suspensionDistance > SUSPENSION_THRESHOLD)
				{
					AudioClip clip = suspensionClips[Random.Range(0, suspensionClips.Length)];
					AudioManager.Instance.Play(suspensionSource, clip, 0.1f, AudioTag.VehicleAudio);
				}
				oldSuspensionDistance = suspensionDistance;
				suspensionInterval = SUSPENSION_INTERVAL;
			}
			float num = engine.EngineRpm / 800f;
			if (num > MAX_RPM_FACTOR)
			{
				num = MAX_RPM_FACTOR;
			}
			if (doEnginePop())
			{
				DarkSmokeEffect.Play();
			}
			if (!rear && !onAir)
			{
				onAir = true;
			}
			else if (rear && onAir)
			{
				onAir = false;
				onAirTimer = 0f;
				rpmModifier = DEFAULT_RPM_MODIFIER;
			}
			if (onAir)
			{
				onAirTimer += Time.deltaTime;
				if (onAirTimer < ON_AIR_ACCELERATION_TIME)
				{
					rpmModifier += Time.deltaTime * 0.5f;
				}
				if (onAirTimer > ON_AIR_ACCELERATION_TIME)
				{
					rpmModifier -= Time.deltaTime * 1.2f;
				}
				if (rpmModifier > RPM_MODIFIER_CAP)
				{
					rpmModifier = RPM_MODIFIER_CAP;
				}
				num += rpmModifier;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			float num2 = 1f;
			if (popTimer < 1f)
			{
				num2 = popTimer * 2f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			runningSource1.pitch = 0.8f + 0.9f * num;
			m_audioManager.ReMix(runningSource1, 3.5f * num, AudioTag.VehicleAudio);
			if (num > RUNNING2_FACTOR)
			{
				m_audioManager.ReMix(runningSource1, 1.6f - 2f * num, AudioTag.VehicleAudio);
			}
			runningSource2.pitch = 0.8f + 0.9f * num;
			m_audioManager.ReMix(runningSource2, 3f * num - 0.7f, AudioTag.VehicleAudio);
			m_audioManager.ReMix(idleSource, 1f - 3f * num, AudioTag.VehicleAudio);
			if (runningSource1.pitch > 1.2f && !onAir)
			{
				runningSource1.pitch = 1.2f;
			}
			if (runningSource2.pitch > 1.2f && !onAir)
			{
				runningSource2.pitch = 1.2f;
			}
			if (runningSource1.pitch > 1.3f && onAir)
			{
				runningSource1.pitch = 1.3f;
			}
			if (runningSource2.pitch > 1.3f && onAir)
			{
				runningSource2.pitch = 1.3f;
			}
			m_audioManager.ReMix(runningSource1, runningSource1.volume * 0.3f * num2, AudioTag.VehicleAudio);
			m_audioManager.ReMix(runningSource2, runningSource2.volume * 0.3f * num2, AudioTag.VehicleAudio);
			m_audioManager.ReMix(idleSource, idleSource.volume * 0.3f * num2, AudioTag.VehicleAudio);
		}
	}
}
