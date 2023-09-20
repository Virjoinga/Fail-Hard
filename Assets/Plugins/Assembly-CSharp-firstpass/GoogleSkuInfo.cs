using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GoogleSkuInfo
{
	public string title { get; private set; }

	public string price { get; private set; }

	public string currencyCode { get; private set; }

	public string type { get; private set; }

	public string description { get; private set; }

	public string productId { get; private set; }

	public int hundredParts { get; private set; }

	public GoogleSkuInfo(Dictionary<string, object> dict)
	{
		if (dict.ContainsKey("title"))
		{
			title = dict["title"] as string;
		}
		if (dict.ContainsKey("price"))
		{
			string input = dict["price"] as string;
			Regex regex = new Regex("([\\d,.]+)");
			Match match = regex.Match(input);
			if (match.Success)
			{
				price = match.Groups[1].Value;
			}
			else
			{
				price = input;
			}
		}
		if (dict.ContainsKey("type"))
		{
			type = dict["type"] as string;
		}
		if (dict.ContainsKey("description"))
		{
			description = dict["description"] as string;
		}
		if (dict.ContainsKey("productId"))
		{
			productId = dict["productId"] as string;
		}
		if (dict.ContainsKey("price_currency_code"))
		{
			currencyCode = dict["price_currency_code"] as string;
		}
		if (dict.ContainsKey("price_amount_micros"))
		{
			long num = (long)dict["price_amount_micros"];
			hundredParts = (int)(num / 10000);
		}
	}

	public static List<GoogleSkuInfo> fromList(List<object> items)
	{
		List<GoogleSkuInfo> list = new List<GoogleSkuInfo>();
		foreach (Dictionary<string, object> item in items)
		{
			list.Add(new GoogleSkuInfo(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<GoogleSkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}", title, price, type, description, productId);
	}
}
