using System.Xml.Serialization;

namespace Valinta
{
	public class TrackingEvents
	{
		[XmlElement("Tracking")]
		public Tracking[] Tracking { get; set; }
	}
}
