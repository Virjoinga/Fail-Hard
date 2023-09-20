using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class NotificationTray : MonoBehaviour
{
	public float SlideInLength;

	public float SlideInTime;

	public float ShowingTime;

	public UISprite CharacterSprite;

	public bool IsBlocking;

	public Transform CloseButton;

	public UILabel DescriptionLabel;

	private string m_textToShow;

	private string m_characterIconName;

	private Action m_hiddenAction;

	private void OnClick()
	{
		Hide();
		CancelInvoke();
	}

	private void ScaleEffect()
	{
		if (!(CloseButton == null))
		{
			Vector3 vector = 1.1f * CloseButton.localScale;
			TweenParms p_parms = new TweenParms().Prop("localScale", vector).Loops(2, LoopType.Yoyo);
			HOTween.To(CloseButton, 0.2f, p_parms);
		}
	}

	public void SetData(string text, string iconName = "", Action hideAction = null)
	{
		m_textToShow = text;
		m_characterIconName = iconName;
		CharacterSprite.spriteName = m_characterIconName;
		m_hiddenAction = hideAction;
		StartCoroutine(Show());
	}

	public void SetData(string text, bool isBlocking, string iconName = "", Action hideAction = null)
	{
		m_textToShow = text;
		m_characterIconName = iconName;
		CharacterSprite.spriteName = m_characterIconName;
		m_hiddenAction = hideAction;
		IsBlocking = isBlocking;
		StartCoroutine(Show());
	}

	private IEnumerator Show()
	{
		yield return null;
		UIAnchor anchor = GetComponent<UIAnchor>();
		if ((bool)anchor)
		{
			anchor.enabled = false;
		}
		DescriptionLabel.text = m_textToShow;
		Vector3 newPos = base.transform.localPosition;
		newPos.x -= SlideInLength;
		HOTween.To(base.transform, SlideInTime, "localPosition", newPos);
		Invoke("Hide", ShowingTime);
	}

	private void Hide()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x += SlideInLength;
		TweenParms p_parms = new TweenParms().Prop("localPosition", localPosition).OnComplete(DespawnTray);
		HOTween.To(base.transform, SlideInTime, p_parms);
	}

	private void DespawnTray()
	{
		if (base.gameObject.activeSelf)
		{
			UIAnchor component = GetComponent<UIAnchor>();
			if ((bool)component)
			{
				component.enabled = true;
			}
			GOTools.Despawn(base.gameObject);
			if (m_hiddenAction != null)
			{
				m_hiddenAction();
			}
		}
	}
}
