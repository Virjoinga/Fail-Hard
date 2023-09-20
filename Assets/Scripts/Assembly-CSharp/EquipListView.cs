using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class EquipListView : MonoBehaviour
{
	public delegate void OnGadgetEquipped(VehiclePart gadget, bool isEquipped);

	public delegate void OnGetCoins();

	private List<GameObject> m_listItems;

	public VehiclePartType PartTypeFilter;

	public GameObject itemDelegatePrefab;

	public GameObject dialogPrefab;

	public Transform DialogParent;

	private GameObject dialog;

	private EquipListDelegate m_selectedItem;

	public UILabel TitleLabel;

	private bool m_started;

	public UIGrid Grid;

	[method: MethodImpl(32)]
	public event OnGadgetEquipped GadgetEquipped;

	[method: MethodImpl(32)]
	public event OnGetCoins GetCoins;

	private void Start()
	{
		m_started = true;
		DialogParent = NGUITools.FindInParents<UIRoot>(base.gameObject).GetComponentInChildren<UIAnchor>().transform;
	}

	public void SetData(List<VehiclePart> gadgets)
	{
		bool flag = true;
		if (gadgets[0].HasAction || gadgets[0].HasProtection)
		{
			flag = false;
		}
		switch (gadgets[0].ItemType)
		{
		case VehiclePartType.PlayerGadgetHead:
			if (flag)
			{
				TitleLabel.text = "Cool stuff";
			}
			else
			{
				TitleLabel.text = "Head gadgets";
			}
			break;
		case VehiclePartType.PlayerGadgetBack:
			if (flag)
			{
				TitleLabel.text = "Cool stuff";
			}
			else
			{
				TitleLabel.text = "Body gadgets";
			}
			break;
		case VehiclePartType.VehicleGadget:
			if (flag)
			{
				TitleLabel.text = "Cool stuff";
			}
			else
			{
				TitleLabel.text = "Vehicle gadgets";
			}
			break;
		}
		PartTypeFilter = gadgets[0].ItemType;
		InitModel(gadgets);
	}

	private void InitModel(List<VehiclePart> gadgets)
	{
		if (m_listItems == null)
		{
			m_listItems = new List<GameObject>();
		}
		DestroyAll();
		foreach (VehiclePart gadget in gadgets)
		{
			GameObject gameObject = NGUITools.AddChild(Grid.gameObject, itemDelegatePrefab);
			gameObject.GetComponent<EquipListDelegate>().SetData(gadget);
			gameObject.GetComponent<EquipListDelegate>().ItemSelected += OnItemSelected;
			m_listItems.Add(gameObject);
			bool isOn = GameController.Instance.Character.IsEquipped(gadget);
			gameObject.GetComponent<EquipListDelegate>().Highlight(isOn);
		}
		Grid.Reposition();
	}

	private void DestroyAll()
	{
		if (m_listItems == null || !m_started)
		{
			return;
		}
		foreach (GameObject listItem in m_listItems)
		{
			listItem.transform.parent = null;
			Object.Destroy(listItem);
		}
		m_listItems.Clear();
	}

	private void OnDisable()
	{
		DestroyAll();
	}

	public void OnItemSelected(EquipListDelegate item)
	{
		if (GameController.Instance.Character.IsPurchased(item.VehiclePart))
		{
			if (item.VehiclePart.NeedsFixing)
			{
				m_selectedItem = item;
				OpenPurchaseDialog(Language.Get("FIX_IT"), "The gadget needs some maintenance to get it back in business.", item.VehiclePart.CurrentFixingPrice);
			}
			else
			{
				HandleGadgetEquip(item);
			}
		}
		else if (item.VehiclePart.State != 0)
		{
			m_selectedItem = item;
			OpenPurchaseDialog("Purchase it?", item.VehiclePart.Description, item.VehiclePart.NextUpgradePrice());
		}
	}

	private void HandleGadgetEquip(EquipListDelegate item)
	{
		VehiclePart vehiclePart = item.VehiclePart;
		if (vehiclePart.ItemType == VehiclePartType.PlayerGadgetHead || vehiclePart.ItemType == VehiclePartType.PlayerGadgetBack)
		{
			List<VehiclePart> equippedGadgets = GameController.Instance.Character.GetEquippedGadgets();
			if (equippedGadgets.Contains(item.VehiclePart))
			{
				GameController.Instance.Character.GadgetEquipped(item.VehiclePart, false);
				item.Highlight(false);
				if (this.GadgetEquipped != null)
				{
					this.GadgetEquipped(item.VehiclePart, false);
				}
			}
			else
			{
				foreach (VehiclePart item2 in equippedGadgets)
				{
					if (item2.ItemType == item.VehiclePart.ItemType)
					{
						GameController.Instance.Character.GadgetEquipped(item2, false);
						if (this.GadgetEquipped != null)
						{
							this.GadgetEquipped(item2, false);
						}
						break;
					}
				}
				GameController.Instance.Character.GadgetEquipped(item.VehiclePart, true);
				item.Highlight(true);
				if (this.GadgetEquipped != null)
				{
					this.GadgetEquipped(item.VehiclePart, true);
				}
			}
		}
		else
		{
			List<VehiclePart> equippedGadgets2 = GameController.Instance.Character.CurrentVehicle.GetEquippedGadgets();
			if (equippedGadgets2.Contains(item.VehiclePart))
			{
				GameController.Instance.Character.CurrentVehicle.GadgetEquipped(item.VehiclePart, false);
				item.Highlight(false);
				if (this.GadgetEquipped != null)
				{
					this.GadgetEquipped(item.VehiclePart, false);
				}
			}
			else
			{
				foreach (VehiclePart item3 in equippedGadgets2)
				{
					if (item3.ItemType == item.VehiclePart.ItemType)
					{
						GameController.Instance.Character.CurrentVehicle.GadgetEquipped(item3, false);
						if (this.GadgetEquipped != null)
						{
							this.GadgetEquipped(item3, false);
						}
						break;
					}
				}
				GameController.Instance.Character.CurrentVehicle.GadgetEquipped(item.VehiclePart, true);
				item.Highlight(true);
				if (this.GadgetEquipped != null)
				{
					this.GadgetEquipped(item.VehiclePart, true);
				}
			}
		}
		foreach (GameObject listItem in m_listItems)
		{
			if (listItem != item.gameObject)
			{
				listItem.GetComponent<EquipListDelegate>().Highlight(false);
			}
		}
	}

	public void OpenPurchaseDialog(string title, string description, Price price)
	{
		dialog = Object.Instantiate(dialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		dialog.transform.parent = DialogParent;
		dialog.transform.localScale = Vector3.one;
		Dialog component = dialog.GetComponent<Dialog>();
		component.Title = title;
		component.Description = description;
		if (!GameController.Instance.Character.Afford(price))
		{
			component.Cost = "Get coins";
		}
		else
		{
			component.Cost = price.Amount.ToString();
		}
		component.PanelsParent = DialogParent;
		component.DialogClosed += OnPopupClosed;
	}

	private void OnPopupClosed(bool isYes)
	{
		if (isYes)
		{
			Price price = ((m_selectedItem.VehiclePart.State != UpgradeState.Upgradeable) ? m_selectedItem.VehiclePart.CurrentFixingPrice : m_selectedItem.VehiclePart.NextUpgradePrice());
			if (!GameController.Instance.Character.Afford(price))
			{
				if (this.GetCoins != null)
				{
					this.GetCoins();
				}
			}
			else
			{
				if (m_selectedItem.VehiclePart.State == UpgradeState.Installed && m_selectedItem.VehiclePart.NeedsFixing)
				{
					GameController.Instance.Character.Mechanics[0].InstantFixGadget(m_selectedItem.VehiclePart);
					HandleGadgetEquip(m_selectedItem);
				}
				else
				{
					GameController.Instance.Character.Mechanics[0].Upgrade(m_selectedItem.VehiclePart);
					HandleGadgetEquip(m_selectedItem);
				}
				m_selectedItem.RefreshItem();
			}
		}
		m_selectedItem = null;
	}
}
