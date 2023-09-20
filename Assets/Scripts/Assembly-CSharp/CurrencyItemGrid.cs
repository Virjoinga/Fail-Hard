using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class CurrencyItemGrid : MonoBehaviour
{
	private const int ITEMS_PER_PAGE = 3;

	public GameObject CurrencyItemPage;

	public ErrorInfo ErrorInfo;

	public UIPanel ClippingPanel;

	private bool m_started;

	private List<GameObject> m_listItems;

	public void Start()
	{
		InitGrid();
		m_started = true;
	}

	public void InitGrid()
	{
		if (m_listItems == null)
		{
			m_listItems = new List<GameObject>();
		}
		ClearGrid();
		if (Application.internetReachability != 0)
		{
			StoreFront store = GameController.Instance.Store;
			List<CurrencyItem> list = store.CurrencyItems();
			int num = 0;
			while (num < list.Count)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, CurrencyItemPage);
				List<CurrencyItem> list2 = new List<CurrencyItem>();
				int i;
				for (i = 0; num + i < list.Count && i < 3; i++)
				{
					list2.Add(list[num + i]);
				}
				num += i;
				CurrencyItemPage component = gameObject.GetComponent<CurrencyItemPage>();
				component.SetData(list2, OnItemClicked);
				m_listItems.Add(gameObject);
			}
			if ((list.Count == 0 || list.TrueForAll((CurrencyItem x) => x.Type == CurrencyItem.PackType.GetJar || x.Type == CurrencyItem.PackType.VideoAds)) && store.IAPEnabled)
			{
				CurrencyItemPage currencyItemPage = null;
				GameObject gameObject2 = null;
				if (m_listItems.Count == 0)
				{
					gameObject2 = NGUITools.AddChild(base.gameObject, CurrencyItemPage);
					m_listItems.Add(gameObject2);
				}
				else
				{
					gameObject2 = m_listItems[0];
				}
				currencyItemPage = gameObject2.GetComponent<CurrencyItemPage>();
				currencyItemPage.FillWithDummy(3);
				store.InventoryUpdated += UpdateIAPContents;
				StartCoroutine(PollStoreInventory());
			}
		}
		if (m_listItems.Count > 0)
		{
			UIGrid component2 = GetComponent<UIGrid>();
			if (!component2)
			{
				return;
			}
			float num2 = (float)Screen.width / (float)Screen.height;
			float num3 = 1.3333334f;
			Vector4 clipRange = ClippingPanel.clipRange;
			clipRange.z = 1024f * num2 / num3;
			ClippingPanel.clipRange = clipRange;
			component2.cellWidth = 1024f * num2 / num3;
			float num4 = 1024f * num2 / num3 - 840f;
			component2.cellWidth -= 0.7f * num4;
			foreach (GameObject listItem in m_listItems)
			{
				BoxCollider component3 = listItem.GetComponent<BoxCollider>();
				Vector3 size = component3.size;
				size.x = component2.cellWidth;
				component3.size = size;
			}
			component2.Reposition();
		}
		else if (Application.internetReachability != 0)
		{
			ErrorInfo.Show("Could not contact store. Please try again later!");
		}
		else
		{
			ErrorInfo.Show("No network connectivity. Please check your settings and try again!");
		}
	}

	public void OnEnable()
	{
		if (m_started)
		{
			InitGrid();
		}
		GetComponent<Collider>().enabled = false;
	}

	public void OnDisable()
	{
		ClearGrid();
		StoreFront store = GameController.Instance.Store;
		store.PurchaseCompleted -= PurchaseDone;
		store.PurchaseFailed -= PurchaseFailed;
		store.InventoryUpdated -= UpdateIAPContents;
		VideoAds.Instance.VideoClosed -= VideoAdClosed;
	}

	private void UpdateIAPContents()
	{
		GameController.Instance.Store.InventoryUpdated -= UpdateIAPContents;
		StartCoroutine(RefreshContents());
	}

	private IEnumerator RefreshContents()
	{
		ClearGrid();
		yield return null;
		InitGrid();
	}

	private void ClearGrid()
	{
		foreach (GameObject listItem in m_listItems)
		{
			Object.Destroy(listItem);
		}
		m_listItems.Clear();
	}

	private void OnItemClicked(CurrencyItem item)
	{
		GetComponent<Collider>().enabled = true;
		if (item.Type == CurrencyItem.PackType.GetJar)
		{
			return;
		}
		if (item.Type == CurrencyItem.PackType.VideoAds)
		{
			if (VideoAds.Instance != null)
			{
				VideoAds.Instance.VideoClosed += VideoAdClosed;
			}
			VideoAds.ShowVideo();
		}
		else
		{
			StoreFront store = GameController.Instance.Store;
			store.PurchaseCompleted += PurchaseDone;
			store.PurchaseFailed += PurchaseFailed;
			store.BuyCurrencyItem(item);
		}
	}

	private void PurchaseDone(CurrencyItem item)
	{
		GetComponent<Collider>().enabled = false;
		GetComponent<NavigateFromTo>().NavigateBack();
		AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/buy_gold2"));
		GameController.Instance.Store.PurchaseCompleted -= PurchaseDone;
		GameController.Instance.Store.PurchaseFailed -= PurchaseFailed;
		Tracking.BusinessEvent(Tracking.Events.IAPPurchase, item.ProductId, item.CurrencyCode, item.RealCurrencyAmount);
	}

	private void PurchaseFailed(CurrencyItem item)
	{
		GetComponent<Collider>().enabled = false;
		GetComponent<NavigateFromTo>().NavigateBack();
		GameController.Instance.Store.PurchaseCompleted -= PurchaseDone;
		GameController.Instance.Store.PurchaseFailed -= PurchaseFailed;
		Debug.LogWarning("Purchase of (" + item.ProductId + ") failed!");
		Tracking.DesignEvent(Tracking.Events.IAPPurchaseFailed, item.ProductId);
	}

	private IEnumerator PollStoreInventory()
	{
		StoreFront store = GameController.Instance.Store;
		while (store.CurrencyItems().Find((CurrencyItem x) => x.Type == CurrencyItem.PackType.Small) == null)
		{
			yield return new WaitForSeconds(20f);
			store.UpdateInventory();
		}
	}

	private void VideoAdClosed()
	{
		VideoAds.Instance.VideoClosed -= VideoAdClosed;
		GetComponent<Collider>().enabled = false;
		GetComponent<NavigateFromTo>().NavigateBack();
	}
}
