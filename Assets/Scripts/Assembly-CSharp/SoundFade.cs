using UnityEngine;

public class SoundFade
{
	private float fadeFactor;

	private float originalVolume;

	private float minVolume;

	private AudioSource source;

	public SoundFade(AudioSource source, float fadeFactor, float minVolume)
	{
		this.fadeFactor = fadeFactor;
		this.source = source;
		originalVolume = source.volume;
		if (fadeFactor > 0f)
		{
			source.volume = minVolume;
		}
		this.minVolume = minVolume;
	}

	public void resetFade()
	{
		source.volume = originalVolume;
	}

	public bool doFade()
	{
		source.volume += fadeFactor * Time.deltaTime;
		if (source.volume < minVolume)
		{
			source.volume = minVolume;
		}
		if (source.volume >= originalVolume)
		{
			source.volume = originalVolume;
			return true;
		}
		if (source.volume <= minVolume && minVolume == 0f)
		{
			source.Stop();
			source.volume = originalVolume;
			return true;
		}
		return false;
	}

	public void setFadeFactor(float fadeFactor)
	{
		this.fadeFactor = fadeFactor;
	}

	public AudioSource getSource()
	{
		return source;
	}
}
