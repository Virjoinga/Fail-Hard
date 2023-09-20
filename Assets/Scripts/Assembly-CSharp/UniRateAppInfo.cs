using System;
using System.Collections.Generic;
using UniRateMiniJSON;

public class UniRateAppInfo
{
	private const string kAppInfoResultsKey = "results";

	private const string kAppInfoBundleIdKey = "bundleId";

	private const string kAppInfoGenreIdKey = "primaryGenreId";

	private const string kAppInfoAppIdKey = "trackId";

	private const string kAppInfoVersion = "version";

	public bool validAppInfo;

	public string bundleId;

	public int appStoreGenreID;

	public int appID;

	public string version;

	public UniRateAppInfo(string jsonResponse)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(jsonResponse) as Dictionary<string, object>;
		if (dictionary == null)
		{
			return;
		}
		List<object> list = dictionary["results"] as List<object>;
		if (list != null && list.Count > 0)
		{
			Dictionary<string, object> dictionary2 = list[0] as Dictionary<string, object>;
			if (dictionary2 != null)
			{
				bundleId = dictionary2["bundleId"] as string;
				appStoreGenreID = Convert.ToInt32(dictionary2["primaryGenreId"]);
				appID = Convert.ToInt32(dictionary2["trackId"]);
				version = dictionary2["version"] as string;
				validAppInfo = true;
			}
		}
	}
}
