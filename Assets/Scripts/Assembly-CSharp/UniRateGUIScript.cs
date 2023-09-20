using System.Text;
using UnityEngine;

public class UniRateGUIScript : MonoBehaviour
{
	private string bundleIDString = "Bundle ID: {0}\n";

	private string appIDString = "App ID: {0}\n";

	private string appNameString = "App Name: {0}\n";

	private string appVersionString = "App Version: {0}\n";

	private string usesUntilPromptString = "Uses Until Prompt: {0}\n";

	private string usedString = "Already Used Count: {0}\n";

	private string eventsUntilPromptString = "Events Until Prompt: {0}\n";

	private string eventsHappenedString = "Events Happened: {0}\n";

	private string daysUntilPromptString = "Days Until Prompt:{0}\n";

	private string daysUsedString = "Days From First Use:{0}\n";

	private string usesPerWeekToPromptString = "Uses per week to prompt:{0}\n";

	private string usesPerWeekString = "Current uses per week:{0}\n";

	private string remindString = "Remind after:{0} days\n";

	private void OnGUI()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format(bundleIDString, UniRate.Instance.applicationBundleID));
		stringBuilder.Append(string.Format(appIDString, UniRate.Instance.appStoreID));
		stringBuilder.Append(string.Format(appNameString, UniRate.Instance.applicationName));
		stringBuilder.Append(string.Format(appVersionString, UniRate.Instance.applicationVersion));
		stringBuilder.Append(string.Format(usesUntilPromptString, UniRate.Instance.usesUntilPrompt));
		stringBuilder.Append(string.Format(usedString, UniRate.Instance.usesCount));
		stringBuilder.Append(string.Format(eventsUntilPromptString, UniRate.Instance.eventsUntilPrompt));
		stringBuilder.Append(string.Format(eventsHappenedString, UniRate.Instance.eventCount));
		stringBuilder.Append(string.Format(daysUntilPromptString, UniRate.Instance.daysUntilPrompt));
		stringBuilder.Append(string.Format(daysUsedString, UniRate.Instance.usedDays));
		stringBuilder.Append(string.Format(usesPerWeekToPromptString, UniRate.Instance.usesPerWeekForPrompt));
		stringBuilder.Append(string.Format(usesPerWeekString, UniRate.Instance.usesPerWeek));
		if (UniRate.Instance.waitingByRemindLater)
		{
			stringBuilder.Append(string.Format(remindString, UniRate.Instance.leftRemindDays));
		}
		GUI.Label(new Rect(0f, 0f, 300f, 500f), stringBuilder.ToString());
		if (GUI.Button(new Rect(0f, Screen.height - 50, 100f, 50f), "Rate"))
		{
			UniRate.Instance.RateIfNetworkAvailable();
		}
		if (GUI.Button(new Rect(100f, Screen.height - 50, 100f, 50f), "Rate Prompt"))
		{
			UniRate.Instance.PromptIfNetworkAvailable();
		}
		if (GUI.Button(new Rect(200f, Screen.height - 50, 100f, 50f), "LogEvent"))
		{
			UniRate.Instance.LogEvent(true);
		}
		if (GUI.Button(new Rect(300f, Screen.height - 50, 100f, 50f), "Reset"))
		{
			UniRate.Instance.Reset();
		}
	}
}
