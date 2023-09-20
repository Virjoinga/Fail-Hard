using UnityEngine;

public class PrivacyButton : MonoBehaviour
{
	private void OnClick()
	{
		Application.OpenURL(ExternalURL.PRIVACY_POLICY_URL);
	}
}
