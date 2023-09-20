using UnityEngine;

public class UniRateEventHandler : MonoBehaviour
{
	private void Awake()
	{
		UniRate.Instance.ShouldUniRatePromptForRating += ShouldUniRatePromptForRating;
		UniRate.Instance.ShouldUniRateOpenRatePage += ShouldUniRateOpenRatePage;
		UniRate.Instance.OnPromptedForRating += OnPromptedForRating;
		UniRate.Instance.OnDetectAppUpdated += OnDetectAppUpdated;
		UniRate.Instance.OnUniRateFaild += OnUniRateFaild;
		UniRate.Instance.OnUserAttemptToRate += OnUserAttemptToRate;
		UniRate.Instance.OnUserDeclinedToRate += OnUserDeclinedToRate;
		UniRate.Instance.OnUserWantReminderToRate += OnUserWantReminderToRate;
	}

	private bool ShouldUniRatePromptForRating()
	{
		return true;
	}

	private bool ShouldUniRateOpenRatePage()
	{
		return true;
	}

	private void OnPromptedForRating()
	{
	}

	private void OnDetectAppUpdated()
	{
		Debug.Log("A new version is installed. Current version: " + UniRate.Instance.applicationVersion);
	}

	private void OnUniRateFaild(UniRate.Error error)
	{
		Debug.Log(error);
	}

	private void OnUserAttemptToRate()
	{
		Debug.Log("Yeh, great, user want to rate us!");
	}

	private void OnUserDeclinedToRate()
	{
		Debug.Log("User declined the rate prompt.");
	}

	private void OnUserWantReminderToRate()
	{
		Debug.Log("User wants to be reminded later.");
	}

	private void OnDestroy()
	{
		UniRate.Instance.ShouldUniRatePromptForRating -= ShouldUniRatePromptForRating;
		UniRate.Instance.ShouldUniRateOpenRatePage -= ShouldUniRateOpenRatePage;
		UniRate.Instance.OnPromptedForRating -= OnPromptedForRating;
		UniRate.Instance.OnDetectAppUpdated -= OnDetectAppUpdated;
		UniRate.Instance.OnUniRateFaild -= OnUniRateFaild;
		UniRate.Instance.OnUserAttemptToRate -= OnUserAttemptToRate;
		UniRate.Instance.OnUserDeclinedToRate -= OnUserDeclinedToRate;
		UniRate.Instance.OnUserWantReminderToRate -= OnUserWantReminderToRate;
	}
}
