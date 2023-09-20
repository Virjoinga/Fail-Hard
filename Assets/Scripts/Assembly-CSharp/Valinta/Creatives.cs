using System.Xml.Serialization;

namespace Valinta
{
	public class Creatives
	{
		[XmlElement("Creative")]
		public Creative[] Creative { get; set; }
	}
}
