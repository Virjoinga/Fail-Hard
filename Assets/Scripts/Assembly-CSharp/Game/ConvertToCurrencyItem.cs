namespace Game
{
	public class ConvertToCurrencyItem
	{
		public enum PackType
		{
			Small = 0,
			Medium = 1,
			Large = 2,
			Huge = 3
		}

		private Currency m_fromCurrency;

		private Currency m_toCurreny;

		private PackType m_type;

		public Currency FromCurrency
		{
			get
			{
				return m_fromCurrency;
			}
		}

		public Currency ToCurrency
		{
			get
			{
				return m_toCurreny;
			}
		}

		public PackType Type
		{
			get
			{
				return m_type;
			}
		}

		public ConvertToCurrencyItem(Currency fromCurrency, Currency toCurrency, PackType type)
		{
			m_toCurreny = toCurrency;
			m_fromCurrency = fromCurrency;
			m_type = type;
		}
	}
}
