using System.Xml.Serialization;

namespace Valinta
{
	public class MediaFiles
	{
		[XmlElement("MediaFile")]
		public MediaFile[] MediaFile { get; set; }
	}
}
