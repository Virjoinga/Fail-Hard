using System.Xml.Serialization;

namespace Valinta
{
	public class VideoClicks
	{
		[XmlElement("ClickThrough")]
		public ClickThrough ClickThrough { get; set; }

		[XmlElement("ClickTracking")]
		public ClickTracking ClickTracking { get; set; }

		[XmlElement("CustomClick")]
		public CustomClick CustomClick { get; set; }
	}
}
