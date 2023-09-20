using System.Xml.Serialization;

namespace Valinta
{
	public class AdSystem
	{
		[XmlAttribute("version")]
		public string Version { get; set; }

		[XmlText]
		public string Text { get; set; }
	}
}
