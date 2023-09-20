using System.Collections.Generic;

namespace Game
{
	public interface IStorage
	{
		int SavedAt { get; }

		void Shutdown();

		void Restore();

		CharacterInfo LoadCharacterInfo();

		CharacterParameters LoadCharacterParameters();

		void UpdateCharacterInfo(CharacterInfo info);

		LevelParameters LoadLevelParameters(string levelid);

		IAPParameters LoadIAPParameters();

		void LoadLevelInfo(string levelid, IMessageConsumer consumer);

		LevelInfo LoadLevelInfo(string levelid);

		List<VehiclePartParameters> LoadVehiclePartParameters();

		void UpdateVehiclePartParameters(List<VehiclePartParameters> parameters);

		VehiclePartInfo LoadVehiclePartInfo(string id);

		void UpdateVehiclePartInfo(VehiclePartInfo info, string id);

		void UpdateLevelInfo(LevelInfo stats, string levelid);

		List<VehicleParameters> LoadVehicleParameters();

		List<CutsceneParameters> LoadCutsceneParameters();

		VehicleInfo LoadVehicleInfo(string id);

		void UpdateVehicleInfo(VehicleInfo info, string id);

		List<BundleParameters> LoadBundleParameters();

		void UpdateBundleParameters(List<BundleParameters> parameters);

		BundleInfo LoadBundleInfo(string id);

		void UpdateBundleInfo(BundleInfo info, string id);

		TransactionHistory LoadTransactionHistory();

		void UpdateTransactionHistory(TransactionHistory history);

		Counter LoadCounter(string id);

		void UpdateCounter(string id);

		ScoringParameters LoadScoringParameters();
	}
}
