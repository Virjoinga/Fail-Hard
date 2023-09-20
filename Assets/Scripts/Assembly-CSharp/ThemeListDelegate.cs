using System.Collections;
using System.Runtime.CompilerServices;
using Game;
using Holoville.HOTween;
using UnityEngine;

public class ThemeListDelegate : MonoBehaviour
{
	public delegate void OnItemSelected(ThemeListDelegate listItem);

	public Bundle Bundle;

	public UILabel categoryLabel;

	public UILabel targetsLabel;

	public UILabel PriceLabel;

	public GameObject LockedTheme;

	public GameObject UnlockedTheme;

	public GameObject CompletedTheme;

	public Transform ImagesRoot;

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	public void SetData(Bundle data)
	{
		Bundle = data;
		Bundle.StateChanged += Bundle_StateChanged;
		categoryLabel.text = Bundle.Name;
		string path = ((Bundle.Id == "PV50City") ? "Levels/citybundle" : ((Bundle.Id == "PV50Backyard") ? "Levels/suburbbundle" : ((Bundle.Id == "PV50Beach") ? "Levels/beachbundle" : ((Bundle.Id == "Wings") ? "Levels/wingsbundle" : ((Bundle.Id == "PV50Beginner") ? "Levels/gravelpitbundle" : ((Bundle.Id == "Rocket") ? "Levels/rocketbundle" : ((Bundle.Id == "GravelUpdate1") ? "Levels/waterbundle" : ((Bundle.Id == "Snow") ? "Levels/snowbundle" : ((Bundle.Id == "Jungle1") ? "Levels/junglebundle" : ((Bundle.Id == "Monster") ? "Levels/monsterbundle" : ((Bundle.Id == "HC") ? "Levels/hcbundle" : ((Bundle.Id == "WATER PLAY") ? "Levels/waterplaybundle" : ((Bundle.Id == "SpeedJump") ? "Levels/speedjumpbundle" : ((Bundle.Id == "Streets") ? "Levels/alleybundle" : ((Bundle.Id == "XMAS") ? "Levels/xmasbundle" : ((Bundle.Id == "Four Seasons") ? "Levels/fourseasonsbundle" : ((!(Bundle.Id == "HIGHWAY")) ? "Levels/beachbundle" : "Levels/highwaybundle")))))))))))))))));
		GameObject prefab = (GameObject)Resources.Load(path);
		NGUITools.AddChild(ImagesRoot.gameObject, prefab);
		RefreshTargets();
	}

	private void Bundle_StateChanged(Bundle bundle)
	{
		RefreshTargets();
	}

	private void OnDestroy()
	{
		Bundle.StateChanged -= Bundle_StateChanged;
	}

	public void OnClick()
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(this);
		}
	}

	private void RefreshTargets()
	{
		int achievedTargets = Bundle.AchievedTargets;
		int num = Bundle.TotalTargets();
		if (Bundle.State == BundleState.BundleLocked)
		{
			if (GameController.Instance.Character.ContainsEvents(Bundle.Preconditions))
			{
				if (Bundle.Price.Amount == 0)
				{
					GameController.Instance.Agent.BundlePurchased(Bundle);
					GameController.Instance.BundleToShow = Bundle;
				}
				else
				{
					Bundle.State = BundleState.BundleUnlocked;
				}
			}
			else
			{
				LockedTheme.SetActive(true);
				UnlockedTheme.SetActive(false);
			}
		}
		else if (Bundle.State == BundleState.BundleUnlocked)
		{
			LockedTheme.SetActive(false);
			UnlockedTheme.SetActive(true);
			PriceLabel.text = Bundle.Price.Amount.ToString();
			targetsLabel.text = achievedTargets + "/" + num;
		}
		else if (Bundle.State == BundleState.BundleCompletePending)
		{
			LockedTheme.SetActive(false);
			UnlockedTheme.SetActive(false);
			base.collider.enabled = false;
			categoryLabel.text = Bundle.Name;
			targetsLabel.text = achievedTargets + "/" + num;
		}
		else if (Bundle.State == BundleState.BundleCompleted)
		{
			LockedTheme.SetActive(false);
			UnlockedTheme.SetActive(false);
			CompletedTheme.SetActive(true);
			categoryLabel.text = Bundle.Name;
			targetsLabel.text = achievedTargets + "/" + num;
		}
		else
		{
			LockedTheme.SetActive(false);
			UnlockedTheme.SetActive(false);
			categoryLabel.text = Bundle.Name;
			targetsLabel.text = achievedTargets + "/" + num;
		}
	}

	public void AnimateCompleted()
	{
		StartCoroutine(DelayedAnimationStart());
	}

	private IEnumerator DelayedAnimationStart()
	{
		CompletedTheme.SetActive(false);
		yield return new WaitForSeconds(1f);
		CompletedTheme.SetActive(true);
		CompletedTheme.transform.localScale *= 2f;
		Vector3 targetScale = CompletedTheme.transform.localScale * 0.5f;
		HOTween.To(p_parms: new TweenParms().Prop("localScale", targetScale).Ease(EaseType.EaseInQuint).OnComplete(AnimationComplete), p_target: CompletedTheme.transform, p_duration: 0.5f);
	}

	private void AnimationComplete()
	{
		Bundle.State = BundleState.BundleCompleted;
		int num = (int)((float)Bundle.Price.Amount * 0.25f);
		if (num == 0)
		{
			num = 10000;
		}
		GameController.Instance.Character.Coins += num;
		NotificationCentre.Send(Notification.Types.BundleCompletedAward, num);
		base.collider.enabled = true;
		RefreshTargets();
	}

	public void OnEnable()
	{
		if (Bundle != null)
		{
			RefreshTargets();
		}
	}
}
