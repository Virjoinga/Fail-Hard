using System.Xml.Serialization;

namespace Valinta
{
	public class Ad
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlElement("InLine")]
		public InLine[] InLines { get; set; }

		[XmlElement("Wrapper")]
		public Wrapper[] Wrappers { get; set; }
	}
}
