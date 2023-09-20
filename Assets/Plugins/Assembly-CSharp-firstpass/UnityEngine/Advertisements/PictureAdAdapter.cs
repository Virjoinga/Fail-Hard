using System;
using System.Collections.Generic;

namespace UnityEngine.Advertisements
{
	internal class PictureAdAdapter : Adapter
	{
		private PictureAdsManager _manager;

		public PictureAdAdapter(string adapterId)
			: base(adapterId)
		{
		}

		public override void Initialize(string zoneId, string adapterId, Dictionary<string, object> configuration)
		{
			string text = null;
			string text2 = null;
			triggerEvent(EventType.initStart, EventArgs.Empty);
			if (configuration != null && configuration.ContainsKey("network"))
			{
				text = (string)configuration["network"];
			}
			text2 = DeviceInfo.currentPlatform();
			if (text == null || text.Length == 0)
			{
				switch (text2)
				{
				default:
				{
					int num;
					text = ((num != 1) ? "picture_editor" : "picture_android");
					break;
				}
				case "ios":
					text = "picture_ios";
					break;
				}
			}
			_manager = new PictureAdsManager(text);
			_manager.setPictureAdDidClosedDelegate(onPictureAdDidClosed);
			_manager.setPictureAdWillBeClosed(onPictureAdWillBeClosed);
			_manager.setPictureAdFailedDelegate(onPictureAdFailed);
			_manager.setPictureAdReadyDelegate(onPictureAdReady);
			_manager.setPictureAdWillBeShownDelegate(onPictureAdWillBeShown);
			_manager.setPictureAdDidOpenDelegate(onPictureAdDidOpen);
			_manager.setPictureAdClicked(onPictureAdClicked);
			_manager.init();
		}

		private void onPictureAdClicked()
		{
			triggerEvent(EventType.adClicked, EventArgs.Empty);
		}

		private void onPictureAdDidOpen()
		{
			triggerEvent(EventType.adDidOpen, EventArgs.Empty);
		}

		private void onPictureAdWillBeShown()
		{
			triggerEvent(EventType.adWillOpen, EventArgs.Empty);
		}

		private void onPictureAdReady()
		{
			triggerEvent(EventType.initComplete, EventArgs.Empty);
			triggerEvent(EventType.adAvailable, EventArgs.Empty);
		}

		private void onPictureAdFailed()
		{
			triggerEvent(EventType.initFailed, EventArgs.Empty);
			triggerEvent(EventType.error, EventArgs.Empty);
		}

		private void onPictureAdWillBeClosed()
		{
			triggerEvent(EventType.adWillClose, EventArgs.Empty);
		}

		private void onPictureAdDidClosed()
		{
			triggerEvent(EventType.adFinished, EventArgs.Empty);
			triggerEvent(EventType.adDidClose, EventArgs.Empty);
		}

		public override void RefreshAdPlan()
		{
			Utils.LogDebug("Got refresh ad plan request for picture ads");
		}

		public override void StartPrecaching()
		{
		}

		public override void StopPrecaching()
		{
		}

		public override bool isReady(string zoneId, string adapterId)
		{
			return _manager.isAdAvailable();
		}

		public override void Show(string zoneId, string adapterId, ShowOptions options = null)
		{
			_manager.showAd();
		}

		public override bool isShowing()
		{
			return _manager.isShowingAd();
		}
	}
}
