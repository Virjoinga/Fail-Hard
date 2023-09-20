using System.Collections.Generic;
using UnityEngine;

public class GridLocationDisplay : MonoBehaviour
{
	public UIDraggablePanel TargetGridPanel;

	public GameObject GridLocatorItem;

	private UIGrid m_displayGrid;

	private Transform m_targetGridTransform;

	private List<GameObject> m_targetItems;

	private List<GridLocatorItem> m_locatorItems;

	private AggressiveCenterOnChild m_centerer;

	private GameObject m_previousCentered;

	private void OnEnable()
	{
		if (TargetGridPanel != null)
		{
			SetGrid(TargetGridPanel);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		if (m_locatorItems != null)
		{
			foreach (GridLocatorItem locatorItem in m_locatorItems)
			{
				Object.Destroy(locatorItem.gameObject);
			}
			m_locatorItems.Clear();
		}
		m_previousCentered = null;
	}

	private void Update()
	{
		if (m_targetItems.Count != m_targetGridTransform.childCount)
		{
			SetGrid(TargetGridPanel);
		}
		if (m_previousCentered == null || m_centerer.centeredObject != m_previousCentered)
		{
			UpdateDisplayLocation();
		}
	}

	private void SetGrid(UIDraggablePanel targetPanel)
	{
		TargetGridPanel = targetPanel;
		UIGrid uIGrid = GOTools.FindFromChildren<UIGrid>(TargetGridPanel.gameObject);
		if (!uIGrid)
		{
			return;
		}
		m_targetGridTransform = uIGrid.transform;
		m_targetItems = GOTools.FindChildren(m_targetGridTransform.gameObject);
		if (uIGrid.sorted)
		{
			m_targetItems.Sort((GameObject x, GameObject y) => x.name.CompareTo(y.name));
		}
		if (m_targetItems.Count > 0)
		{
			m_displayGrid = GetComponent<UIGrid>();
			if (m_displayGrid == null)
			{
				m_displayGrid = base.gameObject.AddComponent<UIGrid>();
				m_displayGrid.cellHeight = 30f;
				m_displayGrid.cellWidth = 30f;
			}
			CreateDisplayItems();
		}
	}

	private void CreateDisplayItems()
	{
		if (m_locatorItems == null)
		{
			m_locatorItems = new List<GridLocatorItem>();
		}
		for (int i = 0; i < m_targetItems.Count; i++)
		{
			GameObject gameObject = GOTools.Instantiate(GridLocatorItem, base.gameObject);
			GridLocatorItem component = gameObject.GetComponent<GridLocatorItem>();
			m_locatorItems.Add(component);
		}
		m_displayGrid.Reposition();
		m_centerer = GOTools.FindFromChildren<AggressiveCenterOnChild>(TargetGridPanel.gameObject);
	}

	private void UpdateDisplayLocation()
	{
		if (!m_centerer)
		{
			return;
		}
		GameObject closest = m_centerer.centeredObject;
		if (closest == null)
		{
			Transform transform = m_centerer.FindClosest();
			if (transform != null)
			{
				closest = transform.gameObject;
			}
		}
		int num = 0;
		if ((bool)closest)
		{
			num = m_targetItems.FindIndex((GameObject x) => x == closest);
		}
		if (num >= 0 && num < m_targetItems.Count)
		{
			m_previousCentered = m_targetItems[num];
		}
		else
		{
			num = 0;
		}
		for (int i = 0; i < m_locatorItems.Count; i++)
		{
			m_locatorItems[i].SetState(i == num);
		}
	}
}
