using UnityEngine;

public class SpawnGarageView : MonoBehaviour
{
	public GameObject GaragePrefab;

	public GameObject GarageRoot;

	public Ads Ads;

	public LevelIntroHUD LevelIntro;

	private void OnClick()
	{
		LevelIntro.InterruptedWithGarage(true);
	}

	public void ManualTrigger()
	{
		Ads.Show();
		OnClick();
		GetComponent<NavigateFromTo>().ManualTrigger();
	}
}
