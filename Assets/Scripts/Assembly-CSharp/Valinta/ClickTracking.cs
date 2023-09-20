using System.Xml.Serialization;

namespace Valinta
{
	public class ClickTracking
	{
		[XmlText]
		public string Text { get; set; }
	}
}
