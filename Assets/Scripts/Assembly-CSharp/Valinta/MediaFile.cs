using System.Xml.Serialization;

namespace Valinta
{
	public class MediaFile
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("delivery")]
		public string DeliveryMethod { get; set; }

		[XmlAttribute("type")]
		public string MimeType { get; set; }

		[XmlAttribute("bitrate")]
		public string BitRate { get; set; }

		[XmlAttribute("width")]
		public string Width { get; set; }

		[XmlAttribute("height")]
		public int Height { get; set; }

		[XmlAttribute("scalable")]
		public bool Scalable { get; set; }

		[XmlAttribute("maintainAspectRatio")]
		public bool MaintainAspect { get; set; }

		[XmlAttribute("apiFramework")]
		public string ApiFramework { get; set; }

		[XmlText]
		public string MediaUrl { get; set; }
	}
}
