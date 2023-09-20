using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
	public class Vehicle
	{
		public delegate void OnStateChanged(Vehicle vehicle);

		private VehicleParameters m_vehicleParams;

		private VehicleInfo m_vehicleInfo;

		private VehicleInfo m_infoChanges;

		private float m_currentTopSpeed;

		private float m_currentAcceleration;

		private float topSpeedFactor = 1f;

		private float accelerationFactor = 1f;

		private float a;

		private float b;

		private float c;

		private List<VehiclePart> upgrades;

		private List<VehiclePart> gadgets;

		private List<VehiclePart> m_equippedGadgets;

		public string Id
		{
			get
			{
				return m_vehicleParams.Id;
			}
		}

		public string Name
		{
			get
			{
				return m_vehicleParams.Name;
			}
			private set
			{
				m_vehicleParams.Name = value;
			}
		}

		public string Description { get; set; }

		public string ResourceName
		{
			get
			{
				return m_vehicleParams.ResourceName;
			}
			private set
			{
				m_vehicleParams.ResourceName = value;
			}
		}

		public int MaxCondition
		{
			get
			{
				return m_vehicleParams.MaxCondition;
			}
			set
			{
				m_vehicleParams.MaxCondition = value;
			}
		}

		public List<GameEvent> Preconditions { get; set; }

		public Price PriceTag
		{
			get
			{
				return m_vehicleParams.PriceTag;
			}
			private set
			{
				m_vehicleParams.PriceTag = value;
			}
		}

		public int CurrentCondition
		{
			get
			{
				return m_vehicleInfo.CurrentCondition;
			}
			set
			{
				if (m_vehicleParams.MaxCondition > 0)
				{
					if (value <= 0 && m_vehicleInfo.CurrentCondition > 0)
					{
						m_vehicleInfo.CurrentCondition = Mathf.Clamp(value, 0, m_vehicleParams.MaxCondition);
					}
					m_vehicleInfo.CurrentCondition = Mathf.Clamp(value, 0, m_vehicleParams.MaxCondition);
				}
			}
		}

		public ItemState State
		{
			get
			{
				return m_vehicleInfo.State;
			}
			set
			{
				m_vehicleInfo.State = value;
				m_infoChanges.State = value;
				if (this.StateChanged != null)
				{
					this.StateChanged(this);
				}
				UpdateInfo();
			}
		}

		public float CurrentMaxSpinVelocity { get; private set; }

		public float CurrentMaxLift { get; private set; }

		public float JumpOffForceAbsorb
		{
			get
			{
				return m_vehicleParams.DefaultJumpOffForceAbsorb;
			}
			private set
			{
				m_vehicleParams.DefaultJumpOffForceAbsorb = value;
			}
		}

		public float MaxJumpOffForce
		{
			get
			{
				return m_vehicleParams.DefaultJumpOffForce;
			}
			private set
			{
				m_vehicleParams.DefaultJumpOffForce = value;
			}
		}

		public float MaxJumpingForce
		{
			get
			{
				return m_vehicleParams.DefaultJumpingForce;
			}
			private set
			{
				m_vehicleParams.DefaultJumpingForce = value;
			}
		}

		public float UltimateMaxSpeed { get; private set; }

		public float BrakingFactor { get; private set; }

		public float MaxBrakingForce { get; private set; }

		public List<string> UpgradeIds
		{
			get
			{
				return m_vehicleInfo.UpgradeIds;
			}
		}

		public List<string> GadgetIds
		{
			get
			{
				return m_vehicleInfo.GadgetIds;
			}
		}

		public List<string> EquippedGadgetIds
		{
			get
			{
				return m_vehicleInfo.EquippedGadgetIds;
			}
		}

		[method: MethodImpl(32)]
		public event OnStateChanged StateChanged;

		public Vehicle(VehicleParameters p, VehicleInfo i)
		{
			m_vehicleParams = p;
			switch (m_vehicleParams.Id)
			{
			case "PV50":
				Description = "Good beginner moped with potential for great stunts.";
				break;
			case "PappaT":
				Description = "Slow, heavy and stable.";
				break;
			case "VESPA":
				Description = "Lightweight and agile. Spins great.";
				break;
			case "DirtBike":
				Description = "High performance motocross bike.";
				break;
			case "Stiga":
				Description = "Flies like an eagle, slides like a duck.";
				break;
			case "DownhillCar":
				Description = "Heavy weight four wheeler. With reverse gear!";
				break;
			case "Monster":
				Description = "Powerful 4WD all terrain buggy.";
				break;
			}
			if (i == null)
			{
				m_vehicleInfo = new VehicleInfo();
			}
			else
			{
				m_vehicleInfo = i;
			}
			m_infoChanges = new VehicleInfo();
			if (PriceTag.Amount == 0)
			{
				m_vehicleInfo.State = ItemState.Purchased;
			}
			m_currentTopSpeed = m_vehicleParams.DefaultTopSpeed;
			m_currentAcceleration = m_vehicleParams.DefaultAcceleration;
			CurrentMaxSpinVelocity = m_vehicleParams.DefaultSpinVelocity;
			CurrentMaxLift = 10f;
			UltimateMaxSpeed = 60f;
			BrakingFactor = 10f;
			MaxBrakingForce = 10f;
			upgrades = new List<VehiclePart>();
			gadgets = new List<VehiclePart>();
			m_equippedGadgets = new List<VehiclePart>();
			CalculateCoefficients();
		}

		public bool InDrivingCondition()
		{
			return m_vehicleParams.MaxCondition < 0 || m_vehicleInfo.CurrentCondition > 0;
		}

		public float GetTopSpeed()
		{
			return m_currentTopSpeed;
		}

		public float GetTopSpeedFactor()
		{
			return topSpeedFactor;
		}

		public float GetAccelerationFactor()
		{
			return accelerationFactor;
		}

		public float GetMaxAcceleration(float currentVelocity)
		{
			currentVelocity *= 3.6f;
			if (currentVelocity > m_currentTopSpeed)
			{
				currentVelocity = m_currentTopSpeed;
			}
			float num = a + b * currentVelocity + 0.5f * c * currentVelocity * currentVelocity;
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}

		public void SetMaxAcceleration(float value)
		{
			m_currentAcceleration = value;
			CalculateCoefficients();
		}

		public void SetTopSpeed(float value)
		{
			m_currentTopSpeed = value;
			CalculateCoefficients();
		}

		private void CalculateCoefficients()
		{
			a = accelerationFactor * m_currentAcceleration;
			b = 0f;
			c = -2f * accelerationFactor * m_currentAcceleration / (topSpeedFactor * m_currentTopSpeed * topSpeedFactor * m_currentTopSpeed);
		}

		public void AddUpgrade(VehiclePart item)
		{
			switch (item.ItemType)
			{
			case VehiclePartType.VehicleGadget:
			case VehiclePartType.PlayerGadgetHead:
			case VehiclePartType.PlayerGadgetBack:
				AddGadget(item);
				return;
			}
			if (item.MaxLevel > 0)
			{
				UpgradeWithMultiLevelPart(item);
			}
			else if (!upgrades.Contains(item))
			{
				upgrades.Add(item);
				m_vehicleInfo.AddUpgradeId(item.Id);
				m_infoChanges.UpgradeIds = m_vehicleInfo.UpgradeIds;
				UpdateInfo();
				m_currentTopSpeed += item.TopSpeedDelta;
				m_currentAcceleration += item.AccelerationDelta;
				CurrentMaxSpinVelocity += item.SpinDelta;
				CalculateCoefficients();
			}
		}

		private void UpgradeWithMultiLevelPart(VehiclePart item)
		{
			item.CurrentLevel++;
			ApplyMultiLevelUpgrade(item);
		}

		private void ApplyMultiLevelUpgrade(VehiclePart item)
		{
			if (item != null && item.MaxLevel != 0)
			{
				switch (item.UpgradeType)
				{
				case VehicleUpgradeType.VehicleUpgradeAcceleration:
					m_currentAcceleration = m_vehicleParams.DefaultAcceleration + item.UpgradeDeltas[item.CurrentLevel];
					break;
				case VehicleUpgradeType.VehicleUpgradeTopSpeed:
					m_currentTopSpeed = m_vehicleParams.DefaultTopSpeed + item.UpgradeDeltas[item.CurrentLevel];
					break;
				case VehicleUpgradeType.VehicleUpgradeSpin:
					CurrentMaxSpinVelocity = m_vehicleParams.DefaultSpinVelocity + item.UpgradeDeltas[item.CurrentLevel];
					break;
				case VehicleUpgradeType.VehicleUpgradeLift:
					CurrentMaxLift = 10f + item.UpgradeDeltas[item.CurrentLevel];
					break;
				}
				CalculateCoefficients();
			}
		}

		public void RestoreUpgradesAndGadgets(ItemListModel partsModel)
		{
			ApplyMultiLevelUpgrade(partsModel.GetVehicleUpgrade(this, VehicleUpgradeType.VehicleUpgradeAcceleration));
			ApplyMultiLevelUpgrade(partsModel.GetVehicleUpgrade(this, VehicleUpgradeType.VehicleUpgradeTopSpeed));
			ApplyMultiLevelUpgrade(partsModel.GetVehicleUpgrade(this, VehicleUpgradeType.VehicleUpgradeSpin));
			foreach (string upgradeId in UpgradeIds)
			{
				VehiclePart item = partsModel.GetItem(upgradeId);
				if (item != null)
				{
					upgrades.Add(item);
					m_currentTopSpeed += item.TopSpeedDelta;
					m_currentAcceleration += item.AccelerationDelta;
				}
				else
				{
					Debug.LogWarning("UPGRADE ITEM ID NOT FOUND " + upgradeId);
				}
			}
			CalculateCoefficients();
			foreach (string gadgetId in GadgetIds)
			{
				VehiclePart item2 = partsModel.GetItem(gadgetId);
				if (item2 != null)
				{
					gadgets.Add(item2);
				}
				else
				{
					Debug.LogWarning("GADGET ID NOT FOUND " + gadgetId);
				}
			}
			foreach (string equippedGadgetId in EquippedGadgetIds)
			{
				VehiclePart item3 = partsModel.GetItem(equippedGadgetId);
				if (item3 != null)
				{
					m_equippedGadgets.Add(item3);
				}
				else
				{
					Debug.LogWarning("EQUIPPED GADGET ID NOT FOUND " + equippedGadgetId);
				}
			}
		}

		public int GetVehicleLevel()
		{
			return upgrades.Count + 1;
		}

		private void AddGadget(VehiclePart item)
		{
			if (!gadgets.Contains(item))
			{
				gadgets.Add(item);
				m_vehicleInfo.AddGadgetId(item.Id);
				m_infoChanges.GadgetIds = m_vehicleInfo.GadgetIds;
				UpdateInfo();
			}
		}

		public List<VehiclePart> GetGadgets()
		{
			return gadgets;
		}

		public void GadgetEquipped(VehiclePart item, bool isEquipped)
		{
			if (!gadgets.Contains(item))
			{
				Debug.LogError("Equipping something that is not purchased.");
			}
			else if (m_equippedGadgets.Contains(item) && !isEquipped)
			{
				m_vehicleInfo.RemoveEquippedGadgetId(item.Id);
				m_equippedGadgets.Remove(item);
				m_infoChanges.EquippedGadgetIds = m_vehicleInfo.EquippedGadgetIds;
				UpdateInfo();
			}
			else if (!m_equippedGadgets.Contains(item) && isEquipped)
			{
				m_equippedGadgets.Add(item);
				m_vehicleInfo.AddEquippedGadgetId(item.Id);
				m_infoChanges.EquippedGadgetIds = m_vehicleInfo.EquippedGadgetIds;
				UpdateInfo();
			}
		}

		public List<VehiclePart> GetEquippedGadgets()
		{
			return m_equippedGadgets;
		}

		public List<VehiclePart> GetUpgrades()
		{
			return upgrades;
		}

		public List<VehiclePart> GetUpgrades(int partSet)
		{
			List<VehiclePart> list = new List<VehiclePart>();
			foreach (VehiclePart upgrade in upgrades)
			{
				if (upgrade.SetIndex == partSet)
				{
					list.Add(upgrade);
				}
			}
			return list;
		}

		public List<VehiclePart> GetGadgets(VehiclePartType itemType)
		{
			List<VehiclePart> list = new List<VehiclePart>();
			foreach (VehiclePart gadget in gadgets)
			{
				if (gadget.ItemType == itemType)
				{
					list.Add(gadget);
				}
			}
			return list;
		}

		private void UpdateInfo()
		{
			LocalStorage.Instance.UpdateVehicleInfo(m_vehicleInfo, string.Empty + Id);
		}
	}
}
