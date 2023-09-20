using System.Xml.Serialization;

namespace Valinta
{
	public class AltText
	{
		[XmlText]
		public string Text { get; set; }
	}
}
