using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class CurrencyItemPage : MonoBehaviour
{
	public GameObject CurrencyItemPrototype;

	public GameObject GridParent;

	public void SetData(List<CurrencyItem> items, Action<CurrencyItem> clickAction)
	{
		foreach (CurrencyItem item in items)
		{
			GameObject gameObject = GOTools.Instantiate(CurrencyItemPrototype, GridParent);
			CurrencyItemPrefab component = gameObject.GetComponent<CurrencyItemPrefab>();
			component.SetData(item, clickAction);
		}
	}

	public void FillWithDummy(int toChildCount)
	{
		while (GridParent.transform.childCount < 3)
		{
			GameObject gameObject = GOTools.Instantiate(CurrencyItemPrototype, GridParent);
			CurrencyItemPrefab component = gameObject.GetComponent<CurrencyItemPrefab>();
			component.SetDummyMode();
		}
	}

	private void Start()
	{
		UIAnchor uIAnchor = GOTools.FindFromChildren<UIAnchor>(base.gameObject);
		UIGrid uIGrid = GOTools.FindFromChildren<UIGrid>(base.gameObject);
		if ((bool)uIAnchor && (bool)uIGrid)
		{
			float num = uIGrid.cellWidth * 3f;
			uIAnchor.pixelOffset.x = 0f - (num / 2f - uIGrid.cellWidth / 2f);
			uIAnchor.relativeOffset = Vector2.zero;
		}
	}
}
