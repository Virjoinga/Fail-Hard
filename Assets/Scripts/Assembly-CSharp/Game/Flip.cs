using UnityEngine;

namespace Game
{
	public class Flip
	{
		private int[] m_sectors = new int[4];

		public bool IsCw { get; set; }

		public int StartingSector { get; set; }

		public int CurrentSectorCount { get; set; }

		public bool Update(Transform vehicle)
		{
			int sector = GetSector(vehicle);
			m_sectors[sector] = 1;
			int num = FlipProgress();
			if (num == 4)
			{
				return true;
			}
			return false;
		}

		public int FlipProgress()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num += m_sectors[i];
			}
			return num;
		}

		public int GetSector(Transform vehicle)
		{
			Vector3 forward = vehicle.forward;
			if (forward.x > 0f && forward.y > 0f)
			{
				return 0;
			}
			if (forward.x > 0f && forward.y <= 0f)
			{
				return 1;
			}
			if (forward.x <= 0f && forward.y <= 0f)
			{
				return 2;
			}
			return 3;
		}

		public void Reset(bool isCw, Transform vehicle)
		{
			IsCw = isCw;
			StartingSector = GetSector(vehicle);
			for (int i = 0; i < 4; i++)
			{
				m_sectors[i] = 0;
			}
			m_sectors[StartingSector] = 1;
		}
	}
}
