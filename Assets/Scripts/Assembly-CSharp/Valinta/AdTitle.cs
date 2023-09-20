using System.Xml.Serialization;

namespace Valinta
{
	public class AdTitle
	{
		[XmlText]
		public string Text { get; set; }
	}
}
