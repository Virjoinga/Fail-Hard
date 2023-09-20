using System.Collections.Generic;
using UnityEngine.Advertisements.MiniJSON;

namespace UnityEngine.Advertisements
{
	internal class PictureAdsParser
	{
		private const int defaultFrameSpace = 90;

		private const int defaultCloseButtonSpace = 5;

		private const int defaultBaseSpace = 80;

		private const int defaultCloseButtonDelay = 0;

		private static string game__KEY = "game";

		private static string landscapeFramePicture__KEY = "landscapeFramePicture";

		private static string portraitFramePicture__KEY = "portraitFramePicture";

		private static string closeButtonPicture__KEY = "closeButtonPicture";

		private static string closeButtonDelay__KEY = "closeButtonDelay";

		private static string campaign__KEY = "campaign";

		private static string landscapePicture__KEY = "landscapePicture";

		private static string portraitPicture__KEY = "portraitPicture";

		private static string id__KEY = "id";

		private static string clickActionUrl__KEY = "clickUrl";

		private static string hasMoreCampaigns__KEY = "hasMoreCampaigns";

		private static string frameSpace__KEY = "frameSpace";

		private static string closeSpace__KEY = "closeSpace";

		private static string baseSpace__KEY = "baseSpace";

		public static PictureAd parseJSONString(string jsonString, string localCachePath)
		{
			PictureAd pictureAd = new PictureAd();
			pictureAd.setImageSpace(ImageType.Base, 80);
			pictureAd.setImageSpace(ImageType.Close, 5);
			pictureAd.setImageSpace(ImageType.Frame, 90);
			pictureAd.closeButtonDelay = 0;
			if (jsonString == null || jsonString.Length == 0)
			{
				return pictureAd;
			}
			Dictionary<string, object> dictionary = Json.Deserialize(jsonString) as Dictionary<string, object>;
			if (dictionary == null)
			{
				return pictureAd;
			}
			if (!dictionary.ContainsKey("data"))
			{
				return pictureAd;
			}
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["data"];
			foreach (string key in dictionary2.Keys)
			{
				if (key == game__KEY)
				{
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)dictionary2[key];
					if (dictionary3 == null)
					{
						return pictureAd;
					}
					if (dictionary3.ContainsKey(landscapeFramePicture__KEY))
					{
						setupPathsForAd(pictureAd, localCachePath, (string)dictionary3[landscapeFramePicture__KEY], ImageOrientation.Landscape, ImageType.Frame);
					}
					if (dictionary3.ContainsKey(portraitFramePicture__KEY))
					{
						setupPathsForAd(pictureAd, localCachePath, (string)dictionary3[portraitFramePicture__KEY], ImageOrientation.Portrait, ImageType.Frame);
					}
					if (dictionary3.ContainsKey(closeButtonPicture__KEY))
					{
						setupPathsForAd(pictureAd, localCachePath, (string)dictionary3[closeButtonPicture__KEY], ImageOrientation.Landscape, ImageType.Close);
						pictureAd.setImageURL(localPathForResource(localCachePath, (string)dictionary3[closeButtonPicture__KEY]), ImageURLType.Local, ImageOrientation.Portrait, ImageType.Close);
					}
					if (dictionary3.ContainsKey(closeButtonDelay__KEY))
					{
						pictureAd.closeButtonDelay = stringToInt(dictionary3[closeButtonDelay__KEY].ToString());
					}
					setImageSpace(pictureAd, ImageType.Frame, dictionary3);
					setImageSpace(pictureAd, ImageType.Close, dictionary3);
				}
				if (key == campaign__KEY)
				{
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)dictionary2[key];
					if (dictionary4 == null)
					{
						return pictureAd;
					}
					if (dictionary4.ContainsKey(landscapePicture__KEY))
					{
						setupPathsForAd(pictureAd, localCachePath, (string)dictionary4[landscapePicture__KEY], ImageOrientation.Landscape, ImageType.Base);
					}
					if (dictionary4.ContainsKey(portraitPicture__KEY))
					{
						setupPathsForAd(pictureAd, localCachePath, (string)dictionary4[portraitPicture__KEY], ImageOrientation.Portrait, ImageType.Base);
					}
					if (dictionary4.ContainsKey(id__KEY))
					{
						pictureAd.id = (string)dictionary4[id__KEY];
					}
					if (dictionary4.ContainsKey(clickActionUrl__KEY))
					{
						pictureAd.clickActionUrl = (string)dictionary4[clickActionUrl__KEY];
					}
					setImageSpace(pictureAd, ImageType.Base, dictionary4);
				}
				if (key == hasMoreCampaigns__KEY)
				{
					bool.TryParse(dictionary2[key].ToString(), out pictureAd.hasMoreCampaigns);
				}
			}
			return pictureAd;
		}

		private static int stringToInt(string v)
		{
			int result = 0;
			int.TryParse(v, out result);
			return result;
		}

		private static void setImageSpace(PictureAd ad, ImageType imageType, Dictionary<string, object> dict)
		{
			string key = null;
			switch (imageType)
			{
			case ImageType.Base:
				key = baseSpace__KEY;
				break;
			case ImageType.Frame:
				key = frameSpace__KEY;
				break;
			case ImageType.Close:
				key = closeSpace__KEY;
				break;
			}
			if (dict != null && dict.ContainsKey(key))
			{
				ad.setImageSpace(imageType, stringToInt(dict[key].ToString()));
			}
		}

		private static string localPathForResource(string localCachePath, string remotePath)
		{
			string[] array = remotePath.Split('/');
			string text = array[array.Length - 2] + array[array.Length - 1];
			return localCachePath + text;
		}

		private static void setupPathsForAd(PictureAd ad, string localCachePath, string remotePath, ImageOrientation orientation, ImageType imageType)
		{
			ad.setImageURL(remotePath, ImageURLType.Remote, orientation, imageType);
			ad.setImageURL(localPathForResource(localCachePath, remotePath), ImageURLType.Local, orientation, imageType);
		}
	}
}
