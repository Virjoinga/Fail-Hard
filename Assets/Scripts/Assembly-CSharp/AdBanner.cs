using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class AdBanner : MonoBehaviour
{
	private enum BannerSize
	{
		Small = 1,
		MediumRect = 2,
		MediumBanner = 3,
		LeaderBoardBanner = 4
	}

	public enum FsAdProvider
	{
		IN_MOBI = 0,
		IN_MOBI_2 = 1,
		MILLENIAL = 2,
		ADMOB = 3,
		ADMOB_2 = 4,
		MOPUB = 5,
		FLURRY = 6,
		ADMOB_INTERSTITIAL = 7,
		ADMOB_INTERSTITIAL_2 = 8,
		CHARTBOOST = 9,
		SMAATO = 10,
		SMAATO_2 = 11,
		MOPUB_INTERSTITIAL = 12,
		ADCOLONYVID = 13,
		VUNGLEVID = 14,
		SUPERSONICVID = 15,
		FACEBOOK = 16,
		FACEBOOK_2 = 17,
		FACEBOOKINT = 18,
		CHARTBOOSTVID = 19,
		IN_MOBIINT = 20
	}

	private class StaticSettings
	{
		public List<ProviderInfo> UseAdProviders;

		public BannerSize Size = BannerSize.Small;

		public bool TestMode;
	}

	[Serializable]
	public class ProviderInfo
	{
		public FsAdProvider Provider;

		public string ProviderId;

		public string ProviderId2;
	}

	public bool TestMode;

	private static StaticSettings s_settings;

	private void Start()
	{
		if (s_settings == null)
		{
			GameController.Instance.LevelDatabase.LevelDatabasePopulated += InitializeAds;
		}
		else
		{
			ShowAd();
		}
	}

	private void OnDestroy()
	{
		DisableFsPopup();
	}

	public static void ShowAd()
	{
		if (IsAdFree())
		{
			SetAdFree();
		}
		else
		{
			ShowFsAd();
		}
	}

	public static void HideAd()
	{
		HideFsAd();
	}

	public static void ShowInterstitial()
	{
		if (IsAdFree())
		{
			SetAdFree();
			return;
		}
		Debug.Log("AdBanner.cs, ShowInterstitial");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("showInterstitial");
		}
	}

	public static void ShowVideoAd()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("showVideoAd");
		}
	}

	private static void SetAdFree(CurrencyItem item)
	{
		HideAd();
		StoreFront store = GameController.Instance.Store;
		store.PurchaseCompleted -= SetAdFree;
	}

	private void InitializeAds(LevelDatabase db)
	{
		s_settings = new StaticSettings();
		GameController.Instance.LevelDatabase.LevelDatabasePopulated -= InitializeAds;
		BannerSize size = BannerSize.MediumBanner;
		if (Screen.height > 1024)
		{
			size = BannerSize.LeaderBoardBanner;
		}
		else if (Screen.height < 550)
		{
			size = BannerSize.Small;
		}
		s_settings.TestMode = TestMode;
		s_settings.Size = size;
		List<ProviderInfo> list = new List<ProviderInfo>();
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.ADMOB,
			ProviderId = "ca-app-pub-4731967106298980/3179318153"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.ADMOB_2,
			ProviderId = "ca-app-pub-4731967106298980/6132784550"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.FACEBOOK,
			ProviderId = "1485782695061039_1487099658262676"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.FACEBOOK_2,
			ProviderId = "1485782695061039_1487105981595377"
		});
		list.Add(new ProviderInfo
		{
			ProviderId = "9e8566703c604774a09d8ebd48779406",
			ProviderId2 = "1431975799897904"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.ADMOB_INTERSTITIAL,
			ProviderId = "ca-app-pub-4731967106298980/7524863756"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.MOPUB_INTERSTITIAL,
			ProviderId = "44c97fbcaad341388a1d5e56c208914e",
			ProviderId2 = "05134fe4868b4a87aee6245469303794"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.CHARTBOOST,
			ProviderId = "562a020843150f2a55c57e95",
			ProviderId2 = "a33c8b7f96314aaf6f4347b4b8cf46c095129ace"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.FACEBOOKINT,
			ProviderId = "1485782695061039_1487107028261939"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.IN_MOBIINT,
			ProviderId = "1431975799898409"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.ADCOLONYVID,
			ProviderId = "appaf32ae500f4e42cf81",
			ProviderId2 = "vzf310364d0114420898"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.SUPERSONICVID,
			ProviderId = "2e2438d1"
		});
		list.Add(new ProviderInfo
		{
			Provider = FsAdProvider.CHARTBOOSTVID,
			ProviderId = "562a020843150f2a55c57e95",
			ProviderId2 = "a33c8b7f96314aaf6f4347b4b8cf46c095129ace"
		});
		List<ProviderInfo> useAdProviders = list;
		s_settings.UseAdProviders = useAdProviders;
		InitializeFsLink();
		ApplicationFocusController.RegisterPauseEvent(OnPauseEvent);
		if (!IsAdFree())
		{
			StoreFront store = GameController.Instance.Store;
			store.PurchaseCompleted += SetAdFree;
			ShowAd();
		}
		else
		{
			HideAd();
			SetAdFree();
		}
	}

	public static bool IsAdFree()
	{
		StoreFront store = GameController.Instance.Store;
		Debug.Log("AdBanner.cs is adfree? " + store.HasAnyPreviousPurchases);
		return store.HasAnyPreviousPurchases;
	}

	private static void InitializeFsLink()
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			foreach (ProviderInfo useAdProvider in s_settings.UseAdProviders)
			{
				Debug.Log("Trying to send provider " + useAdProvider.ProviderId);
				androidJavaObject.CallStatic("addProviderId", (int)useAdProvider.Provider, useAdProvider.ProviderId, useAdProvider.ProviderId2);
			}
			androidJavaObject.CallStatic("create", s_settings.TestMode);
		}
	}

	private static void SetAdFree()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("setAdFree");
		}
	}

	private static void ShowFsAd()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("showAd");
		}
	}

	private static void HideFsAd()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("hideAd");
		}
	}

	public static bool HasVideoAds()
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			bool flag = androidJavaObject.CallStatic<bool>("hasVideoAds", new object[0]);
			Debug.Log("Has video ads? " + flag);
			return flag;
		}
	}

	private static void DisableFsPopup()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("disablePopup");
		}
	}

	private static void OnPauseEvent(bool paused)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.viimagames.plugins.FsLink"))
		{
			androidJavaClass.CallStatic("pauseEvent", paused);
		}
	}
}
