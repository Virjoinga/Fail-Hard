using UnityEngine;

public class DownloadObbExample : MonoBehaviour
{
	private bool m_obbAvailable;

	private bool m_fetching;

	private void OnGUI()
	{
		if (!GooglePlayDownloader.RunningOnAndroid())
		{
			GUI.Label(new Rect(10f, 10f, Screen.width - 10, 20f), "Use GooglePlayDownloader only on Android device!");
		}
		else
		{
			Application.LoadLevel(1);
		}
	}
}
