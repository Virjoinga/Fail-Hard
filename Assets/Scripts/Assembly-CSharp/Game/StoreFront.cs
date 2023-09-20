using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game
{
	public class StoreFront
	{
		public delegate void Purchase(CurrencyItem item);

		public delegate void Inventory();

		private const string SMALL_CURRENCY_PACK_ID = "com.fingersoft.failhard.small_gold_pack";

		private const string MEDIUM_CURRENCY_PACK_ID = "com.fingersoft.failhard.medium_gold_pack";

		private const string LARGE_CURRENCY_PACK_ID = "com.fingersoft.failhard.large_gold_pack";

		private const string EXTRA_LARGE_CURRENCY_PACK_ID = "com.fingersoft.failhard.extra_large_gold_pack";

		private const string HUGE_CURRENCY_PACK_ID = "com.fingersoft.failhard.huge_gold_pack";

		private const string ENORMOUS_CURRENCY_PACK_ID = "com.fingersoft.failhard.enormous_gold_pack";

		public static readonly CurrencyType Coins = new CurrencyType(1, "cr");

		public static readonly CurrencyType Diamonds = new CurrencyType(2, "dmnd");

		private List<CurrencyType> m_currencyTypes;

		private List<CurrencyItem> m_currencyItems;

		private List<ConvertToCurrencyItem> m_convertCurrencyItems;

		private List<StoreItem> m_storeItems;

		private TransactionManager m_transactionManager;

		private bool m_iapEnabled;

		private int m_consumingRetries;

		private bool m_initialized;

		private IAPParameters m_iapParameters;

		public IAPParameters IAPParameters
		{
			get
			{
				return m_iapParameters;
			}
		}

		public TransactionManager TransactionManager
		{
			get
			{
				if (!m_initialized)
				{
					Initialize();
				}
				return m_transactionManager;
			}
		}

		public bool IAPEnabled
		{
			get
			{
				return m_iapEnabled;
			}
		}

		public bool HasAnyPreviousPurchases
		{
			get
			{
				if (!m_initialized)
				{
					Initialize();
				}
				return m_transactionManager.HasAnyCompletedTransaction;
			}
		}

		private bool PlatformSupportsIAP
		{
			get
			{
				return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
			}
		}

		private string[] PackIds
		{
			get
			{
				return new string[6] { "com.fingersoft.failhard.small_gold_pack", "com.fingersoft.failhard.medium_gold_pack", "com.fingersoft.failhard.large_gold_pack", "com.fingersoft.failhard.extra_large_gold_pack", "com.fingersoft.failhard.huge_gold_pack", "com.fingersoft.failhard.enormous_gold_pack" };
			}
		}

		private Dictionary<string, CurrencyItem> CurrencyMap
		{
			get
			{
				Dictionary<string, CurrencyItem> dictionary = new Dictionary<string, CurrencyItem>();
				dictionary.Add("com.fingersoft.failhard.small_gold_pack", new CurrencyItem("com.fingersoft.failhard.small_gold_pack", "Small pack", string.Empty, CurrencyItem.PackType.Small, new Currency(m_iapParameters.MoneyPacks[0], Diamonds), "0.49DD"));
				dictionary.Add("com.fingersoft.failhard.medium_gold_pack", new CurrencyItem("com.fingersoft.failhard.medium_gold_pack", "Medium pack", string.Empty, CurrencyItem.PackType.Medium, new Currency(m_iapParameters.MoneyPacks[1], Diamonds), "0.99DD"));
				dictionary.Add("com.fingersoft.failhard.large_gold_pack", new CurrencyItem("com.fingersoft.failhard.large_gold_pack", "Large pack", string.Empty, CurrencyItem.PackType.Large, new Currency(m_iapParameters.MoneyPacks[2], Diamonds), "1.99DD"));
				dictionary.Add("com.fingersoft.failhard.extra_large_gold_pack", new CurrencyItem("com.fingersoft.failhard.extra_large_gold_pack", "Extra large pack", string.Empty, CurrencyItem.PackType.ExtraLarge, new Currency(m_iapParameters.MoneyPacks[3], Diamonds), "8.99DD"));
				dictionary.Add("com.fingersoft.failhard.huge_gold_pack", new CurrencyItem("com.fingersoft.failhard.huge_gold_pack", "Huge pack", string.Empty, CurrencyItem.PackType.Huge, new Currency(m_iapParameters.MoneyPacks[4], Diamonds), "15.99DD"));
				dictionary.Add("com.fingersoft.failhard.enormous_gold_pack", new CurrencyItem("com.fingersoft.failhard.enormous_gold_pack", "Enormous pack", string.Empty, CurrencyItem.PackType.Enormous, new Currency(m_iapParameters.MoneyPacks[5], Diamonds), "19.99DD"));
				return dictionary;
			}
		}

		[method: MethodImpl(32)]
		public event Purchase PurchaseCompleted;

		[method: MethodImpl(32)]
		public event Purchase PurchaseFailed;

		[method: MethodImpl(32)]
		public event Inventory InventoryUpdated = delegate
		{
		};

		public StoreFront()
		{
			m_currencyTypes = new List<CurrencyType>();
			m_storeItems = new List<StoreItem>();
			m_currencyItems = new List<CurrencyItem>();
			m_convertCurrencyItems = new List<ConvertToCurrencyItem>();
			m_transactionManager = new TransactionManager();
		}

		public void Initialize()
		{
			if (!m_initialized)
			{
				m_transactionManager.Initialize();
				m_iapParameters = Storage.Instance.LoadIAPParameters();
				Init();
				m_initialized = true;
			}
		}

		public void BuyCurrencyItem(CurrencyItem currencyItem)
		{
			if (!m_initialized)
			{
				Initialize();
			}
			if (!m_iapEnabled)
			{
				Debug.LogWarning("##! IAP not supported! Cannot make purchases!");
				return;
			}
			if (m_transactionManager.HasPendingTransaction)
			{
				Debug.Log("Cancelling pending purchase.");
				m_transactionManager.CancelPendingTransaction();
			}
			if (PlatformSupportsIAP)
			{
				string developerPayload = m_transactionManager.CreatePendingTransaction(currencyItem);
				GoogleIAB.purchaseProduct(currencyItem.ProductId, developerPayload);
			}
			else
			{
				this.PurchaseCompleted(currencyItem);
			}
		}

		public List<CurrencyItem> CurrencyItems()
		{
			if (!m_initialized)
			{
				Initialize();
			}
			return new List<CurrencyItem>(m_currencyItems);
		}

		public CurrencyItem FindProduct(string productId)
		{
			if (!m_initialized)
			{
				Initialize();
			}
			return m_currencyItems.Find((CurrencyItem x) => x.ProductId == productId);
		}

		public List<ConvertToCurrencyItem> ConvertCurrencyItems()
		{
			if (!m_initialized)
			{
				Initialize();
			}
			return new List<ConvertToCurrencyItem>(m_convertCurrencyItems);
		}

		public void UpdateInventory()
		{
			if (!m_initialized)
			{
				Initialize();
			}
			else if (m_iapEnabled)
			{
				RequestProductData();
			}
		}

		private void Init()
		{
			AddCurrencyType(Coins);
			AddCurrencyType(Diamonds);
			Func<bool> availabilityChecker = () => VideoAds.VideoCount() > 0;
			CurrencyItem currencyItem = new CurrencyItem("videoads", "Video Ads", "Watch videos, get coins", CurrencyItem.PackType.VideoAds, new Currency(1, Coins), "FREE", availabilityChecker);
			currencyItem.NotAvailableDescription = "Check again later!";
			AddCurrencyItem(currencyItem);
			if (PlatformSupportsIAP)
			{
				GoogleIABManager.billingSupportedEvent += BillingSupported;
				GoogleIABManager.billingNotSupportedEvent += BillingNotSupported;
				GoogleIABManager.consumePurchaseSucceededEvent += PurchaseConsumed;
				GoogleIABManager.consumePurchaseFailedEvent += PurchaseConsumingFailed;
				GoogleIABManager.purchaseFailedEvent += PurchaseAborted;
				GoogleIABManager.purchaseSucceededEvent += PurchaseDone;
				GoogleIAB.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqAo9RwnByKiQx2YCXLAKrUTQSR1jpunPS6KcKqxiQnvuuwC5EDbUPnzNE2JUYljHcHm5gLzKtWmaOwxwPo2/KK4guFp6FxamhDQNGPJJiWhJu0azVcCjXZrSJxqlKeuTJMfq6VUHuVzB6pgohIZcVErxByKVbY2hPr2WFA3a7efYf3YZH4cra73emETy5JvZm7X1tH7xKF2a3Hg0XVn6ecH8I0YrkOJELngSHhJgiQU2zKQfyvGZTdwBU1bd6+tAxTUBaO+hhofNwIRqZZj7YwmJE0W/qBpvDymKnCMKCcKX3+zOzme4HYlmuQvYxQEr/LuhfTo2gWHR6DikDE24FQIDAQAB");
			}
			else
			{
				m_iapEnabled = true;
				LoadDummyCurrencyItems();
				SortCurrencyItems();
			}
		}

		private void RequestProductData()
		{
			if (!m_iapEnabled)
			{
				this.InventoryUpdated();
			}
			else
			{
				GoogleIAB.queryInventory(PackIds);
			}
		}

		private void AddCurrencyType(CurrencyType type)
		{
			if (m_currencyTypes.Contains(type))
			{
				Debug.LogWarning("##! Tried to same currency type again: " + type.ToString());
			}
			else
			{
				m_currencyTypes.Add(type);
			}
		}

		private void AddStoreItem(StoreItem item)
		{
			m_storeItems.Add(item);
		}

		private void AddCurrencyItem(CurrencyItem item)
		{
			int num = m_currencyItems.FindIndex((CurrencyItem x) => x.ProductId == item.ProductId);
			if (num < 0)
			{
				m_currencyItems.Add(item);
				return;
			}
			m_currencyItems.RemoveAt(num);
			m_currencyItems.Add(item);
		}

		private void BillingSupported()
		{
			m_iapEnabled = true;
			GoogleIABManager.queryInventoryFailedEvent += InventoryNotAvailable;
			GoogleIABManager.queryInventorySucceededEvent += InventoryAvailable;
			RequestProductData();
		}

		private void BillingNotSupported(string message)
		{
			Debug.LogWarning("##! Billing not supported: " + message);
			m_iapEnabled = false;
		}

		private void InventoryAvailable(List<GooglePurchase> purchases, List<GoogleSkuInfo> skuInfos)
		{
			if (purchases.Count > 0)
			{
				ConsumePurchases(purchases);
			}
			Dictionary<string, CurrencyItem> currencyMap = CurrencyMap;
			Func<string, float> findFloat = delegate(string value)
			{
				Regex regex = new Regex("[0-9]+[\\.,\\,][0-9]+");
				Match match = regex.Match(value);
				float result = 0f;
				float.TryParse(match.Value, out result);
				return result;
			};
			skuInfos.Sort(delegate(GoogleSkuInfo x, GoogleSkuInfo y)
			{
				float num = findFloat(x.price);
				float value2 = findFloat(y.price);
				return num.CompareTo(value2);
			});
			foreach (GoogleSkuInfo skuInfo in skuInfos)
			{
				if (FindProduct(skuInfo.productId) == null)
				{
					CurrencyItem currencyItem = currencyMap[skuInfo.productId];
					currencyItem.Title = skuInfo.title;
					currencyItem.Description = skuInfo.description;
					currencyItem.FormattedPrice = skuInfo.price + skuInfo.currencyCode;
					currencyItem.CurrencyCode = skuInfo.currencyCode;
					currencyItem.RealCurrencyAmount = skuInfo.hundredParts;
					AddCurrencyItem(currencyItem);
				}
			}
			SortCurrencyItems();
			this.InventoryUpdated();
		}

		private void InventoryNotAvailable(string message)
		{
			Debug.LogWarning("##! Product inventory not available! " + message);
			m_iapEnabled = false;
			this.InventoryUpdated();
		}

		private void PurchaseDone(GooglePurchase purchase)
		{
			if (purchase.purchaseState != 0)
			{
				Debug.LogWarning("Purchase state other than purchased: " + purchase.purchaseState);
				m_transactionManager.CancelPendingTransaction();
			}
			else
			{
				ConsumePurchases(new List<GooglePurchase> { purchase });
			}
		}

		private void PurchaseAborted(string message)
		{
			Debug.LogWarning("##!??Failed: " + message);
			TransactionManager.Transaction transaction = m_transactionManager.CancelPendingTransaction();
			if (this.PurchaseFailed != null)
			{
				this.PurchaseFailed(transaction.Item);
			}
		}

		private void PurchaseConsumed(GooglePurchase purchase)
		{
			if (m_transactionManager.HasPendingTransaction)
			{
				TransactionManager.Transaction pendingTransaction = m_transactionManager.PendingTransaction;
				m_transactionManager.CompleteTransaction(pendingTransaction);
			}
			else
			{
				Debug.LogWarning("No pending transaction! : " + purchase.productId + "::" + purchase.developerPayload);
			}
			if (this.PurchaseCompleted != null)
			{
				CurrencyItem currencyItem = FindProduct(purchase.productId);
				if (currencyItem != null)
				{
					this.PurchaseCompleted(currencyItem);
				}
				else
				{
					Debug.LogWarning("No item info in transaction or invalid purchase product Id!");
				}
			}
			else
			{
				Debug.LogWarning("##! Not callback to call! Purchase not registered in app anyway!");
			}
		}

		private void PurchaseConsumingFailed(string message)
		{
			Debug.LogWarning("##! Purchase consuming failed: " + message);
			if (m_consumingRetries < 5)
			{
				GoogleIAB.queryInventory(PackIds);
				m_consumingRetries++;
			}
			else
			{
				m_transactionManager.CancelPendingTransaction();
			}
		}

		private void ConsumePurchases(List<GooglePurchase> purchases)
		{
			List<string> list = new List<string>(purchases.Count);
			foreach (GooglePurchase purchase in purchases)
			{
				list.Add(purchase.productId);
			}
			GoogleIAB.consumeProducts(list.ToArray());
		}

		private void SortCurrencyItems()
		{
			m_currencyItems.Sort((CurrencyItem x, CurrencyItem y) => x.GameCurrency.CompareTo(y.GameCurrency));
		}

		public void LoadDummyCurrencyItems()
		{
			List<CurrencyItem> list = new List<CurrencyItem>(CurrencyMap.Values);
			foreach (CurrencyItem item in list)
			{
				AddCurrencyItem(item);
			}
		}
	}
}
