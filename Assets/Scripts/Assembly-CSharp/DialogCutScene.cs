using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class DialogCutScene : MonoBehaviour
{
	public delegate void OnDialogClosed(bool isYes);

	public UILabel BubbleLabel;

	public UISprite Background;

	public UISprite Pointer;

	public GameObject Bubble;

	private GameObject m_picture;

	private int m_frameIndex;

	private CutsceneData m_data;

	private string m_prevResource = string.Empty;

	public Transform BubbleMirror;

	public GameObject CustomContentParent;

	private List<UIPanel> m_panelCache;

	private Camera m_uiCamera;

	public Vector2 RelativePicturePosition
	{
		get
		{
			return m_picture.GetComponent<UIAnchor>().relativeOffset;
		}
		set
		{
			m_picture.GetComponent<UIAnchor>().relativeOffset = value;
		}
	}

	public float RelativePositionX
	{
		get
		{
			return BubbleMirror.localPosition.x;
		}
		set
		{
			Vector3 localPosition = BubbleMirror.localPosition;
			localPosition.x = value;
			BubbleMirror.localPosition = localPosition;
			if (value < 0f)
			{
				BubbleMirror.localScale = Vector3.one;
			}
			else
			{
				BubbleMirror.localScale = new Vector3(-1f, 1f, 1f);
			}
		}
	}

	public string BubbleText
	{
		get
		{
			return BubbleLabel.text;
		}
		set
		{
			if (value == string.Empty)
			{
				BubbleMirror.gameObject.SetActive(false);
			}
			else
			{
				BubbleMirror.gameObject.SetActive(true);
			}
			BubbleLabel.text = value;
		}
	}

	[method: MethodImpl(32)]
	public event OnDialogClosed DialogClosed;

	public void SetData(CutsceneData data)
	{
		m_frameIndex = 0;
		m_data = data;
	}

	private void ShowFrame(CutsceneFrame frame)
	{
		if (frame.Text != string.Empty)
		{
			BubbleText = Language.Get(frame.Text);
		}
		else
		{
			BubbleText = string.Empty;
		}
		if (frame.IsThought)
		{
			Pointer.spriteName = "thoughtbubble";
			Pointer.MakePixelPerfect();
			Pointer.cachedTransform.localScale *= 0.5f;
		}
		else
		{
			Pointer.spriteName = "notification_plain_pointer";
			Pointer.MakePixelPerfect();
		}
		if (m_prevResource != frame.Picture)
		{
			if (m_picture != null)
			{
				Object.Destroy(m_picture);
			}
			m_prevResource = frame.Picture;
			GameObject prefab = (GameObject)Resources.Load(m_prevResource);
			m_picture = NGUITools.AddChild(base.gameObject, prefab);
		}
		RelativePicturePosition = new Vector2(frame.PicturePosX, frame.PicturePosY);
		RelativePositionX = frame.BubblePos;
	}

	private void Start()
	{
		HideOthers();
		AnimatedStart();
	}

	private void HideOthers()
	{
		if (m_panelCache == null)
		{
			m_panelCache = new List<UIPanel>();
		}
		UIPanel[] componentsInChildren = base.transform.parent.GetComponentsInChildren<UIPanel>();
		foreach (UIPanel uIPanel in componentsInChildren)
		{
			if (uIPanel != GetComponent<UIPanel>())
			{
				m_panelCache.Add(uIPanel);
				uIPanel.alpha = 0f;
			}
		}
	}

	private void ShowOthers()
	{
		foreach (UIPanel item in m_panelCache)
		{
			item.alpha = 1f;
		}
		m_panelCache.Clear();
	}

	public void OnYes()
	{
		ShowOthers();
		if (this.DialogClosed != null)
		{
			this.DialogClosed(true);
		}
		Object.Destroy(base.gameObject);
	}

	public void OnNo()
	{
		ShowOthers();
		if (this.DialogClosed != null)
		{
			this.DialogClosed(false);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnClick()
	{
		m_frameIndex++;
		if (m_frameIndex < m_data.Frames.Count)
		{
			ShowFrame(m_data.Frames[m_frameIndex]);
		}
		else
		{
			AnimateEnd();
		}
	}

	private void AnimateEnd()
	{
		Bubble.SetActive(false);
		if (m_picture != null)
		{
			Object.Destroy(m_picture);
		}
		TweenAlpha.Begin(Background.gameObject, 0.5f, 0f);
		base.collider.enabled = false;
		Invoke("OnNo", 0.5f);
	}

	private void AnimatedStart()
	{
		Background.alpha = 0f;
		TweenAlpha.Begin(Background.gameObject, 0.5f, 0.6f);
		base.collider.enabled = false;
		Invoke("AnimationDone", 0.5f);
	}

	private void AnimationDone()
	{
		base.collider.enabled = true;
		Bubble.SetActive(true);
		if (m_frameIndex < m_data.Frames.Count)
		{
			ShowFrame(m_data.Frames[m_frameIndex]);
		}
	}
}
