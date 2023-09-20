using System.Xml.Serialization;

namespace Valinta
{
	public class ClickThrough
	{
		[XmlText]
		public string Text { get; set; }
	}
}
