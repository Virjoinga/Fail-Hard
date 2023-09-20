using System.Xml.Serialization;

namespace Valinta
{
	public class Impression
	{
		[XmlText]
		public string URL { get; set; }
	}
}
