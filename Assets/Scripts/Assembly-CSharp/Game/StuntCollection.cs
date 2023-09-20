using System.Collections.Generic;

namespace Game
{
	public class StuntCollection
	{
		private List<Stunt> m_stunts;

		public StuntCollection()
		{
			m_stunts = new List<Stunt>();
		}

		public void Add(Stunt newStunt)
		{
			m_stunts.Add(newStunt);
		}

		public float CalculateTotalScore()
		{
			return -1f;
		}
	}
}
