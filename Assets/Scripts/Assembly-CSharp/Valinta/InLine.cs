using System.Xml.Serialization;

namespace Valinta
{
	public class InLine
	{
		[XmlElement("AdSystem")]
		public AdSystem AdSystem { get; set; }

		[XmlElement("AdTitle")]
		public AdTitle AdTitle { get; set; }

		[XmlElement("Error")]
		public Error Error { get; set; }

		[XmlElement("Impression")]
		public Impression[] Impressions { get; set; }

		[XmlElement("Creatives")]
		public Creatives Creatives { get; set; }

		[XmlElement("Extensions")]
		public Extensions Extensions { get; set; }
	}
}
