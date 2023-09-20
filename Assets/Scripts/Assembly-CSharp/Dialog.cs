using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Holoville.HOTween;
using UnityEngine;

public class Dialog : MonoBehaviour
{
	public delegate void OnDialogClosed(bool isYes);

	public UISprite CurrencyIcon;

	public UISprite CharacterSprite;

	public UILabel TitleLabel;

	public UILabel DescriptionLabel;

	public UILabel CostLabel;

	public GameObject CustomContentParent;

	public ButtonWithTextAndIcon yesButton;

	private List<UIPanel> m_hiddenPanels;

	private bool m_started;

	public Transform PanelsParent { get; set; }

	public string Character
	{
		set
		{
			CharacterSprite.spriteName = value;
		}
	}

	public string Cost
	{
		set
		{
			CostLabel.text = value;
		}
	}

	public string Title
	{
		get
		{
			return TitleLabel.text;
		}
		set
		{
			TitleLabel.text = value;
		}
	}

	public string Description
	{
		get
		{
			return DescriptionLabel.text;
		}
		set
		{
			DescriptionLabel.text = value;
		}
	}

	[method: MethodImpl(32)]
	public event OnDialogClosed DialogClosed;

	private void Start()
	{
		if (!m_started)
		{
			m_started = true;
			HideOtherPanels();
			GetComponentInChildren<HighlightButton>().ShowHighlight();
		}
	}

	private void Update()
	{
		if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && Input.GetKeyUp(KeyCode.Escape))
		{
			OnNo();
		}
	}

	private void OnDisable()
	{
		ShowOtherPanels();
	}

	private void OnClick()
	{
		OnNo();
	}

	public void HideCurrencyIcon(bool isEnabled)
	{
		CurrencyIcon.gameObject.SetActive(isEnabled);
	}

	public void OnYes()
	{
		if (this.DialogClosed != null)
		{
			this.DialogClosed(true);
			Object.Destroy(base.gameObject);
		}
	}

	public void OnNo()
	{
		if (this.DialogClosed != null)
		{
			this.DialogClosed(false);
			Object.Destroy(base.gameObject);
		}
	}

	private void HideOtherPanels()
	{
		m_started = true;
		UIPanel component = GetComponent<UIPanel>();
		component.alpha = 0f;
		HOTween.To(component, 0.2f, "alpha", 1f);
		if (!(PanelsParent != null))
		{
			return;
		}
		m_hiddenPanels = GOTools.FindAllComponents<UIPanel>(PanelsParent.gameObject);
		m_hiddenPanels.Remove(component);
		foreach (UIPanel hiddenPanel in m_hiddenPanels)
		{
			HOTween.To(hiddenPanel, 0.1f, "alpha", 0f);
		}
	}

	private void ShowOtherPanels()
	{
		UIPanel component = GetComponent<UIPanel>();
		HOTween.To(component, 0.1f, "alpha", 0f);
		if (m_hiddenPanels == null || m_hiddenPanels.Count <= 0)
		{
			return;
		}
		foreach (UIPanel hiddenPanel in m_hiddenPanels)
		{
			HOTween.To(hiddenPanel, 0.2f, "alpha", 1f);
		}
	}
}
