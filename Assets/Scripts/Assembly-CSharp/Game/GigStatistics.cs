namespace Game
{
	public class GigStatistics
	{
		public int StuntCount { get; set; }

		public int TotalGigCoins { get; set; }

		public int HoleInOneBonus { get; set; }

		public int TotalCollectedTargetCount { get; set; }

		public int TargetsForMultiplier { get; set; }

		public int StarsAtGigStart { get; set; }

		public int StarsDuringGig { get; set; }

		public int TotalComboBonus { get; set; }

		public int MultiTargetBonus { get; set; }

		public int BigAirTime { get; set; }

		public int BigAirBonus { get; set; }

		public int FlipBonus { get; set; }

		public int CollateralDamageBonus { get; set; }

		public float Multiplier { get; set; }

		public float CrowdMeterValue { get; set; }

		public GigStatistics()
		{
			Multiplier = 1f;
			CrowdMeterValue = 1f;
		}

		public int CoinsWithoutBonus()
		{
			int totalGigCoins = TotalGigCoins;
			totalGigCoins -= BigAirBonus;
			totalGigCoins -= TotalComboBonus;
			totalGigCoins -= FlipBonus;
			totalGigCoins -= MultiTargetBonus;
			totalGigCoins -= CollateralDamageBonus;
			return totalGigCoins - HoleInOneBonus;
		}

		public void CollectedTarget()
		{
			TotalCollectedTargetCount++;
			TargetsForMultiplier++;
			Multiplier = GameController.Instance.Scoring.CumulativeTargetsMultiplier(TargetsForMultiplier);
			CrowdMeterValue = GameController.Instance.Scoring.CumulativeTargetsMultiplierProgress(TargetsForMultiplier);
		}

		public void ResetMultiplier()
		{
			TargetsForMultiplier = 0;
			Multiplier = GameController.Instance.Scoring.CumulativeTargetsMultiplier(0);
			CrowdMeterValue = GameController.Instance.Scoring.CumulativeTargetsMultiplierProgress(0);
		}
	}
}
