using UnityEngine;

namespace Game
{
	public class ScoringAlgorithm
	{
		private ScoringParameters m_params;

		public int BigAirFramesThreshold
		{
			get
			{
				return m_params.BigAirFramesThreshold;
			}
			private set
			{
				m_params.BigAirFramesThreshold = value;
			}
		}

		public int CoinsForBigAir
		{
			get
			{
				return m_params.CoinsForBigAir;
			}
			private set
			{
				m_params.CoinsForBigAir = value;
			}
		}

		public int CoinsForHoleInOne
		{
			get
			{
				return m_params.CoinsForHoleInOne;
			}
			private set
			{
				m_params.CoinsForHoleInOne = value;
			}
		}

		public int CoinsForLevelTarget
		{
			get
			{
				return m_params.CoinsForLevelTarget;
			}
			private set
			{
				m_params.CoinsForLevelTarget = value;
			}
		}

		public int CoinsForStayTarget
		{
			get
			{
				return m_params.CoinsForStayTarget;
			}
			private set
			{
				m_params.CoinsForStayTarget = value;
			}
		}

		public ScoringAlgorithm()
		{
			InitWithServerData();
		}

		private void InitWithServerData()
		{
			m_params = Storage.Instance.LoadScoringParameters();
		}

		public int LevelUpCriteria(int level)
		{
			float num;
			float num2;
			float num3;
			if (m_params.CareerLevelCriteriaCoeff != null && m_params.CareerLevelCriteriaCoeff.Count == 3)
			{
				num = m_params.CareerLevelCriteriaCoeff[0];
				num2 = m_params.CareerLevelCriteriaCoeff[1];
				num3 = m_params.CareerLevelCriteriaCoeff[2];
			}
			else
			{
				num = 2f;
				num2 = 10f;
				num3 = 0f;
			}
			return (int)(num * (float)level * (float)level + num2 * (float)level + num3);
		}

		public int CoinsForLevelUp(int level)
		{
			float num;
			float num2;
			float num3;
			if (m_params.CareerLevelCriteriaCoeff != null && m_params.CoinsForCareerLevelUpCoeff.Count == 3)
			{
				num = m_params.CoinsForCareerLevelUpCoeff[0];
				num2 = m_params.CoinsForCareerLevelUpCoeff[1];
				num3 = m_params.CoinsForCareerLevelUpCoeff[2];
			}
			else
			{
				num = 20f;
				num2 = 100f;
				num3 = 0f;
			}
			return (int)(num * (float)level * (float)level + num2 * (float)level + num3);
		}

		public int CareerLevelForStars(int starCount)
		{
			int num = 0;
			float num2;
			float num3;
			float num4;
			if (m_params.CareerLevelCriteriaCoeff != null && m_params.CareerLevelCriteriaCoeff.Count == 3)
			{
				num2 = m_params.CareerLevelCriteriaCoeff[0];
				num3 = m_params.CareerLevelCriteriaCoeff[1];
				num4 = m_params.CareerLevelCriteriaCoeff[2];
			}
			else
			{
				num2 = 2f;
				num3 = 10f;
				num4 = 0f;
			}
			if (num2 != 0f)
			{
				return (int)(0.5f * (0f - num3 + Mathf.Sqrt(num3 * num3 - 4f * num2 * (num4 - (float)starCount))) / num2);
			}
			if (num3 != 0f)
			{
				return (int)(((float)starCount - num4) / num3);
			}
			return (int)num4;
		}

		public int CoinsForStar(int amount)
		{
			if (m_params.CoinsForStar == null || m_params.CoinsForStar.Count == 0)
			{
				int[] array = new int[4];
				return array[amount];
			}
			if (amount >= m_params.CoinsForStar.Count)
			{
				amount = m_params.CoinsForStar.Count - 1;
			}
			return m_params.CoinsForStar[amount];
		}

		public int CoinsForRemainingStunts(int amount)
		{
			return m_params.CoinsForRemainingStunt * amount;
		}

		public int CoinsForFlip(int amount)
		{
			if (amount >= m_params.CoinsForFlip.Count)
			{
				amount = m_params.CoinsForFlip.Count - 1;
			}
			return m_params.CoinsForFlip[amount];
		}

		public int CoinsForCollateralDamage(float normalizedAmount)
		{
			return (int)(normalizedAmount * (float)m_params.CoinsForCollateralDamage);
		}

		public int ComboBonus(int comboCount)
		{
			if (comboCount >= m_params.ComboBonus.Count)
			{
				comboCount = m_params.ComboBonus.Count - 1;
			}
			return m_params.ComboBonus[comboCount];
		}

		public float CumulativeTargetsMultiplierProgress(int targets)
		{
			if (targets >= m_params.CumulativeTargetsMultiplierProgress.Count)
			{
				targets = m_params.CumulativeTargetsMultiplierProgress.Count - 1;
			}
			return m_params.CumulativeTargetsMultiplierProgress[targets];
		}

		public int CumulativeTargetsMultiplier(int targets)
		{
			return Mathf.FloorToInt(CumulativeTargetsMultiplierProgress(targets));
		}
	}
}
