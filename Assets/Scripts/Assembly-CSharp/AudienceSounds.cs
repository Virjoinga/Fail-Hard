using System.Collections;
using Game;
using UnityEngine;

public class AudienceSounds : MonoBehaviour
{
	public enum DriverState
	{
		Idle = 0,
		Driving = 1,
		OnAir = 2,
		Landing = 3
	}

	public enum AudienceState
	{
		NoSound = 0,
		Chattering = 1,
		Cheering = 2,
		Booing = 3,
		Reacting = 4
	}

	private const float CHEERING_SPEED_DELTA = -2f;

	public AudioSource[] chatterSource;

	public AudioSource[] accelerationCheerSource;

	public AudioSource[] booingSource;

	public AudioSource[] landingReactionSource;

	public AudioSource[] landingOutcomeSource;

	public AudioSource applauseSource;

	public AudioSource randomReactionSource;

	public AudioClip[] exc0Chatter;

	public AudioClip[] exc1Chatter;

	public AudioClip[] exc2Chatter;

	public AudioClip[] exc0AccelerationCheer;

	public AudioClip[] exc1AccelerationCheer;

	public AudioClip[] exc2AccelerationCheer;

	public AudioClip[] landingReaction;

	public AudioClip[] landingOutcome;

	public AudioClip[] applause;

	public AudioClip[] booing;

	public AudioClip[] randomReaction;

	private AudioSource music;

	private bool accelerationCheered;

	private ArrayList fadeList = new ArrayList();

	private AudienceState currentAudienceState;

	private DriverState currentDriverState;

	public static int MAX_EXCITEMENT = 100;

	public static float BOOING_RANGE = 8f;

	public static float CHEERING_RANGE = 4f;

	private int excitement;

	private float speedDelta;

	private float previousSpeed;

