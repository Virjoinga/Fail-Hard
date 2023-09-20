using System;
using System.Collections;
using UnityEngine;

public class PostcardCreator : MonoBehaviour
{
	public int PostcardWidth;

	public int PostcardHeight;

	public UITexture Picture;

	public Camera PhotoCamera;

	public UILabel CoinsLabel;

	public Action<Texture2D> m_createdAction;

	public Action<RenderTexture> m_renderAction;

	private RenderTexture m_targetTexture;

	private void Start()
	{
		m_targetTexture = RenderTexture.GetTemporary(PostcardWidth, PostcardHeight, 0);
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		float num = (float)uIRoot.manualHeight / (float)PostcardHeight;
		base.transform.localScale = Vector3.one * num;
		ScaleScreenshot();
		StartCoroutine(Render());
	}

	private void OnDestroy()
	{
		if (m_targetTexture != null)
		{
			RenderTexture.ReleaseTemporary(m_targetTexture);
		}
	}

	public void SetData(Texture texture, int coins, Action<RenderTexture> previewAction)
	{
		CoinsLabel.text = "COINS " + coins;
		Picture.mainTexture = texture;
		m_renderAction = previewAction;
	}

	public void Create(Action<Texture2D> createAction)
	{
		m_createdAction = createAction;
		StartCoroutine(CreatePhoto());
	}

	private void ScaleScreenshot()
	{
		Transform cachedTransform = Picture.cachedTransform;
		float num = 1f;
		int height = Picture.mainTexture.height;
		int width = Picture.mainTexture.width;
		cachedTransform.localScale = new Vector2(width, height);
		if (height != PostcardHeight)
		{
			num /= (float)height / (float)PostcardHeight;
		}
		if (num * (float)width < (float)PostcardWidth)
		{
			num *= (float)PostcardWidth / (num * (float)width);
		}
		cachedTransform.localScale *= num;
	}

	private IEnumerator Render()
	{
		yield return new WaitForEndOfFrame();
		PhotoCamera.targetTexture = m_targetTexture;
		PhotoCamera.Render();
		if (m_renderAction != null)
		{
			m_renderAction(m_targetTexture);
		}
	}

	private IEnumerator CreatePhoto()
	{
		yield return new WaitForSeconds(0.2f);
		yield return new WaitForEndOfFrame();
		Texture2D texture = new Texture2D(m_targetTexture.width, m_targetTexture.height);
		RenderTexture backup = RenderTexture.active;
		RenderTexture.active = m_targetTexture;
		texture.ReadPixels(new Rect(0f, 0f, m_targetTexture.width, m_targetTexture.height), 0, 0);
		texture.Apply();
		RenderTexture.active = backup;
		if (m_createdAction != null)
		{
			m_createdAction(texture);
			m_createdAction = null;
		}
		else
		{
			Debug.LogWarning("Could not send composed screenshot anywhere!");
		}
	}
}
