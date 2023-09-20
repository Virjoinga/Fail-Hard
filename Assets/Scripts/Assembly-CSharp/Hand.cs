using System.Collections.Generic;
using Game;
using UnityEngine;

public class Hand : BreakingStuff
{
	public Transform arm1;

	public Transform arm2;

	public List<Vector2> targetAngles;

	public int currentTarget;

	public float speed;

	public AudioSource handSource;

	private void Start()
	{
		arm1.localEulerAngles = new Vector3(0f, targetAngles[0].x, 180f);
		arm2.localEulerAngles = new Vector3(0f, targetAngles[0].y, 0f);
	}

	private void FixedUpdate()
	{
		arm1.localRotation = Quaternion.Slerp(Quaternion.Euler(arm1.localEulerAngles), Quaternion.Euler(new Vector3(0f, targetAngles[currentTarget].x, 180f)), 0.1f);
		Vector3 vector = new Vector3(0f, targetAngles[currentTarget].y, 0f);
		float num = vector.y - arm2.localEulerAngles.y;
		if (num < -180f)
		{
			num += 360f;
		}
		if (num > 5f)
		{
			arm2.rigidbody.MoveRotation(arm2.rotation * Quaternion.Euler(speed * Vector3.up * Time.fixedDeltaTime));
		}
		else if (num < -5f)
		{
			arm2.rigidbody.MoveRotation(arm2.rotation * Quaternion.Euler((0f - speed) * Vector3.up * Time.fixedDeltaTime));
		}
	}

	public void Swing()
	{
		if ((bool)handSource && !handSource.isPlaying)
		{
			AudioManager.Instance.Play(handSource, handSource.clip, 0.5f, AudioTag.BigCrashAudio);
		}
		currentTarget = 1;
		Invoke("Return", 1f);
	}

	public void Return()
	{
		currentTarget = 0;
	}

	public override void Break()
	{
		Swing();
	}
}
