using System.Xml.Serialization;

namespace Valinta
{
	public class NonLinearAd
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("width")]
		public string Width { get; set; }

		[XmlAttribute("height")]
		public string Height { get; set; }

		[XmlAttribute("expandedWidth")]
		public string ExpandedWidth { get; set; }

		[XmlAttribute("expandedHeight")]
		public string ExpandedHeight { get; set; }

		[XmlAttribute("scalable")]
		public bool Scalable { get; set; }

		[XmlAttribute("maintainAspectRatio")]
		public bool MaintainAspectRatio { get; set; }

		[XmlAttribute("minSuggestedDuration")]
		public string MinSuggestedDuration { get; set; }

		[XmlAttribute("apiFramework")]
		public string ApiFramework { get; set; }

		[XmlElement("StaticResource")]
		public StaticResource StaticResourceURL { get; set; }

		[XmlElement("TrackingEvents")]
		public TrackingEvents TrackingEvents { get; set; }

		[XmlElement("NonLinearClickThrough")]
		public ClickThrough ClickThrough { get; set; }

		[XmlElement("AdParameters")]
		public AdParameter[] AdParameters { get; set; }
	}
}
