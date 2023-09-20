using System.Xml.Serialization;

namespace Valinta
{
	public class Creative
	{
		[XmlElement("TrackingEvents")]
		public TrackingEvents[] TrackingEvents;

		[XmlAttribute("sequence")]
		public string Sequence { get; set; }

		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlElement("VideoClicks")]
		public VideoClicks VideoClicks { get; set; }

		[XmlElement("Linear")]
		public Linear[] LinearAds { get; set; }

		[XmlElement("NonLinearAds")]
		public NonLinearAd[] NonLinearAds { get; set; }

		[XmlElement("CompanionAds")]
		public CompanionAd[] CompanionAds { get; set; }
	}
}
