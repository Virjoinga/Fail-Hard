namespace Valinta
{
	public class XmlParser
	{
		private VAST DeserializedXML;

		public XmlParser(string xml)
		{
			DeserializedXML = VUtils.Deserialize<VAST>(xml);
		}

		public VAST GetVAST()
		{
			return DeserializedXML;
		}
	}
}
