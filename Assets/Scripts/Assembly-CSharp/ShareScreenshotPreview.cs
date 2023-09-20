using System;
using Game;
using UnityEngine;

public class ShareScreenshotPreview : MonoBehaviour
{
	public UITexture PreviewTexture;

	public GameObject ScreenshotCreator;

	public Transform ScreenshotCreatorParent;

	private GameObject m_screenshotCreator;

	private Texture m_rawScreenshot;

	private PostcardCreator m_creator;

	private Texture2D m_finishedScreenshot;

	private Action<Texture2D> m_readyTextureDelegate;

	public void SetData(Texture rawScreenshot, Action<Texture2D> readyTextureCallback)
	{
		m_finishedScreenshot = null;
		m_rawScreenshot = rawScreenshot;
		m_readyTextureDelegate = readyTextureCallback;
	}

	public void CreateAndShare()
	{
		if (m_finishedScreenshot != null)
		{
			ShareScreenshot(m_finishedScreenshot);
		}
		else
		{
			m_creator.Create(ShareScreenshot);
		}
	}

	private void OnEnable()
	{
		if (m_finishedScreenshot == null)
		{
			if (m_screenshotCreator == null)
			{
				m_screenshotCreator = GOTools.Instantiate(ScreenshotCreator, ScreenshotCreatorParent.gameObject, GOTools.InstantiateOptions.None);
			}
			if (m_creator == null)
			{
				m_creator = m_screenshotCreator.GetComponent<PostcardCreator>();
			}
			GigStatus gigController = GameController.Instance.CurrentLevel.GigController;
			m_creator.SetData(m_rawScreenshot, gigController.TotalGigCoins(), SetPreview);
		}
	}

	private void SetPreview(RenderTexture previewTexture)
	{
		PreviewTexture.mainTexture = previewTexture;
	}

	private void OnDisable()
	{
		UnityEngine.Object.Destroy(m_screenshotCreator);
	}

	private void ShareScreenshot(Texture2D completedScreenshot)
	{
		if (completedScreenshot != null)
		{
			m_finishedScreenshot = completedScreenshot;
			PreviewTexture.mainTexture = completedScreenshot;
			GigStatus gigController = GameController.Instance.CurrentLevel.GigController;
			SocialShare.ShareScreenshot(completedScreenshot, gigController.TotalGigCoins());
		}
		else
		{
			Cleanup();
		}
		if (m_readyTextureDelegate != null)
		{
			m_readyTextureDelegate(completedScreenshot);
		}
		if (Application.isEditor)
		{
			Cleanup();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			Cleanup();
		}
	}

	private void Cleanup()
	{
		UnityEngine.Object.Destroy(m_creator);
		base.gameObject.SetActive(false);
	}
}
