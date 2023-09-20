using UnityEngine;

public class ScreenShotButton : MonoBehaviour
{
	public ShareScreenshotPreview Preview;

	public UITexture ScreenShotTexture;

	private RenderTexture m_screenshot;

	private Texture2D m_finishedScreenshot;

	public void OnEnable()
	{
		Camera main = Camera.main;
		ScreenShotRenderTexture component = main.GetComponent<ScreenShotRenderTexture>();
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			float result = -1f;
			string text = SystemInfo.operatingSystem.Replace("iPhone OS ", string.Empty);
			float.TryParse(text.Substring(0, 1), out result);
			if (result > 0f && result < 6f)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		if ((bool)component)
		{
			m_screenshot = component.GetScreenshot();
			if (m_screenshot != null)
			{
				ScreenShotTexture.mainTexture = m_screenshot;
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		m_finishedScreenshot = null;
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && m_finishedScreenshot == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		if ((bool)m_screenshot && m_finishedScreenshot == null)
		{
			Preview.SetData(m_screenshot, SetCreatedTexture);
			Preview.gameObject.SetActive(true);
		}
		else if (m_finishedScreenshot != null)
		{
			Preview.gameObject.SetActive(true);
		}
	}

	private void SetCreatedTexture(Texture2D texture)
	{
		if (texture != null)
		{
			m_finishedScreenshot = texture;
			ScreenShotTexture.mainTexture = texture;
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
