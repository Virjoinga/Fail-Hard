using System.Collections;
using UnityEngine;

public class OpenLinks : MonoBehaviour
{
	private bool m_facebookAppFound;

	private float iOSVersion
	{
		get
		{
			float result = -1f;
			string text = SystemInfo.operatingSystem.Replace("iPhone OS ", string.Empty);
			float.TryParse(text.Substring(0, 1), out result);
			return result;
		}
	}

	private void Facebook()
	{
		Application.OpenURL(ExternalURL.FACEBOOK_PAGE);
	}

	private void Twitter()
	{
		Application.OpenURL(ExternalURL.TWITTER_PAGE);
	}

	private void MoreApps()
	{
		Application.OpenURL(ExternalURL.MORE_APPS);
	}

	private IEnumerator OpenFacebookLink()
	{
		m_facebookAppFound = false;
		Application.OpenURL(ExternalURL.FACEBOOK_APP_PAGE);
		yield return new WaitForSeconds(1f);
		if (!m_facebookAppFound)
		{
			Application.OpenURL(ExternalURL.FACEBOOK_PAGE);
		}
		m_facebookAppFound = false;
	}

	private void OnApplicationPause()
	{
		m_facebookAppFound = true;
	}
}
