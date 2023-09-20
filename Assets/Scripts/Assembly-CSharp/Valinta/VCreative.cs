using System.Collections.Generic;

namespace Valinta
{
	public class VCreative
	{
		public Dictionary<string, List<string>> TrackingEvents;

		public Dictionary<string, string> VideoClicks;

		public Dictionary<string, string> MediaFiles;

		public string Type { get; set; }

		public string ID { get; set; }

		public string Sequence { get; set; }

		public string Duration { get; set; }

		public string SkipOffset { get; set; }

		public bool Skippable { get; set; }

		public VCreative()
		{
			TrackingEvents = new Dictionary<string, List<string>>();
			VideoClicks = new Dictionary<string, string>();
			MediaFiles = new Dictionary<string, string>();
		}
	}
}
