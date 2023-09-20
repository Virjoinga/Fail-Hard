using Game;
using UnityEngine;

public class DoorSound : MonoBehaviour
{
	private int lastDirection;

	public static float MIN_ANGULAR_VELOCITY = 0.2f;

	public AudioSource doorSource;

	public AudioClip[] doorClips;

	private int doorClipIndex;

	private void Start()
	{
		doorClipIndex = Random.Range(0, doorClips.Length);
	}

	private void Update()
	{
		int num = 0;
		float y = base.gameObject.rigidbody.angularVelocity.y;
		if (y > 0f)
		{
			num = 1;
		}
		if (y < 0f)
		{
			num = -1;
		}
		if (Mathf.Abs(y) > MIN_ANGULAR_VELOCITY && num != lastDirection && num != 0)
		{
			AudioManager.Instance.Play(doorSource, doorClips[doorClipIndex], 0.5f, AudioTag.AudienceAudio);
			doorClipIndex++;
			if (doorClipIndex == doorClips.Length)
			{
				doorClipIndex = 0;
			}
		}
		lastDirection = num;
	}
}
