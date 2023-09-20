using System;

namespace Game
{
	public class CurrencyItem
	{
		public enum PackType
		{
			Small = 0,
			Medium = 1,
			Large = 2,
			ExtraLarge = 3,
			Huge = 4,
			Enormous = 5,
			GetJar = 6,
			VideoAds = 7
		}

		private string m_formattedPrice;

		private Currency m_gameCurrency;

		private string m_currencyCode;

		private int m_realCurrencyAmount;

		private string m_description;

		private string m_notAvailableDescription;

		private string m_title;

		private PackType m_type;

		private string m_iapId;

		private Func<bool> m_availabilityChecker;

		public bool IsAvailable
		{
			get
			{
				if (m_availabilityChecker != null)
				{
					return m_availabilityChecker();
				}
				return true;
			}
		}

		public string Title
		{
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public string NotAvailableDescription
		{
			get
			{
				return m_notAvailableDescription;
			}
			set
			{
				m_notAvailableDescription = value;
			}
		}

		public Currency GameCurrency
		{
			get
			{
				return m_gameCurrency;
			}
		}

		public PackType Type
		{
			get
			{
				return m_type;
			}
		}

		public string FormattedPrice
		{
			get
			{
				return m_formattedPrice;
			}
			set
			{
				m_formattedPrice = value;
			}
		}

		public string CurrencyCode
		{
			get
			{
				return m_currencyCode;
			}
			set
			{
				m_currencyCode = value;
			}
		}

		public int RealCurrencyAmount
		{
			get
			{
				return m_realCurrencyAmount;
			}
			set
			{
				m_realCurrencyAmount = value;
			}
		}

		public string ProductId
		{
			get
			{
				return m_iapId;
			}
		}

		public CurrencyItem(string iapId, string title, string description, PackType type, Currency gameCurrency, string formattedPrice, Func<bool> availabilityChecker = null)
		{
			m_iapId = iapId;
			m_title = title;
			m_description = description;
			m_type = type;
			m_gameCurrency = gameCurrency;
			m_formattedPrice = formattedPrice;
			m_availabilityChecker = availabilityChecker;
		}
	}
}
