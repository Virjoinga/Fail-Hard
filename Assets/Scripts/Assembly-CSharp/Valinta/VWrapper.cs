using System.Collections.Generic;

namespace Valinta
{
	public class VWrapper
	{
		public string AdSystem;

		public string VASTAdTagURI;

		public string ErrorURI;

		public Dictionary<int, string> Impressions = new Dictionary<int, string>();

		public List<VCreative> Creatives = new List<VCreative>();
	}
}
