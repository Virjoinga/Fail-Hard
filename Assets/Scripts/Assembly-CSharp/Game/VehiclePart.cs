using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Progress;

namespace Game
{
	public class VehiclePart : IId, IItem
	{
		public delegate void OnStateChanged(UpgradeState state);

		private VehiclePartParameters Parameters;

		private VehiclePartInfo Info;

		public string IconTextureName { get; set; }

		public string EquipIconTextureName { get; set; }

		public ArrayList Capabilities { get; set; }

		public string Id
		{
			get
			{
				return Parameters.Id;
			}
		}

		public VehicleUpgradeType UpgradeType
		{
			get
			{
				return Parameters.UpgradeType;
			}
			private set
			{
				Parameters.UpgradeType = value;
			}
		}

		public VehiclePartType ItemType
		{
			get
			{
				return Parameters.ItemType;
			}
			private set
			{
				Parameters.ItemType = value;
			}
		}

		public List<GameEvent> Preconditions { get; set; }

		public int Index
		{
			get
			{
				return Parameters.Index;
			}
			private set
			{
				Parameters.Index = value;
			}
		}

		public int SetIndex
		{
			get
			{
				return Parameters.SetIndex;
			}
			private set
			{
				Parameters.SetIndex = value;
			}
		}

		public string Name
		{
			get
			{
				return Parameters.Name;
			}
			private set
			{
				Parameters.Name = value;
			}
		}

		public string Description
		{
			get
			{
				return Parameters.Description;
			}
			private set
			{
				Parameters.Description = value;
			}
		}

		public int Price
		{
			get
			{
				return Parameters.Price;
			}
			private set
			{
				Parameters.Price = value;
			}
		}

		public Price PriceTag
		{
			get
			{
				return Parameters.PriceTag;
			}
			private set
			{
				Parameters.PriceTag = value;
			}
		}

		public float UpgradeDelay
		{
			get
			{
				return Parameters.UpgradeDelay;
			}
			private set
			{
				Parameters.UpgradeDelay = value;
			}
		}

		public float TopSpeedDelta
		{
			get
			{
				return Parameters.TopSpeedDelta;
			}
			private set
			{
				Parameters.TopSpeedDelta = value;
			}
		}

		public float AccelerationDelta
		{
			get
			{
				return Parameters.AccelerationDelta;
			}
			private set
			{
				Parameters.AccelerationDelta = value;
			}
		}

		public float SpinDelta
		{
			get
			{
				return Parameters.SpinDelta;
			}
			private set
			{
				Parameters.SpinDelta = value;
			}
		}

		public string RequiresId
		{
			get
			{
				return Parameters.RequiresId;
			}
			private set
			{
				Parameters.RequiresId = value;
			}
		}

		public int UpgradeLevel
		{
			get
			{
				return Parameters.UpgradeLevel;
			}
			private set
			{
				Parameters.UpgradeLevel = value;
			}
		}

		public bool HasAction
		{
			get
			{
				return Parameters.HasAction;
			}
			private set
			{
				Parameters.HasAction = value;
			}
		}

		public bool HasProtection { get; private set; }

		public int MaxLevel
		{
			get
			{
				return Parameters.MaxLevel;
			}
			private set
			{
				Parameters.MaxLevel = value;
			}
		}

		public List<int> UpgradePrices
		{
			get
			{
				return Parameters.UpgradePrices;
			}
		}

		public List<float> UpgradeDeltas
		{
			get
			{
				return Parameters.UpgradeDeltas;
			}
		}

		public int CurrentLevel
		{
			get
			{
				return Info.CurrentLevel;
			}
			set
			{
				Info.CurrentLevel = value;
				InfoChanges.CurrentLevel = value;
				UpdateInfo();
			}
		}

		public bool IsNew { get; set; }

		public int MaxCondition
		{
			get
			{
				return Parameters.MaxCondition;
			}
			private set
			{
				Parameters.MaxCondition = value;
			}
		}

		public Price FixingPricePerUnit
		{
			get
			{
				return Parameters.FixingPricePerUnit;
			}
			private set
			{
				Parameters.FixingPricePerUnit = value;
			}
		}

		public Price CurrentFixingPrice
		{
			get
			{
				Price price = new Price();
				if (MaxCondition > 0)
				{
					price.Amount = FixingPricePerUnit.Amount * (MaxCondition - CurrentCondition);
					price.Currency = FixingPricePerUnit.Currency;
				}
				return price;
			}
			private set
			{
			}
		}

		public bool NeedsFixing
		{
			get
			{
				return MaxCondition > 0 && CurrentCondition <= 0;
			}
			private set
			{
			}
		}

		public int FixingDelayPerUnit
		{
			get
			{
				return Parameters.FixingDelayPerUnit;
			}
			private set
			{
				Parameters.FixingDelayPerUnit = value;
			}
		}

		public string GadgetPrefabResource { get; private set; }

		private VehiclePartInfo InfoChanges { get; set; }

		public int CurrentCondition
		{
			get
			{
				return Info.CurrentCondition;
			}
			set
			{
				Info.CurrentCondition = value;
				InfoChanges.CurrentCondition = value;
				UpdateInfo();
			}
		}

		public bool Bought
		{
			get
			{
				return Info.Bought;
			}
			set
			{
				Info.Bought = value;
				InfoChanges.Bought = value;
				UpdateInfo();
			}
		}

		public DateTime UpgradeStartTimeUTC
		{
			get
			{
				return Info.UpgradeStartTimeUTC;
			}
			set
			{
				Info.UpgradeStartTimeUTC = value;
				InfoChanges.UpgradeStartTimeUTC = value;
				UpdateInfo();
			}
		}

