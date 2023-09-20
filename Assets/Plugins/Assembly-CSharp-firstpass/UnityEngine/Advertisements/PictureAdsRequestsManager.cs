using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	internal class PictureAdsRequestsManager
	{
		private static PictureAdsRequestsManager _inst;

		private Stack<PictureAdsRequest> _requestsForJSON;

		private Stack<PictureAdsRequest> _requestsForResources;

		private PictureAdsRequestsManager()
		{
			_requestsForJSON = new Stack<PictureAdsRequest>();
			_requestsForResources = new Stack<PictureAdsRequest>();
		}

		public static PictureAdsRequestsManager sharedInstance()
		{
			if (_inst == null)
			{
				_inst = new PictureAdsRequestsManager();
			}
			return _inst;
		}

		public void downloadJson(string network, PictureAdsManager manager)
		{
			PictureAdsRequest pictureAdsRequest = new PictureAdsRequest(network);
			pictureAdsRequest.setJsonAvailableDelegate(manager.jsonAvailableDelegate);
			pictureAdsRequest.setOperationCompleteDelegate(jsonOperationComplete);
			_requestsForJSON.Push(pictureAdsRequest);
			if (_requestsForJSON.Count == 1)
			{
				RequestsForJSONLoop();
			}
		}

		public void downloadResourcesForAd(string network, PictureAdsManager manager, PictureAd ad)
		{
			PictureAdsRequest pictureAdsRequest = new PictureAdsRequest(network);
			pictureAdsRequest.setResourcesAvailableDelegate(manager.resourcesAvailableDelegate);
			pictureAdsRequest.setOperationCompleteDelegate(resourcesOperationComplete);
			pictureAdsRequest.ad = ad;
			_requestsForResources.Push(pictureAdsRequest);
			if (_requestsForResources.Count == 1)
			{
				RequestsForResourcesLoop();
			}
		}

		private void jsonOperationComplete()
		{
			if (_requestsForJSON.Count != 0)
			{
				RequestsForJSONLoop();
			}
		}

		private void resourcesOperationComplete()
		{
			if (_requestsForResources.Count != 0)
			{
				RequestsForResourcesLoop();
			}
		}

		private void RequestsForJSONLoop()
		{
			if (_requestsForJSON.Count != 0)
			{
				PictureAdsRequest pictureAdsRequest = _requestsForJSON.Pop();
				if (pictureAdsRequest != null)
				{
					pictureAdsRequest.downloadJson();
				}
			}
		}

		private void RequestsForResourcesLoop()
		{
			if (_requestsForResources.Count != 0)
			{
				PictureAdsRequest pictureAdsRequest = _requestsForResources.Pop();
				if (pictureAdsRequest != null)
				{
					pictureAdsRequest.downloadAssetsForPictureAd(pictureAdsRequest.ad);
				}
			}
		}
	}
}
