using System.Xml.Serialization;

namespace Valinta
{
	[XmlRoot("VAST")]
	public class VAST
	{
		[XmlAttribute("version")]
		public string Version { get; set; }

		[XmlElement("Ad")]
		public Ad[] Ads { get; set; }
	}
}
