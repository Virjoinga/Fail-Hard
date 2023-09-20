using Game;
using Holoville.HOTween;
using UnityEngine;

public class Ads : MonoBehaviour
{
	public GameObject AdsButton;

	public int SlideInLength;

	public float SlideInTime;

	private bool m_shown;

	private Vector3 m_cachePos;

	private bool m_initialized;

	private bool pauseStatus;

	private void Start()
	{
		Debug.Log("Ads start");
		m_cachePos = AdsButton.transform.localPosition;
		Show();
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (pauseStatus || !focusStatus)
		{
			return;
		}
		Debug.Log("Should show ads");
		if (GameController.Instance.LevelDatabase.Initialized)
		{
			Debug.Log(GameController.Instance.CurrentView);
			switch (GameController.Instance.CurrentView)
			{
			case GameController.GameUIView.MainView:
				Show();
				break;
			case GameController.GameUIView.Garage:
				Show();
				break;
			case GameController.GameUIView.ThemeSelection:
				Show();
				break;
			case GameController.GameUIView.LevelGrid:
				Show();
				break;
			}
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		this.pauseStatus = pauseStatus;
	}

	public void Show()
	{
		AdBanner.ShowAd();
		m_shown = true;
		m_initialized = true;
	}

	public void Hide()
	{
		AdBanner.HideAd();
		m_shown = false;
		m_initialized = true;
	}

	public void ShowInterstitial()
	{
		Hide();
		AdBanner.ShowInterstitial();
	}

	public bool IsAdFree()
	{
		return AdBanner.IsAdFree();
	}

	private void SlideButton(GameObject button, Vector3 origo, float slideAmount, bool collisions)
	{
		TweenParms p_parms = new TweenParms().Prop("localPosition", origo + slideAmount * Vector3.up, false).Ease(EaseType.EaseOutBounce).OnComplete(EnableDisable);
		HOTween.To(button.transform, SlideInTime, p_parms);
		Collider[] componentsInChildren = button.GetComponentsInChildren<Collider>(true);
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = collisions;
		}
	}

	private void EnableDisable()
	{
		AdsButton.SetActive(m_shown);
	}
}
