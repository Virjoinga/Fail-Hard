namespace Game
{
	public class CurrencyType
	{
		private readonly int m_type;

		private readonly string m_typeText;

		public CurrencyType(int type, string typeText)
		{
			m_type = type;
			m_typeText = typeText;
		}

		public override bool Equals(object obj)
		{
			CurrencyType currencyType = obj as CurrencyType;
			return this == currencyType;
		}

		public override int GetHashCode()
		{
			return m_type;
		}

		public override string ToString()
		{
			return m_typeText;
		}

		public static bool operator ==(CurrencyType x, CurrencyType y)
		{
			if (object.ReferenceEquals(x, null))
			{
				return false;
			}
			if (object.ReferenceEquals(y, null))
			{
				return false;
			}
			return x.m_type == y.m_type;
		}

		public static bool operator !=(CurrencyType x, CurrencyType y)
		{
			if (object.ReferenceEquals(x, null))
			{
				return true;
			}
			if (object.ReferenceEquals(y, null))
			{
				return true;
			}
			return x.m_type != y.m_type;
		}
	}
}
