namespace Game
{
	public class StoreItem
	{
		private string m_type;

		private Currency m_value;

		private string m_title;

		private string m_description;

		public string Title
		{
			get
			{
				return m_title;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
		}

		public Currency Value
		{
			get
			{
				return m_value;
			}
		}

		public string Type
		{
			get
			{
				return m_type;
			}
		}

		public StoreItem(string type, string title, string description, Currency value)
		{
			m_type = type;
			m_title = title;
			m_description = description;
			m_value = value;
		}
	}
}
