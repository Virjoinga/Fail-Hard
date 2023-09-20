using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
	public class Mechanic
	{
		private class CurrentTask
		{
			public VehiclePart Item { get; set; }

			public Vehicle Vehicle { get; set; }

			public DateTime StartTime { get; set; }
		}

		public enum MechanicState
		{
			Idle = 0,
			Busy = 1
		}

		public delegate void OnStateChanged(MechanicState s);

		private CurrentTask currentTask;

		public MechanicState State { get; private set; }

		[method: MethodImpl(32)]
		public event OnStateChanged StateChanged;

		public Mechanic(Character character)
		{
			State = MechanicState.Idle;
			character.GameEventRegistered += OnGameEventRegistered;
			if (this.StateChanged != null)
			{
				this.StateChanged(State);
			}
		}

		public void RestoreVehicleStates()
		{
			GameController instance = GameController.Instance;
			foreach (Vehicle item in instance.VehicleModel.Model)
			{
				item.StateChanged += vehicle_StateChanged;
				item.RestoreUpgradesAndGadgets(instance.ItemModelPlaceholder);
			}
			CheckItemUnlocks(false);
		}

		private void vehicle_StateChanged(Vehicle vehicle)
		{
			if (vehicle.State != ItemState.Broken)
			{
			}
		}

		private void OnGameEventRegistered(GameEvent gameEvent)
		{
			CheckItemUnlocks(true);
		}

		private void CheckItemUnlocks(bool notify)
		{
			GameController instance = GameController.Instance;
			foreach (VehiclePart value in instance.ItemModelPlaceholder.GetAllParts().Values)
			{
				if (value.State == UpgradeState.Locked && instance.Character.ContainsEvents(value.Preconditions))
				{
					value.State = UpgradeState.Upgradeable;
					if (notify)
					{
						NotificationCentre.Send(Notification.Types.MechanicItemUnlocked, null);
					}
				}
			}
		}

		public void VIPUpgrade(VehiclePart item)
		{
			FinalizeUpgrade(item);
		}

		public bool InstantFixGadget(VehiclePart item)
		{
			if (!item.NeedsFixing)
			{
				return false;
			}
			if (State == MechanicState.Busy)
			{
				NotificationCentre.Send(Notification.Types.MechanicBusy, null);
				return false;
			}
			if (!GameController.Instance.Character.Afford(item.CurrentFixingPrice))
			{
				AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/aj_denied"));
				return false;
			}
			GameController.Instance.Character.Pay(item.CurrentFixingPrice);
			item.CurrentCondition = item.MaxCondition;
			item.State = UpgradeState.Installed;
			return true;
		}

		private void FinalizeUpgrade(VehiclePart item)
		{
			VehiclePartType itemType = item.ItemType;
			if (itemType == VehiclePartType.PlayerGadgetHead || itemType == VehiclePartType.PlayerGadgetBack)
			{
				GameController.Instance.Character.GadgetPurchased(item);
			}
			else
			{
				GameController.Instance.Character.CurrentVehicle.AddUpgrade(item);
				Vehicle currentVehicle = GameController.Instance.Character.CurrentVehicle;
				float topSpeed = currentVehicle.GetTopSpeed();
				float maxAcceleration = currentVehicle.GetMaxAcceleration(0f);
				Tracking.DesignEvent(Tracking.Events.VechicleState, topSpeed.ToString(), maxAcceleration.ToString());
			}
			AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/repair3"));
			item.State = UpgradeState.Installed;
			item.CurrentCondition = item.MaxCondition;
			GameController.Instance.Character.AddGameEvent(new GameEvent(GameEvent.GameEventType.ItemInstalled, item.Id));
		}

		public void InstallPart(VehiclePart item)
		{
			if (item.State == UpgradeState.ReadyToBeInstalled)
			{
				FinalizeUpgrade(item);
			}
		}

		public bool Upgrade(VehiclePart item)
		{
			if (State == MechanicState.Busy)
			{
				NotificationCentre.Send(Notification.Types.MechanicBusy, null);
				return false;
			}
			Price price = item.NextUpgradePrice();
			if (!GameController.Instance.Character.Afford(price))
			{
				AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/aj_denied"));
				return false;
			}
			GameController.Instance.Character.Pay(price);
			Tracking.DesignEvent(Tracking.Events.ItemPurchased, item.Name, price.Amount);
			FinalizeUpgrade(item);
			return true;
		}
	}
}
