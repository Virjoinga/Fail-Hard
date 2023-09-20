namespace Game
{
	public class Notification
	{
		public class LevelUpNotification
		{
			public int Reward { get; set; }

			public int Stars { get; set; }

			public LevelUpNotification(int reward, int stars)
			{
				Reward = reward;
				Stars = stars;
			}
		}

		public enum Types
		{
			MechanicFinished = 0,
			MechanicFixingVehicle = 1,
			MechanicVehicleFixed = 2,
			MechanicBusy = 3,
			MechanicNotEnoughMoney = 4,
			MechanicItemLocked = 5,
			MechanicItemUnlocked = 6,
			TutorialThrottle = 7,
			TutorialJump = 8,
			TutorialJumpOff = 9,
			TutorialSpinForward = 10,
			TutorialUpgradeAcceleration = 11,
			HighlightThrottle = 12,
			HighlightBrake = 13,
			HighlightJump = 14,
			HighlightAcceleration = 15,
			HideTutorials = 16,
			LevelUnlocked = 17,
			StageCompleted = 18,
			TutorialPurchaseFirstVehicle = 19,
			HighlightVehiclePurchase = 20,
			TutorialUpgradeSpin = 21,
			TutorialUpgradeTopSpeed = 22,
			HighlightSpin = 23,
			OldClient = 24,
			HighlightTopSpeed = 25,
			LevelUp = 26,
			GetJarCoinsGet = 27,
			PrepareForStuntOver = 28,
			SupersonicAdWatched = 29,
			BundleCompletedAward = 30
		}

		private Types m_type;

		private object m_data;

		public Types Type
		{
			get
			{
				return m_type;
			}
		}

		public object Data
		{
			get
			{
				return m_data;
			}
		}

		public Notification(Types type, object data)
		{
			m_type = type;
			m_data = data;
		}
	}
}
