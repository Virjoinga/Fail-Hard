using System.Xml.Serialization;

namespace Valinta
{
	public class Extensions
	{
		[XmlElement("Extension")]
		public Extension[] Extension { get; set; }
	}
}
