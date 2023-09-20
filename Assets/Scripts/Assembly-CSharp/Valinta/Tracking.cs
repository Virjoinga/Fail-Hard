using System.Xml.Serialization;

namespace Valinta
{
	public class Tracking
	{
		[XmlAttribute("event")]
		public string EventType { get; set; }

		[XmlAttribute("offset")]
		public string Offset { get; set; }

		[XmlText]
		public string EventURL { get; set; }
	}
}