		public UpgradeState State
		{
			get
			{
				return Info.State;
			}
			set
			{
				Info.State = value;
				InfoChanges.State = value;
				UpdateInfo();
				if (this.StateChanged != null)
				{
					this.StateChanged(value);
				}
			}
		}

		[method: MethodImpl(32)]
		public event OnStateChanged StateChanged;

		public VehiclePart(VehiclePartParameters p, VehiclePartInfo i)
		{
			Parameters = p;
			Preconditions = new List<GameEvent>();
			MapResourcesBySetIndex(p.SetIndex);
			SetupResources(p);
			if (i == null)
			{
				Info = new VehiclePartInfo();
				Info.State = UpgradeState.Locked;
			}
			else
			{
				Info = i;
			}
			InfoChanges = new VehiclePartInfo();
		}

		public bool IsGadget()
		{
			if (ItemType == VehiclePartType.VehicleGadget || ItemType == VehiclePartType.PlayerGadgetHead || ItemType == VehiclePartType.PlayerGadgetBack)
			{
				return true;
			}
			return false;
		}

		public Price NextUpgradePrice()
		{
			Price price = null;
			if (MaxLevel > 0)
			{
				if (CurrentLevel < MaxLevel)
				{
					return new Price(UpgradePrices[CurrentLevel + 1]);
				}
				return new Price(1);
			}
			return PriceTag;
		}

		private void SetupResources(VehiclePartParameters vpp)
		{
			if (vpp.ItemType == VehiclePartType.VehicleUpgrade)
			{
				switch (vpp.UpgradeType)
				{
				case VehicleUpgradeType.VehicleUpgradeAcceleration:
					IconTextureName = "acceleration";
					break;
				case VehicleUpgradeType.VehicleUpgradeTopSpeed:
					IconTextureName = "topspeed";
					break;
				case VehicleUpgradeType.VehicleUpgradeSpin:
					IconTextureName = "item_spin";
					break;
				case VehicleUpgradeType.VehicleUpgradeLift:
					IconTextureName = "lift";
					break;
				}
			}
		}

		private void MapResourcesBySetIndex(int setIndex)
		{
			switch (setIndex)
			{
			case 0:
			case 10:
			case 20:
				IconTextureName = "item_acceleration";
				break;
			case 1:
			case 11:
			case 21:
				IconTextureName = "item_topspeed";
				break;
			case 2:
			case 12:
			case 22:
				IconTextureName = "item_spin";
				break;
			case 4:
				IconTextureName = "item_rocket";
				EquipIconTextureName = "item_rocket";
				GadgetPrefabResource = "RocketPack";
				break;
			case 5:
				IconTextureName = "item_wings";
				EquipIconTextureName = "item_wings";
				GadgetPrefabResource = "Wings";
				break;
			case 6:
				IconTextureName = "item_helmet";
				EquipIconTextureName = "item_helmet";
				GadgetPrefabResource = "Helmet";
				HasProtection = true;
				break;
			case 7:
			case 17:
			case 27:
				IconTextureName = "item_vehiclerocket";
				EquipIconTextureName = "item_vehiclerocket";
				GadgetPrefabResource = "VehicleRocket";
				break;
			case 8:
				IconTextureName = "item_propel";
				EquipIconTextureName = "item_propel";
				GadgetPrefabResource = "StigaPropel";
				break;
			case 71:
				IconTextureName = "item_springs";
				EquipIconTextureName = "item_springs";
				GadgetPrefabResource = "SuperSprings";
				break;
			case 75:
				IconTextureName = "item_springs";
				EquipIconTextureName = "item_springs";
				GadgetPrefabResource = "GadgetTrailer";
				break;
			case 101:
				IconTextureName = "item_cap";
				EquipIconTextureName = "item_cap";
				GadgetPrefabResource = "RedCap";
				break;
			case 102:
				IconTextureName = "item_hat";
				EquipIconTextureName = "item_hat";
				GadgetPrefabResource = "SirJohnny";
				break;
			case 103:
				IconTextureName = "item_winterhat";
				EquipIconTextureName = "item_winterhat";
				GadgetPrefabResource = "WinterHat";
				break;
			case 104:
				IconTextureName = "item_goggles";
				EquipIconTextureName = "item_goggles";
				GadgetPrefabResource = "Goggles";
				break;
			case 105:
				IconTextureName = "item_longhat";
				EquipIconTextureName = "item_longhat";
				GadgetPrefabResource = "WinterHatLong";
				break;
			case 106:
				IconTextureName = "item_xmashat";
				EquipIconTextureName = "item_xmashat";
				GadgetPrefabResource = "XmasHatLong";
				break;
			case 110:
				IconTextureName = "item_icehockey";
				EquipIconTextureName = "item_icehockey";
				GadgetPrefabResource = "IceHockeyGear_torso";
				HasProtection = true;
				break;
			case 111:
				IconTextureName = "item_icehockey_helmet";
				EquipIconTextureName = "item_icehockey_helmet";
				GadgetPrefabResource = "HelmetIceHockey";
				HasProtection = true;
				break;
			}
		}

		private void UpdateInfo()
		{
			LocalStorage.Instance.UpdateVehiclePartInfo(Info, string.Empty + Parameters.SetIndex + Parameters.Index);
			if (CloudStorageST.ServerAvailable)
			{
				CloudStorageST.Instance.UpdateVehiclePartInfo(InfoChanges, string.Empty + Parameters.SetIndex + Parameters.Index);
				InfoChanges = new VehiclePartInfo();
			}
		}

		public int GetRemainingTime()
		{
			return (int)((double)UpgradeDelay - (DateTime.UtcNow - UpgradeStartTimeUTC).TotalSeconds);
		}
	}
}
