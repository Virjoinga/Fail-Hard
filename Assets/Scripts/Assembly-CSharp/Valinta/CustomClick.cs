using System.Xml.Serialization;

namespace Valinta
{
	public class CustomClick
	{
		[XmlText]
		public string Text { get; set; }
	}
}
