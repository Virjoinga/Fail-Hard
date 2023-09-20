using System.Collections.Generic;
using Game;
using UnityEngine;

public class VehicleUpgradingDelegate : MonoBehaviour
{
	private List<VehiclePart> m_model;

	public GameObject itemDelegatePrefab;

	private Vehicle m_vehicle;

	public List<GameObject> UpgradeListItems;

	public NavigateFromTo NavigateToIAP;

	public GameObject DialogPrefab;

	public Transform DialogParent;

	private GameObject m_dialog;

	private ItemListDelegate m_selectedItem;

	private void Start()
	{
		for (int i = 0; i < 3; i++)
		{
			UpgradeListItems[i].GetComponent<ItemListDelegate>().ItemSelected += OnItemSelected;
			UpgradeListItems[i].GetComponent<ItemListDelegate>().StateChanged += OnItemStateChanged;
		}
	}

	public void SetData(Vehicle vehicle)
	{
		m_vehicle = vehicle;
		ItemListModel itemModelPlaceholder = GameController.Instance.ItemModelPlaceholder;
		VehiclePart vehicleUpgrade = itemModelPlaceholder.GetVehicleUpgrade(m_vehicle, VehicleUpgradeType.VehicleUpgradeAcceleration);
		if (vehicleUpgrade != null)
		{
			UpgradeListItems[0].GetComponent<ItemListDelegate>().SetData(vehicle, vehicleUpgrade);
			vehicleUpgrade = itemModelPlaceholder.GetVehicleUpgrade(m_vehicle, VehicleUpgradeType.VehicleUpgradeTopSpeed);
			UpgradeListItems[1].GetComponent<ItemListDelegate>().SetData(vehicle, vehicleUpgrade);
			vehicleUpgrade = itemModelPlaceholder.GetVehicleUpgrade(m_vehicle, VehicleUpgradeType.VehicleUpgradeSpin);
			if (vehicleUpgrade == null)
			{
				vehicleUpgrade = itemModelPlaceholder.GetVehicleUpgrade(m_vehicle, VehicleUpgradeType.VehicleUpgradeLift);
			}
			UpgradeListItems[2].GetComponent<ItemListDelegate>().SetData(vehicle, vehicleUpgrade);
		}
		else
		{
			SetDataOld(vehicle);
		}
	}

	public void SetDataOld(Vehicle vehicle)
	{
		m_vehicle = vehicle;
		int num = 0;
		switch (vehicle.Id)
		{
		case "PV50":
			num = 0;
			break;
		case "PappaT":
			num = 10;
			break;
		case "VESPA":
			num = 20;
			break;
		}
		for (int i = 0; i < 3; i++)
		{
			List<VehiclePart> upgrades = vehicle.GetUpgrades(num + i);
			VehiclePart vehiclePart = null;
			bool maxed = false;
			if (upgrades != null && upgrades.Count > 0)
			{
				if (upgrades.Count >= 10)
				{
					maxed = true;
					vehiclePart = upgrades[upgrades.Count - 1];
				}
				else
				{
					vehiclePart = upgrades[upgrades.Count - 1];
					vehiclePart = GameController.Instance.ItemModelPlaceholder.GetNextItem(vehiclePart);
					if (vehiclePart == null)
					{
						maxed = true;
						vehiclePart = upgrades[upgrades.Count - 1];
					}
				}
			}
			else
			{
				vehiclePart = GameController.Instance.ItemModelPlaceholder.GetItem(num + i, 0);
			}
			UpgradeListItems[i].GetComponent<ItemListDelegate>().SetData(vehicle, vehiclePart, maxed);
		}
	}

	public void OnItemSelected(ItemListDelegate listItem)
	{
		OpenPurchaseDialog("Do you want to upgrade?", listItem.itemData.Description, listItem.itemData.NextUpgradePrice());
		m_selectedItem = listItem;
	}

	public void OpenPurchaseDialog(string text, string description, Price price)
	{
		m_dialog = Object.Instantiate(DialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = DialogParent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = description;
		if (!GameController.Instance.Character.Afford(price))
		{
			component.Cost = "Get coins";
		}
		else
		{
			component.Cost = price.Amount.ToString();
		}
		component.DialogClosed += OnPopupClosed;
	}

	public void OnPopupClosed(bool isYes)
	{
		if (isYes)
		{
			Price price = m_selectedItem.itemData.NextUpgradePrice();
			if (!GameController.Instance.Character.Afford(price))
			{
				NavigateToIAP.ManualTrigger();
			}
			else
			{
				GameController.Instance.Character.Mechanics[0].Upgrade(m_selectedItem.itemData);
				SetData(m_vehicle);
			}
		}
		m_selectedItem = null;
	}

	public void OnItemStateChanged(ItemListDelegate listItem)
	{
	}
}
