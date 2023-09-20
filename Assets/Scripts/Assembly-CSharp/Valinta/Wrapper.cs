using System.Xml.Serialization;

namespace Valinta
{
	public class Wrapper
	{
		[XmlElement("AdSystem")]
		public AdSystem AdSystem { get; set; }

		[XmlElement("VASTAdTagURI")]
		public VASTAdTagURI AdTagURI { get; set; }

		[XmlElement("Error")]
		public Error Error { get; set; }

		[XmlElement("Impression")]
		public Impression[] Impressions { get; set; }

		[XmlElement("Creatives")]
		public Creatives Creatives { get; set; }

		[XmlElement("Extensions")]
		public Extension[] Extensions { get; set; }
	}
}
