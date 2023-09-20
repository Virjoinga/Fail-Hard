using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Optional;

namespace UnityEngine.Advertisements
{
	internal class VideoAdAdapter : Adapter
	{
		private bool _isShowing;

		private static Dictionary<string, Dictionary<string, object>> _configurations = new Dictionary<string, Dictionary<string, object>>();

		public VideoAdAdapter(string adapterId)
			: base(adapterId)
		{
		}

		public override void Initialize(string zoneId, string adapterId, Dictionary<string, object> configuration)
		{
			UnityAds.OnCampaignsAvailable = (UnityAds.UnityAdsCampaignsAvailable)Delegate.Combine(UnityAds.OnCampaignsAvailable, new UnityAds.UnityAdsCampaignsAvailable(UnityAdsCampaignsAvailable));
			UnityAds.OnCampaignsFetchFailed = (UnityAds.UnityAdsCampaignsFetchFailed)Delegate.Combine(UnityAds.OnCampaignsFetchFailed, new UnityAds.UnityAdsCampaignsFetchFailed(UnityAdsCampaignsFetchFailed));
			triggerEvent(EventType.initStart, EventArgs.Empty);
			UnityAds.SharedInstance.Init(Engine.Instance.AppId, Engine.Instance.testMode);
			_configurations.Add(zoneId + adapterId, configuration);
		}

		public override void RefreshAdPlan()
		{
		}

		public override void StartPrecaching()
		{
		}

		public override void StopPrecaching()
		{
		}

		public override bool isReady(string zoneId, string adapterId)
		{
			Dictionary<string, object> dictionary = _configurations[zoneId + adapterId];
			if (dictionary != null && dictionary.ContainsKey("network"))
			{
				return UnityAds.canShowAds((string)dictionary["network"]);
			}
			return false;
		}

		public override void Show(string zoneId, string adapterId, ShowOptions options = null)
		{
			if (options != null && !options.pause)
			{
				Utils.LogWarning("Video ads will always pause engine, ignoring pause=false in ShowOptions");
			}
			Dictionary<string, object> dictionary = _configurations[zoneId + adapterId];
			if (dictionary != null && dictionary.ContainsKey("network"))
			{
				UnityAds.setNetwork((string)dictionary["network"]);
			}
			string zoneId2 = null;
			string rewardItemKey = string.Empty;
			if (dictionary != null && dictionary.ContainsKey("zone"))
			{
				zoneId2 = (string)dictionary["zone"];
			}
			if (dictionary != null && dictionary.ContainsKey("rewardItem"))
			{
				rewardItemKey = (string)dictionary["rewardItem"];
			}
			UnityAds.OnShow = (UnityAds.UnityAdsShow)Delegate.Combine(UnityAds.OnShow, new UnityAds.UnityAdsShow(UnityAdsShow));
			UnityAds.OnHide = (UnityAds.UnityAdsHide)Delegate.Combine(UnityAds.OnHide, new UnityAds.UnityAdsHide(UnityAdsHide));
			UnityAds.OnVideoCompleted = (UnityAds.UnityAdsVideoCompleted)Delegate.Combine(UnityAds.OnVideoCompleted, new UnityAds.UnityAdsVideoCompleted(UnityAdsVideoCompleted));
			UnityAds.OnVideoStarted = (UnityAds.UnityAdsVideoStarted)Delegate.Combine(UnityAds.OnVideoStarted, new UnityAds.UnityAdsVideoStarted(UnityAdsVideoStarted));
			ShowOptionsExtended showOptionsExtended = options as ShowOptionsExtended;
			if (showOptionsExtended != null && showOptionsExtended.gamerSid != null && showOptionsExtended.gamerSid.Length > 0)
			{
				if (!UnityAds.show(zoneId2, rewardItemKey, new Dictionary<string, string> { { "sid", showOptionsExtended.gamerSid } }))
				{
					triggerEvent(EventType.error, EventArgs.Empty);
				}
			}
			else if (!UnityAds.show(zoneId2, rewardItemKey))
			{
				triggerEvent(EventType.error, EventArgs.Empty);
			}
		}

		public override bool isShowing()
		{
			return _isShowing;
		}

		private void UnityAdsCampaignsAvailable()
		{
			Utils.LogDebug("UNITY ADS: CAMPAIGNS READY!");
			triggerEvent(EventType.initComplete, EventArgs.Empty);
			triggerEvent(EventType.adAvailable, EventArgs.Empty);
		}

		private void UnityAdsCampaignsFetchFailed()
		{
			Utils.LogDebug("UNITY ADS: CAMPAIGNS FETCH FAILED!");
			triggerEvent(EventType.initFailed, EventArgs.Empty);
		}

		private void UnityAdsShow()
		{
			Utils.LogDebug("UNITY ADS: SHOW!");
			_isShowing = true;
			triggerEvent(EventType.adWillOpen, EventArgs.Empty);
			triggerEvent(EventType.adDidOpen, EventArgs.Empty);
		}

		private void UnityAdsHide()
		{
			Utils.LogDebug("UNITY ADS: HIDE!");
			_isShowing = false;
			UnityAds.OnShow = (UnityAds.UnityAdsShow)Delegate.Remove(UnityAds.OnShow, new UnityAds.UnityAdsShow(UnityAdsShow));
			UnityAds.OnHide = (UnityAds.UnityAdsHide)Delegate.Remove(UnityAds.OnHide, new UnityAds.UnityAdsHide(UnityAdsHide));
			UnityAds.OnVideoCompleted = (UnityAds.UnityAdsVideoCompleted)Delegate.Remove(UnityAds.OnVideoCompleted, new UnityAds.UnityAdsVideoCompleted(UnityAdsVideoCompleted));
			UnityAds.OnVideoStarted = (UnityAds.UnityAdsVideoStarted)Delegate.Remove(UnityAds.OnVideoStarted, new UnityAds.UnityAdsVideoStarted(UnityAdsVideoStarted));
			triggerEvent(EventType.adWillClose, EventArgs.Empty);
			triggerEvent(EventType.adDidClose, EventArgs.Empty);
		}

		private void UnityAdsVideoCompleted(string rewardItemKey, bool skipped)
		{
			Utils.LogDebug("UNITY ADS: VIDEO COMPLETE : " + rewardItemKey + " - " + skipped);
			if (skipped)
			{
				triggerEvent(EventType.adSkipped, EventArgs.Empty);
			}
			else
			{
				triggerEvent(EventType.adFinished, EventArgs.Empty);
			}
		}

		private void UnityAdsVideoStarted()
		{
			Utils.LogDebug("UNITY ADS: VIDEO STARTED!");
			triggerEvent(EventType.adStarted, EventArgs.Empty);
		}
	}
}
