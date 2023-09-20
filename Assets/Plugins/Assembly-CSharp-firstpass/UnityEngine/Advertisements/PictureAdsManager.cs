using System.IO;
using UnityEngine.Advertisements.Event;

namespace UnityEngine.Advertisements
{
	internal class PictureAdsManager
	{
		public delegate void PictureAdWillBeShown();

		public delegate void PictureAdReady();

		public delegate void PictureAdFailed();

		public delegate void PictureAdDidOpen();

		public delegate void PictureAdWillBeClosed();

		public delegate void PictureAdDidClosed();

		public delegate void PictureAdClicked();

		private PictureAdsFrameManager framesManager;

		private PictureAdsRequestsManager requestManager;

		private PictureAd currentAd;

		private bool jsonDownloaded;

		private bool resourcesAreDownloaded;

		private string _network;

		private PictureAdDidClosed _pictureAdDidClosedDelegate;

		private PictureAdWillBeShown _pictureAdWillBeShownDelegate;

		private PictureAdReady _pictureAdReadyDelegate;

		private PictureAdFailed _pictureAdFailedDelegate;

		private PictureAdDidOpen _pictureAdDidOpenDelegate;

		private PictureAdWillBeClosed _pictureAdWillBeClosed;

		private PictureAdClicked _pictureAdClicked;

		public string network
		{
			get
			{
				return _network;
			}
		}

		public PictureAdsManager(string network)
		{
			requestManager = PictureAdsRequestsManager.sharedInstance();
			_network = network;
		}

		public void setPictureAdClicked(PictureAdClicked action)
		{
			_pictureAdClicked = action;
		}

		public void setPictureAdWillBeClosed(PictureAdWillBeClosed action)
		{
			_pictureAdWillBeClosed = action;
		}

		public void setPictureAdDidClosedDelegate(PictureAdDidClosed action)
		{
			_pictureAdDidClosedDelegate = action;
		}

		public void setPictureAdWillBeShownDelegate(PictureAdWillBeShown action)
		{
			_pictureAdWillBeShownDelegate = action;
		}

		public void setPictureAdReadyDelegate(PictureAdReady action)
		{
			_pictureAdReadyDelegate = action;
		}

		public void setPictureAdFailedDelegate(PictureAdFailed action)
		{
			_pictureAdFailedDelegate = action;
		}

		public void setPictureAdDidOpenDelegate(PictureAdDidOpen action)
		{
			_pictureAdDidOpenDelegate = action;
		}

		public void init()
		{
			EventManager.sendAdreqEvent(Engine.Instance.AppId);
			currentAd = null;
			jsonDownloaded = false;
			resourcesAreDownloaded = false;
			if (requestManager != null)
			{
				requestManager.downloadJson(_network, this);
			}
		}

		public void pictureAdWillBeClosed()
		{
			_pictureAdWillBeClosed();
		}

		public void pictureAdClicked()
		{
			_pictureAdClicked();
		}

		public void pictureAdDidClosed()
		{
			framesManager = null;
			GameObject obj = GameObject.Find("UnityAdsFramesManagerHolder");
			Object.Destroy(obj);
			_pictureAdDidClosedDelegate();
		}

		public void pictureAdFailed()
		{
			framesManager = null;
			GameObject obj = GameObject.Find("UnityAdsFramesManagerHolder");
			Object.Destroy(obj);
			_pictureAdFailedDelegate();
		}

		private void removeLocalResources(PictureAd ad)
		{
			if (ad.adIsValid())
			{
				File.Delete(ad.getLocalImageURL(ImageOrientation.Landscape, ImageType.Close));
				File.Delete(ad.getLocalImageURL(ImageOrientation.Landscape, ImageType.Frame));
				File.Delete(ad.getLocalImageURL(ImageOrientation.Landscape, ImageType.Base));
				File.Delete(ad.getLocalImageURL(ImageOrientation.Portrait, ImageType.Close));
				File.Delete(ad.getLocalImageURL(ImageOrientation.Portrait, ImageType.Base));
				File.Delete(ad.getLocalImageURL(ImageOrientation.Portrait, ImageType.Frame));
			}
		}

		public void resourcesAvailableDelegate()
		{
			resourcesAreDownloaded = true;
			_pictureAdReadyDelegate();
		}

		public void jsonAvailableDelegate(string jsonData)
		{
			jsonDownloaded = true;
			currentAd = PictureAdsParser.parseJSONString(jsonData, Application.temporaryCachePath + "/");
			if (currentAd == null || !currentAd.adIsValid())
			{
				pictureAdFailed();
			}
			else
			{
				requestManager.downloadResourcesForAd(_network, this, currentAd);
			}
		}

		private bool areResourcesReady()
		{
			return jsonDownloaded && resourcesAreDownloaded;
		}

		public bool isAdAvailable()
		{
			return areResourcesReady() && currentAd.adIsValid() && currentAd.resourcesAreValid() && (!(framesManager != null) || framesManager.adIsClosed()) && !isShowingAd();
		}

		public bool isShowingAd()
		{
			return framesManager != null && framesManager.isShowingAd();
		}

		public void showAd()
		{
			GameObject gameObject = GameObject.Find("UnityAdsFramesManagerHolder");
			if (gameObject == null)
			{
				gameObject = new GameObject("UnityAdsFramesManagerHolder");
				framesManager = gameObject.AddComponent<PictureAdsFrameManager>();
				framesManager.manager = this;
			}
			if (isAdAvailable() && framesManager.adIsClosed())
			{
				framesManager.initAd(currentAd);
				EventManager.sendViewEvent(Engine.Instance.AppId, currentAd.id);
				_pictureAdWillBeShownDelegate();
				framesManager.showAd();
				_pictureAdDidOpenDelegate();
			}
		}
	}
}
