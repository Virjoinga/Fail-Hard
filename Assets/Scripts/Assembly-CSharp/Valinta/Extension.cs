using System.Xml.Serialization;

namespace Valinta
{
	public class Extension
	{
		[XmlAttribute("type")]
		public string Type { get; set; }

		[XmlText]
		public string Value { get; set; }
	}
}
