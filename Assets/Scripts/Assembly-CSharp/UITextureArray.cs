using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
public class UITextureArray : MonoBehaviour
{
	public GameObject spritePrefab;

	private UIGrid m_grid;

	private List<GameObject> m_array;

	private int m_requestedAmount;

	private void Awake()
	{
		m_array = new List<GameObject>();
		foreach (Transform item in base.transform)
		{
			m_array.Add(item.gameObject);
		}
		m_grid = GetComponent<UIGrid>();
	}

	public void Update()
	{
		while (m_requestedAmount > m_array.Count)
		{
			AddNew();
		}
		while (m_requestedAmount < m_array.Count)
		{
			RemoveOne();
		}
	}

	public void ShowAmount(int count)
	{
		m_requestedAmount = count;
	}

	public void SetStateOff(int amount)
	{
		List<GameObject> range = m_array.GetRange(m_array.Count - amount, amount);
		foreach (GameObject item in range)
		{
			UISpriteTuple component = item.GetComponent<UISpriteTuple>();
			component.SetOff();
		}
	}

	private void AddNew()
	{
		GameObject item = GOTools.InstantiateWithOriginalScale(base.gameObject, spritePrefab);
		m_array.Add(item);
		m_grid.Reposition();
	}

	private void RemoveOne()
	{
		int index = m_array.Count - 1;
		GameObject obj = m_array[index];
		m_array.RemoveAt(index);
		Object.Destroy(obj);
		m_grid.Reposition();
	}
}
