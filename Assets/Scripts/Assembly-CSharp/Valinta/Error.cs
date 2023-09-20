using System.Xml.Serialization;

namespace Valinta
{
	public class Error
	{
		[XmlText]
		public string Text { get; set; }
	}
}
