using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class VideoAds : MonoBehaviour
{
	public delegate void VideoEvent();

	private const bool m_useFixedReward = false;

	public float RewardFactor;

	private float m_fixedReward;

	public static VideoAds Instance;

	private static bool s_initialized;

	private int m_adCount;

	private int m_watchSessionCoins;

	private IAPParameters m_iapParameters;

	public static bool WatchedDuringSession;

	[method: MethodImpl(32)]
	public event VideoEvent VideoClosed;

	[method: MethodImpl(32)]
	public event VideoEvent NoMoreVideos;

	public static void ShowVideo()
	{
		if ((bool)Instance)
		{
			if (Instance.VideoClosed != null)
			{
				Instance.VideoClosed();
			}
			WatchedDuringSession = true;
			Instance.ShowVideoAd();
		}
	}

	private IEnumerator<object> DelayedNotification()
	{
		yield return new WaitForSeconds(1.5f);
		NotificationCentre.Send(Notification.Types.SupersonicAdWatched, (int)m_fixedReward);
	}

	public static void UnityAdsVideoStarted()
	{
	}

	public void ShowVideoAd()
	{
		AdBanner.ShowVideoAd();
	}

	public bool HasVideoAds()
	{
		return AdBanner.HasVideoAds();
	}

	public static int VideoCount()
	{
		Debug.Log("VideoCount?");
		if ((bool)Instance && Instance.HasVideoAds())
		{
			return 1;
		}
		return 0;
	}

	private void Start()
	{
		if (s_initialized)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		s_initialized = true;
		Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
	}

	public void Initialize(StoreFront store)
	{
		m_fixedReward = RewardFactor * (float)store.IAPParameters.MoneyPacks[0];
	}

	public void VideoAdViewed(string message)
	{
		Debug.Log("Video ad viewed");
		GameController.Instance.Character.Coins += (int)m_fixedReward;
		Tracking.BusinessEvent(Tracking.Events.ApplifierAdWatched, "applifierAdCoins", (int)m_fixedReward);
		StartCoroutine(DelayedNotification());
	}

	public void VideoAdFailed(string message)
	{
		Debug.Log("Video ad failed");
	}

	public void VideoAdCancelled(string message)
	{
		Debug.Log("Video ad cancelled");
	}
}
