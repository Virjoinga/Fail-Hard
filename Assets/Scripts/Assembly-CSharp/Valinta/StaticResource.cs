using System.Xml.Serialization;

namespace Valinta
{
	public class StaticResource
	{
		[XmlText]
		public string Text { get; set; }
	}
}
