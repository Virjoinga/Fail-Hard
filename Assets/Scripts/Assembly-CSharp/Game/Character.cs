using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
	public class Character
	{
		public delegate void OnGameEventRegistered(GameEvent gameEvent);

		public delegate void OnCurrentVehicleChanged(Vehicle vehicle);

		private CharacterInfo Info;

		private CharacterParameters Parameters;

		private CharacterInfo InfoChanges;

		private List<VehiclePart> m_purchasedGadgets;

		private List<VehiclePart> m_equippedGadgets;

		private Vehicle m_currentVehicle;

		public string CharacterResource
		{
			get
			{
				return Parameters.CharacterResource;
			}
			private set
			{
			}
		}

		public int Coins
		{
			get
			{
				return Info.Coins;
			}
			set
			{
				Info.Coins = value;
				InfoChanges.Coins = value;
				UpdateInfo();
			}
		}

		public int CareerStars
		{
			get
			{
				return Info.CareerStars;
			}
			set
			{
				Info.CareerStars = value;
				InfoChanges.CareerStars = value;
				UpdateInfo();
			}
		}

		public int CareerLevel { get; set; }

		public int Score
		{
			get
			{
				return Info.Score;
			}
			set
			{
				Info.Score = value;
				InfoChanges.Score = value;
				UpdateInfo();
			}
		}

		public List<string> PurchasedGadgetIds
		{
			get
			{
				return Info.PurchasedGadgetIds;
			}
		}

		public List<string> EquippedGadgetIds
		{
			get
			{
				return Info.EquippedGadgetIds;
			}
		}

		public Player Player { get; set; }

		public VehicleBase Vehicle { get; set; }

		public Vehicle CurrentVehicle
		{
			get
			{
				return m_currentVehicle;
			}
			set
			{
				if (value != m_currentVehicle)
				{
					m_currentVehicle = value;
					if (this.CurrentVehicleChanged != null)
					{
						this.CurrentVehicleChanged(value);
					}
					Info.CurrentVehicleId = m_currentVehicle.Id;
					InfoChanges.CurrentVehicleId = m_currentVehicle.Id;
					UpdateInfo();
				}
			}
		}

		public List<Mechanic> Mechanics { get; set; }

		public string Name { get; set; }

		[method: MethodImpl(32)]
		public event OnGameEventRegistered GameEventRegistered;

		[method: MethodImpl(32)]
		public event OnCurrentVehicleChanged CurrentVehicleChanged;

		public Character()
		{
			m_purchasedGadgets = new List<VehiclePart>();
			m_equippedGadgets = new List<VehiclePart>();
			Info = Storage.Instance.LoadCharacterInfo();
			InfoChanges = new CharacterInfo();
			Parameters = Storage.Instance.LoadCharacterParameters();
			CurrentVehicle = VehicleListModel.Instance.GetVehicle(Info.CurrentVehicleId);
			if (CurrentVehicle == null)
			{
				foreach (Vehicle item in VehicleListModel.Instance.Model)
				{
					if (item.State == ItemState.Purchased)
					{
						CurrentVehicle = item;
					}
				}
				if (Coins < Parameters.Coins)
				{
					Coins = Parameters.Coins;
				}
			}
			RestoreGadgetStatus();
			Mechanics = new List<Mechanic>();
			Mechanics.Add(new Mechanic(this));
		}

		public void AddGameEvent(GameEvent ge)
		{
			if (ge.EventType != GameEvent.GameEventType.VehicleBroken && ge.EventType != GameEvent.GameEventType.VehicleFixed)
			{
				Info.AddGameEvent(ge);
				InfoChanges.GameEvents = Info.GameEvents;
				UpdateInfo();
			}
			if (ge.EventType == GameEvent.GameEventType.VehiclePermanentlyFixed)
			{
			}
			if (this.GameEventRegistered != null)
			{
				this.GameEventRegistered(ge);
			}
		}

		public bool ContainsEvents(List<GameEvent> geList)
		{
			if (geList.Count == 0)
			{
				return true;
			}
			if (Info.GameEvents == null)
			{
				return false;
			}
			foreach (GameEvent ge in geList)
			{
				if (!Info.GameEvents.Contains(ge))
				{
					return false;
				}
			}
			return true;
		}

		public bool ContainsEvent(GameEvent ge)
		{
			if (ge == null)
			{
				return true;
			}
			if (Info.GameEvents == null)
			{
				return false;
			}
			if (Info.GameEvents.Contains(ge))
			{
				return true;
			}
			return false;
		}

		public bool IsCutsceneShown(CutsceneData cs)
		{
			return Info.ShownCutscenes.Contains(cs.Id);
		}

		public void CutsceneShown(CutsceneData cs)
		{
			if (!Info.ShownCutscenes.Contains(cs.Id) && cs.SingleShot)
			{
				Info.AddShownCutscene(cs.Id);
				InfoChanges.ShownCutscenes = Info.ShownCutscenes;
				UpdateInfo();
			}
		}

		public void GadgetPurchased(VehiclePart item)
		{
			if (!m_purchasedGadgets.Contains(item))
			{
				m_purchasedGadgets.Add(item);
				Info.AddPurchasedGadgetId(item.Id);
				InfoChanges.PurchasedGadgetIds = Info.PurchasedGadgetIds;
				UpdateInfo();
			}
		}

		public List<VehiclePart> GetPurchasedGadgets()
		{
			return m_purchasedGadgets;
		}

		public bool IsPurchased(VehiclePart vp)
		{
			bool result = false;
			if (vp.ItemType == VehiclePartType.VehicleGadget && CurrentVehicle != null && CurrentVehicle.GetGadgets().Contains(vp))
			{
				result = true;
			}
			else if ((vp.ItemType == VehiclePartType.PlayerGadgetBack || vp.ItemType == VehiclePartType.PlayerGadgetHead) && m_purchasedGadgets.Contains(vp))
			{
				result = true;
			}
			return result;
		}

		public bool IsEquipped(VehiclePart vp)
		{
			bool result = false;
			if (vp.ItemType == VehiclePartType.VehicleGadget && CurrentVehicle != null && CurrentVehicle.GetEquippedGadgets().Contains(vp))
			{
				result = true;
			}
			else if ((vp.ItemType == VehiclePartType.PlayerGadgetBack || vp.ItemType == VehiclePartType.PlayerGadgetHead) && m_equippedGadgets.Contains(vp))
			{
				result = true;
			}
			return result;
		}

		public void GadgetEquipped(VehiclePart item, bool isEquipped)
		{
			if (!m_purchasedGadgets.Contains(item))
			{
				Debug.LogWarning("Equipping something that is not purchased.");
			}
			if (m_equippedGadgets.Contains(item) && !isEquipped)
			{
				m_equippedGadgets.Remove(item);
				Info.RemoveEquippedGadgetId(item.Id);
				InfoChanges.EquippedGadgetIds = Info.EquippedGadgetIds;
				UpdateInfo();
			}
			else if (!m_equippedGadgets.Contains(item) && isEquipped)
			{
				m_equippedGadgets.Add(item);
				Info.AddEquippedGadgetId(item.Id);
				InfoChanges.EquippedGadgetIds = Info.EquippedGadgetIds;
				UpdateInfo();
			}
		}

		public List<VehiclePart> GetEquippedGadgets()
		{
			return m_equippedGadgets;
		}

		public bool Afford(Price price)
		{
			if (price.Currency == Price.CurrencyType.Coins)
			{
				return Coins >= price.Amount;
			}
			Debug.LogError("DEPRECATED: NO DIAMONDS IN THE GAME!");
			return false;
		}

		public bool Pay(Price price)
		{
			if (!Afford(price))
			{
				return false;
			}
			AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/aj_moneyspent"));
			if (price.Currency == Price.CurrencyType.Coins)
			{
				Coins -= price.Amount;
				return true;
			}
			Debug.LogError("DEPRECATED: NO DIAMONDS IN THE GAME!");
			return false;
		}

		private void UpdateInfo()
		{
			LocalStorage.Instance.UpdateCharacterInfo(Info);
			if (CloudStorageST.ServerAvailable)
			{
				InfoChanges.global_dirty = false;
				CloudStorageST.Instance.UpdateCharacterInfo(InfoChanges);
				InfoChanges = new CharacterInfo();
			}
		}

		private void RestoreGadgetStatus()
		{
			foreach (string purchasedGadgetId in PurchasedGadgetIds)
			{
				VehiclePart item = ItemListModel.Instance.GetItem(purchasedGadgetId);
				if (item != null)
				{
					m_purchasedGadgets.Add(item);
				}
				else
				{
					Debug.LogWarning("EQUIPPED PLAYER GADGET ID NOT FOUND " + purchasedGadgetId);
				}
			}
			foreach (string equippedGadgetId in EquippedGadgetIds)
			{
				VehiclePart item2 = ItemListModel.Instance.GetItem(equippedGadgetId);
				if (item2 != null)
				{
					m_equippedGadgets.Add(item2);
				}
				else
				{
					Debug.LogWarning("EQUIPPED PLAYER GADGET ID NOT FOUND " + equippedGadgetId);
				}
			}
		}

		public void ConnectStore(StoreFront store)
		{
			store.PurchaseCompleted += AddPurchase;
		}

		private void AddPurchase(CurrencyItem item)
		{
			Coins += item.GameCurrency.Amount;
		}
	}
}
