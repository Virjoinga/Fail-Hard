using UnityEngine;

public class ShowInterstitial : MonoBehaviour
{
	private const int INTERSTITIAL_DELAY = 120;

	public Ads Ads;

	private static int s_lastInterstitialShowtime = 120;

	private void Start()
	{
		int num = (int)Time.realtimeSinceStartup;
		if (num - s_lastInterstitialShowtime > 120)
		{
			Debug.Log("ShowInterstitial.cs Start()");
			Ads.ShowInterstitial();
			s_lastInterstitialShowtime = num;
		}
	}
}
