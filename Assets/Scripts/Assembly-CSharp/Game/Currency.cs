using System;
using Game.Util;

namespace Game
{
	public class Currency : IComparable
	{
		private readonly CurrencyType m_type;

		private int m_amount;

		public CurrencyType Type
		{
			get
			{
				return m_type;
			}
		}

		public int Amount
		{
			get
			{
				return m_amount;
			}
		}

		public Currency(int amount, CurrencyType type)
		{
			m_type = type;
			m_amount = amount;
		}

		public int CompareTo(object obj)
		{
			Currency currency = obj as Currency;
			if (currency == null)
			{
				Logger.Error("Tried to compare to null currency!");
				return -1;
			}
			return Amount - currency.Amount;
		}
	}
}
