using System.Collections.Generic;
using UnityEngine;

public class ScreenShotRenderTexture : MonoBehaviour
{
	private const int RENDER_TEXTURE_LIMIT = 1;

	public bool TakeScreenShot;

	private List<RenderTexture> m_textures;

	private bool m_screenshotTaken;

	private void OnPostRender()
	{
		if (TakeScreenShot)
		{
			Camera main = Camera.main;
			RenderTexture renderTexture = CachedRenderTexture();
			if (renderTexture != null)
			{
				Graphics.Blit(main.targetTexture, renderTexture);
			}
			m_screenshotTaken = true;
			TakeScreenShot = false;
		}
	}

	private void OnDestroy()
	{
		Clear();
		foreach (RenderTexture texture in m_textures)
		{
			RenderTexture.ReleaseTemporary(texture);
		}
	}

	public void Init()
	{
		m_textures = new List<RenderTexture>();
		base.camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
		for (int i = 0; i < 1; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
			m_textures.Add(temporary);
		}
		m_screenshotTaken = false;
	}

	public void Clear()
	{
		m_screenshotTaken = false;
	}

	public RenderTexture GetScreenshot()
	{
		if (m_screenshotTaken)
		{
			return m_textures[0];
		}
		return null;
	}

	private RenderTexture CachedRenderTexture()
	{
		return m_textures[0];
	}
}
