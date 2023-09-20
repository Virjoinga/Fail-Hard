using UnityEngine;

namespace Game
{
	public class MixingInfo
	{
		private float volume;

		public AudioTag Tag { get; set; }

		public float Volume
		{
			get
			{
				if (Muted)
				{
					return 0f;
				}
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public float VolumeFactor { get; set; }

		private float OriginalVolume { get; set; }

		public int Priority { get; set; }

		public float DopplerLevel { get; set; }

		public AudioRolloffMode RolloffMode { get; set; }

		public AudioType Type { get; set; }

		public bool Muted { get; set; }

		public MixingInfo()
		{
			Volume = 1f;
			VolumeFactor = 1f;
			Priority = 128;
			DopplerLevel = 1f;
			RolloffMode = AudioRolloffMode.Linear;
			Type = AudioType.Sfx;
			Muted = false;
		}

		public void Mute()
		{
			Muted = true;
		}

		public void UnMute()
		{
			Muted = false;
		}

		public void Mix(AudioSource source)
		{
			if (source != null)
			{
				source.volume = Volume * VolumeFactor;
				source.priority = Priority;
				source.dopplerLevel = DopplerLevel;
				source.rolloffMode = RolloffMode;
			}
			else
			{
				Debug.LogWarning("Trying to mix without audiosource!");
			}
		}
	}
}
