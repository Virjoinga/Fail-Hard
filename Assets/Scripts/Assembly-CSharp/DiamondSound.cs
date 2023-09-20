using Game;
using UnityEngine;

public class DiamondSound : MonoBehaviour
{
	public AudioClip diamondClip;

	public void playSound()
	{
		AudioManager.Instance.PlayClipAt(diamondClip, base.transform.position, AudioTag.DiamondAudio);
	}
}
