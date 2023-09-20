using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Advertisements.Event;
using UnityEngine.Advertisements.HTTPLayer;

namespace UnityEngine.Advertisements
{
	internal class PictureAdsRequest
	{
		public delegate void jsonAvailable(string jsonData);

		public delegate void resourcesAvailable();

		public delegate void operationCompleteDelegate();

		private jsonAvailable _jsonAvailable;

		private resourcesAvailable _resourcesAvailable;

		private operationCompleteDelegate _operationCompleteDelegate;

		private Dictionary<string, ImageType> imageTypes;

		private Dictionary<string, ImageOrientation> imageOrientations;

		private int downloadedResourcesCount;

		private int[] retryDelays = new int[4] { 15, 30, 90, 240 };

		private string network;

		public PictureAd ad;

		public PictureAdsRequest(string network)
		{
			this.network = network;
			imageTypes = new Dictionary<string, ImageType>();
			imageOrientations = new Dictionary<string, ImageOrientation>();
		}

		public void setJsonAvailableDelegate(jsonAvailable action)
		{
			_jsonAvailable = action;
		}

		public void setResourcesAvailableDelegate(resourcesAvailable action)
		{
			_resourcesAvailable = action;
		}

		public void setOperationCompleteDelegate(operationCompleteDelegate action)
		{
			_operationCompleteDelegate = action;
		}

		public void downloadAssetsForPictureAd(PictureAd ad)
		{
			downloadedResourcesCount = 0;
			executeRequestForResource(ad, ImageOrientation.Landscape, ImageType.Base);
			executeRequestForResource(ad, ImageOrientation.Landscape, ImageType.Frame);
			executeRequestForResource(ad, ImageOrientation.Landscape, ImageType.Close);
			executeRequestForResource(ad, ImageOrientation.Portrait, ImageType.Base);
			executeRequestForResource(ad, ImageOrientation.Portrait, ImageType.Frame);
		}

		public void downloadJson()
		{
			string newUrl = requestURL();
			HTTPRequest hTTPRequest = new HTTPRequest("POST", newUrl);
			hTTPRequest.addHeader("Content-Type", "application/json");
			string payload = jsonPayload();
			hTTPRequest.setPayload(payload);
			HTTPManager.sendRequest(hTTPRequest, HTTPJsonCallback, retryDelays, 1200);
		}

		private void HTTPJsonCallback(HTTPResponse response)
		{
			if (response.dataLength != 0)
			{
				string @string = Encoding.UTF8.GetString(response.data, 0, response.dataLength);
				EventManager.sendAdplanEvent(Engine.Instance.AppId);
				_jsonAvailable(@string);
				_operationCompleteDelegate();
			}
		}

		private string requestURL()
		{
			string appId = Engine.Instance.AppId;
			return Settings.pictureAdsEndpoint + "/v2/picture/" + appId + "/campaigns";
		}

		private void executeRequestForResource(PictureAd ad, ImageOrientation imageOrientation, ImageType imageType)
		{
			string localImageURL = ad.getLocalImageURL(imageOrientation, imageType);
			if (File.Exists(localImageURL))
			{
				downloadedResourcesCount++;
				if (downloadedResourcesCount == 5)
				{
					_resourcesAvailable();
					_operationCompleteDelegate();
				}
			}
			else
			{
				string remoteImageURL = ad.getRemoteImageURL(imageOrientation, imageType);
				HTTPRequest request = new HTTPRequest(remoteImageURL);
				imageTypes[remoteImageURL] = imageType;
				imageOrientations[remoteImageURL] = imageOrientation;
				HTTPManager.sendFileRequest(request, HTTPFileCallback, retryDelays, 1200);
			}
		}

		private void HTTPFileCallback(HTTPResponse pictureURLRequestResponse)
		{
			downloadedResourcesCount++;
			if (pictureURLRequestResponse.dataLength != 0)
			{
				File.WriteAllBytes(ad.getLocalImageURL(imageOrientations[pictureURLRequestResponse.url], imageTypes[pictureURLRequestResponse.url]), pictureURLRequestResponse.data);
			}
			if (downloadedResourcesCount == 5)
			{
				_resourcesAvailable();
				_operationCompleteDelegate();
			}
		}

		private string jsonPayload()
		{
			return DeviceInfo.adRequestJSONPayload(network);
		}
	}
}
