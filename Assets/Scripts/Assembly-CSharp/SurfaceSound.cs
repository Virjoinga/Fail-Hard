using Game;
using UnityEngine;

public class SurfaceSound : MonoBehaviour
{
	public AudioSource surfaceSource;

	public float volumeFactor;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		if (!(GameController.Instance.Character.Vehicle == null) && collisionInfo.gameObject.tag == "Vehicle")
		{
			float currentSpeed = GameController.Instance.Character.Vehicle.CurrentSpeed;
			if (!surfaceSource.isPlaying && currentSpeed > 5f)
			{
				AudioManager.Instance.Play(surfaceSource, surfaceSource.clip, 0.5f, AudioTag.SurfaceAudio);
			}
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		surfaceSource.Stop();
	}
}
