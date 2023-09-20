using System.Xml.Serialization;

namespace Valinta
{
	public class CompanionAd
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("width")]
		public int Width { get; set; }

		[XmlAttribute("height")]
		public int Height { get; set; }

		[XmlAttribute("expandedWidth")]
		public int ExpandedWidth { get; set; }

		[XmlAttribute("expandedHeight")]
		public int ExpandedHeight { get; set; }

		[XmlAttribute("apiFramework")]
		public string ApiFramework { get; set; }

		[XmlElement("StaticResource")]
		public StaticResource StaticResourceURL { get; set; }

		[XmlElement("TrackingEvents")]
		public TrackingEvents TrackingEvents { get; set; }

		[XmlElement("CompanionClickThrough")]
		public ClickThrough ClickThrough { get; set; }

		[XmlElement("AltText")]
		public AltText[] AltTexts { get; set; }

		[XmlElement("AdParameters")]
		public AdParameter[] AdParameters { get; set; }
	}
}
