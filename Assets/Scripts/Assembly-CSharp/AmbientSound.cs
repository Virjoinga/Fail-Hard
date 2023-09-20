using Game;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
	public AudioSource ambientSource;

	private void Start()
	{
		if ((bool)ambientSource)
		{
			AudioManager.Instance.Play(ambientSource, ambientSource.clip, 0.5f, AudioTag.AmbientAudio);
		}
	}
}