	public Transform target;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("MusicPlayer(Clone)");
		if ((bool)gameObject)
		{
			music = gameObject.GetComponent<AudioSource>();
		}
		reset();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Ramp");
		foreach (GameObject gameObject2 in array)
		{
			if (target == null)
			{
				target = gameObject2.transform;
			}
			else if (gameObject2.transform.position.x < target.transform.position.x)
			{
				target = gameObject2.transform;
			}
		}
	}

	public void reset()
	{
		accelerationCheered = false;
		currentAudienceState = AudienceState.Chattering;
		currentDriverState = DriverState.Idle;
		AudioSource[] array = booingSource;
		foreach (AudioSource audioSource in array)
		{
			audioSource.Stop();
		}
		if (fadeList.Count > 0)
		{
			emptyFadeList();
		}
		speedDelta = 0f;
		previousSpeed = 0f;
	}

	private void emptyFadeList()
	{
		foreach (SoundFade fade in fadeList)
		{
			fade.resetFade();
		}
		fadeList.Clear();
	}

	private int getExcitementLevel()
	{
		int num = excitement / 33;
		if (num > 2)
		{
			num = 2;
		}
		return num;
	}

	public void addExcitement(int amount)
	{
		excitement += amount;
		if (excitement > MAX_EXCITEMENT)
		{
			excitement = MAX_EXCITEMENT;
		}
	}

	public void lowerExcitement(int amount)
	{
		excitement -= amount;
		if (excitement < 0)
		{
			excitement = 0;
		}
	}

	private AudioClip getAudioClip(AudioClip[] clips)
	{
		return clips[Random.Range(0, clips.Length)];
	}

	private AudioClip getChatterClip()
	{
		AudioClip[] array = exc0Chatter;
		int excitementLevel = getExcitementLevel();
		if (excitementLevel == 1)
		{
			array = exc1Chatter;
		}
		if (excitementLevel == 2)
		{
			array = exc2Chatter;
		}
		return array[Random.Range(0, array.Length)];
	}

	private AudioClip getAccelerationClip()
	{
		AudioClip[] array = exc0AccelerationCheer;
		int excitementLevel = getExcitementLevel();
		if (excitementLevel == 1)
		{
			array = exc1AccelerationCheer;
		}
		if (excitementLevel == 2)
		{
			array = exc2AccelerationCheer;
		}
		return array[Random.Range(0, array.Length)];
	}

	private void addFade(AudioSource source, float fadeFactor, float minVolume = 0f)
	{
		SoundFade soundFade = null;
		foreach (SoundFade fade in fadeList)
		{
			if (fade.getSource() == source)
			{
				soundFade = fade;
			}
		}
		if (soundFade != null)
		{
			soundFade.setFadeFactor(fadeFactor);
		}
		else
		{
			fadeList.Add(new SoundFade(source, fadeFactor, minVolume));
		}
	}

	private void doFades()
	{
		ArrayList arrayList = new ArrayList();
		foreach (SoundFade fade in fadeList)
		{
			if (fade.doFade())
			{
				arrayList.Add(fade);
			}
		}
		foreach (SoundFade item in arrayList)
		{
			fadeList.Remove(item);
		}
	}

	private void checkState()
	{
		if (GameController.Instance.Character.Vehicle == null)
		{
			return;
		}
		AudienceState audienceState = currentAudienceState;
		DriverState driverState = currentDriverState;
		bool flag = false;
		PlayerState playerState = null;
		if ((bool)GameController.Instance.Character.Player)
		{
			playerState = GameController.Instance.Character.Player.currentState;
		}
		if (playerState == null)
		{
			return;
		}
		if (playerState.GetType() == typeof(PlayerStateInHCRVehicle))
		{
			currentDriverState = DriverState.Driving;
		}
		else if (playerState.GetType() == typeof(PlayerStateMidAir))
		{
			currentDriverState = DriverState.OnAir;
			if (((PlayerStateMidAir)playerState).getCurrentState().GetType() == typeof(MidAirStateCrashed) && driverState != 0)
			{
				currentDriverState = DriverState.Landing;
			}
		}
		float num = 0f;
		bool flag2 = false;
		if ((bool)target)
		{
			Vector3 vector = GameController.Instance.Character.Vehicle.transform.position - (target.position + target.forward);
			num = vector.magnitude;
			flag2 = false;
			if (vector.x < 0f)
			{
				flag2 = true;
			}
		}
		else
		{
			num = CHEERING_RANGE;
		}
		float num2 = 0f;
		if (num <= BOOING_RANGE)
		{
			num2 = GameController.Instance.Character.Vehicle.CurrentSpeed;
			if (previousSpeed != 0f)
			{
				speedDelta += num2 - previousSpeed;
			}
			if (speedDelta > 2f)
			{
				speedDelta = 0f;
			}
			previousSpeed = num2;
		}
		if (currentDriverState == DriverState.Driving)
		{
			audienceState = ((currentAudienceState == AudienceState.Booing && flag2) ? AudienceState.Booing : AudienceState.NoSound);
			if (flag2)
			{
				if (speedDelta >= -2f)
				{
					if (num <= CHEERING_RANGE)
					{
						audienceState = AudienceState.Cheering;
					}
				}
				else if (num <= BOOING_RANGE)
				{
					audienceState = AudienceState.Booing;
				}
			}
		}
		if (currentDriverState == DriverState.Landing && currentAudienceState != AudienceState.Chattering)
		{
			audienceState = AudienceState.Reacting;
		}
		if (audienceState != currentAudienceState)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (currentAudienceState == AudienceState.Chattering)
		{
			if ((bool)music)
			{
				addFade(music, -0.3f, 0.1f);
			}
			AudioSource[] array = chatterSource;
			foreach (AudioSource source in array)
			{
				addFade(source, -0.3f, 0.1f);
			}
			addFade(applauseSource, -0.3f, 0f);
		}
		if (currentAudienceState == AudienceState.Booing)
		{
			AudioSource[] array2 = booingSource;
			foreach (AudioSource source2 in array2)
			{
				addFade(source2, -0.2f, 0f);
			}
		}
		if (audienceState == AudienceState.Cheering)
		{
			playCheering();
		}
		if (audienceState == AudienceState.Booing)
		{
			playBooing();
		}
		if (audienceState == AudienceState.Reacting)
		{
			if ((bool)music)
			{
				addFade(music, 0.4f, 0f);
			}
			playLandingReaction();
		}
		currentAudienceState = audienceState;
	}

	private void playCheering()
	{
		if (!accelerationCheered)
		{
			accelerationCheered = true;
			AudioSource[] array = accelerationCheerSource;
			foreach (AudioSource source in array)
			{
				AudioManager.Instance.Play(source, getAccelerationClip(), 0.5f, AudioTag.AudienceAudio);
			}
		}
	}

	private void playBooing()
	{
		AudioSource[] array = booingSource;
		foreach (AudioSource source in array)
		{
			addFade(source, 0.1f, 0f);
			AudioManager.Instance.Play(source, getAudioClip(booing), 0.5f, AudioTag.AudienceAudio);
		}
	}

	private void Update()
	{
		checkState();
		if (currentAudienceState == AudienceState.Chattering)
		{
			playChatter();
		}
		doFades();
	}

	private void playChatter()
	{
		if (!chatterSource[0].isPlaying)
		{
			AudioManager.Instance.Play(chatterSource[0], getChatterClip(), 0.5f, AudioTag.AudienceAudio);
		}
		if (!applauseSource.isPlaying && getExcitementLevel() == 2 && Random.Range(0, 100) == 1)
		{
			AudioManager.Instance.Play(applauseSource, getAudioClip(applause), 0.5f, AudioTag.AudienceAudio);
		}
	}

	private void playLandingOutcome()
	{
		AudioSource[] array = landingOutcomeSource;
		foreach (AudioSource source in array)
		{
			AudioManager.Instance.Play(source, getAudioClip(landingOutcome), 0.5f, AudioTag.AudienceAudio);
		}
		AudioSource[] array2 = chatterSource;
		foreach (AudioSource source2 in array2)
		{
			addFade(source2, 0.4f, 0f);
		}
		currentAudienceState = AudienceState.Chattering;
	}

	private void playRandomReaction()
	{
		if (accelerationCheered)
		{
			AudioManager.Instance.Play(randomReactionSource, getAudioClip(randomReaction), 0.5f, AudioTag.AudienceAudio);
		}
	}

	private void playLandingReaction()
	{
		float time = Random.Range(0.6f, 0.9f);
		if (Random.Range(0, 8) != 1)
		{
			float num = Random.Range(0.6f, 1f);
			float volume = 1.6f - num;
			landingReactionSource[0].volume = num;
			landingReactionSource[1].volume = volume;
			AudioSource[] array = landingReactionSource;
			foreach (AudioSource source in array)
			{
				AudioManager.Instance.Play(source, getAudioClip(landingReaction), 0.5f, AudioTag.AudienceAudio);
			}
		}
		Invoke("playLandingOutcome", time);
		if (Random.Range(0, 5) == 1)
		{
			Invoke("playRandomReaction", Random.Range(2f, 3f));
		}
	}
}
