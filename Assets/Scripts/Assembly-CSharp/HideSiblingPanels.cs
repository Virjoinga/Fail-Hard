using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

[RequireComponent(typeof(UIPanel))]
public class HideSiblingPanels : MonoBehaviour
{
	private bool m_started;

	private Transform m_panelsParent;

	private List<UIPanel> m_hiddenPanels;

	private void Start()
	{
		if (!m_started)
		{
			m_panelsParent = base.transform.parent;
			m_started = true;
			m_hiddenPanels = new List<UIPanel>();
		}
	}

	private void OnEnable()
	{
		if (!m_started)
		{
			Start();
		}
		UIPanel component = GetComponent<UIPanel>();
		component.alpha = 0f;
		HOTween.To(component, 0.2f, "alpha", 1f);
		if (!(m_panelsParent != null))
		{
			return;
		}
		m_hiddenPanels = GOTools.FindAllFromChildren<UIPanel>(m_panelsParent.gameObject);
		m_hiddenPanels.Remove(component);
		foreach (UIPanel hiddenPanel in m_hiddenPanels)
		{
			HOTween.To(hiddenPanel, 0.1f, "alpha", 0f);
		}
	}

	private void OnDisable()
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
		m_hiddenPanels.Clear();
	}
}
