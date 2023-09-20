using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEngine.Advertisements
{
	internal class PictureAd
	{
		public const int expectedResourcesCount = 5;

		private Dictionary<ImageURLType, Dictionary<ImageOrientation, Dictionary<ImageType, string>>> imageURLs;

		private Dictionary<ImageType, int> imageSpaces;

		public string id;

		public string clickActionUrl;

		public int closeButtonDelay = -1;

		public bool hasMoreCampaigns;

		public PictureAd()
		{
			imageSpaces = new Dictionary<ImageType, int>();
			imageURLs = new Dictionary<ImageURLType, Dictionary<ImageOrientation, Dictionary<ImageType, string>>>();
			imageURLs[ImageURLType.Local] = new Dictionary<ImageOrientation, Dictionary<ImageType, string>>();
			imageURLs[ImageURLType.Remote] = new Dictionary<ImageOrientation, Dictionary<ImageType, string>>();
			imageURLs[ImageURLType.Local][ImageOrientation.Portrait] = new Dictionary<ImageType, string>();
			imageURLs[ImageURLType.Local][ImageOrientation.Landscape] = new Dictionary<ImageType, string>();
			imageURLs[ImageURLType.Remote][ImageOrientation.Portrait] = new Dictionary<ImageType, string>();
			imageURLs[ImageURLType.Remote][ImageOrientation.Landscape] = new Dictionary<ImageType, string>();
		}

		public void setImageURL(string url, ImageURLType imageURLType, ImageOrientation pictureOrientation, ImageType imageType)
		{
			if (url == null || imageURLs == null || !imageURLs.ContainsKey(imageURLType))
			{
				return;
			}
			Dictionary<ImageOrientation, Dictionary<ImageType, string>> dictionary = imageURLs[imageURLType];
			if (dictionary != null && dictionary.ContainsKey(pictureOrientation))
			{
				Dictionary<ImageType, string> dictionary2 = dictionary[pictureOrientation];
				if (dictionary2 != null)
				{
					dictionary2[imageType] = url;
					Console.WriteLine(dictionary2[imageType]);
				}
			}
		}

		public string getImageURL(ImageURLType imageURLType, ImageOrientation pictureOrientation, ImageType imageType)
		{
			if (imageURLs == null || !imageURLs.ContainsKey(imageURLType))
			{
				return null;
			}
			Dictionary<ImageOrientation, Dictionary<ImageType, string>> dictionary = imageURLs[imageURLType];
			if (dictionary == null || !dictionary.ContainsKey(pictureOrientation))
			{
				return null;
			}
			Dictionary<ImageType, string> dictionary2 = dictionary[pictureOrientation];
			if (dictionary2 == null || !dictionary2.ContainsKey(imageType))
			{
				return null;
			}
			return dictionary2[imageType];
		}

		public string getLocalImageURL(ImageOrientation pictureOrientation, ImageType imageType)
		{
			return getImageURL(ImageURLType.Local, pictureOrientation, imageType);
		}

		public string getRemoteImageURL(ImageOrientation pictureOrientation, ImageType imageType)
		{
			return getImageURL(ImageURLType.Remote, pictureOrientation, imageType);
		}

		public void setImageSpace(ImageType imageType, int space)
		{
			if (imageSpaces != null)
			{
				imageSpaces[imageType] = space;
			}
		}

		public int getImageSpace(ImageType imageType)
		{
			if (imageSpaces == null || !imageSpaces.ContainsKey(imageType))
			{
				return -1;
			}
			return imageSpaces[imageType];
		}

		public bool resourcesAreValid()
		{
			return File.Exists(getLocalImageURL(ImageOrientation.Landscape, ImageType.Base)) && File.Exists(getLocalImageURL(ImageOrientation.Landscape, ImageType.Frame)) && File.Exists(getLocalImageURL(ImageOrientation.Landscape, ImageType.Close)) && File.Exists(getLocalImageURL(ImageOrientation.Portrait, ImageType.Base)) && File.Exists(getLocalImageURL(ImageOrientation.Portrait, ImageType.Frame)) && File.Exists(getLocalImageURL(ImageOrientation.Portrait, ImageType.Close));
		}

		public bool adIsValid()
		{
			return isValidStringField(id) && isValidStringField(clickActionUrl) && closeButtonDelay >= 0 && urlsAreValid() && spacesAreValid();
		}

		private bool spacesAreValid()
		{
			return getImageSpace(ImageType.Base) > 0 && getImageSpace(ImageType.Frame) > 0 && getImageSpace(ImageType.Close) > 0;
		}

		private bool isValidStringField(string field)
		{
			return field != null && field.Length != 0;
		}

		private bool isValidLocalURL(string url)
		{
			return isValidStringField(url);
		}

		private bool isValidRemoteURL(string url)
		{
			Uri result;
			return Uri.TryCreate(url, UriKind.Absolute, out result);
		}

		private bool urlsAreValid()
		{
			bool flag = isValidRemoteURL(getRemoteImageURL(ImageOrientation.Landscape, ImageType.Base)) && isValidRemoteURL(getRemoteImageURL(ImageOrientation.Landscape, ImageType.Frame)) && isValidRemoteURL(getRemoteImageURL(ImageOrientation.Landscape, ImageType.Close)) && isValidRemoteURL(getRemoteImageURL(ImageOrientation.Portrait, ImageType.Base)) && isValidRemoteURL(getRemoteImageURL(ImageOrientation.Portrait, ImageType.Frame));
			bool flag2 = isValidLocalURL(getLocalImageURL(ImageOrientation.Landscape, ImageType.Base)) && isValidLocalURL(getLocalImageURL(ImageOrientation.Landscape, ImageType.Frame)) && isValidLocalURL(getLocalImageURL(ImageOrientation.Landscape, ImageType.Close)) && isValidLocalURL(getLocalImageURL(ImageOrientation.Portrait, ImageType.Base)) && isValidLocalURL(getLocalImageURL(ImageOrientation.Portrait, ImageType.Frame)) && isValidLocalURL(getLocalImageURL(ImageOrientation.Portrait, ImageType.Close));
			return flag && flag2;
		}
	}
}
