using System.Xml.Serialization;

namespace Valinta
{
	public class Linear
	{
		[XmlAttribute("skipoffset")]
		public string SkipOffset { get; set; }

		[XmlElement("Duration")]
		[XmlText]
		public string Duration { get; set; }

		[XmlElement("TrackingEvents")]
		public TrackingEvents TrackingEvents { get; set; }

		[XmlElement("VideoClicks")]
		public VideoClicks VideoClicks { get; set; }

		[XmlElement("MediaFiles")]
		public MediaFiles MediaFiles { get; set; }

		[XmlElement("AdParameters")]
		public AdParameter[] AdParameters { get; set; }
	}
}
