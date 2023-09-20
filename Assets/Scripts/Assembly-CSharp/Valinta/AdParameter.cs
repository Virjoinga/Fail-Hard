using System.Xml.Serialization;

namespace Valinta
{
	public class AdParameter
	{
		[XmlText]
		public string Text { get; set; }
	}
}
