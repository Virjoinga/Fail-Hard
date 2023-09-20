using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class GadgetPageList : MonoBehaviour
{
	public delegate void OnGadgetEquipped(VehiclePart gadget, bool isEquipped);

	private List<VehiclePart> m_model;

	private List<GameObject> m_listItems;

	public VehiclePartType PartTypeFilter;

	public GameObject itemDelegatePrefab;

	private bool m_started;

	public UIGrid Grid;

	public NavigateFromTo NavigateToIAP;

	[method: MethodImpl(32)]
	public event OnGadgetEquipped GadgetEquipped;

	private void Start()
	{
		m_started = true;
	}

	public void SetData(VehiclePartType newType)
	{
		if (newType != PartTypeFilter || !base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			PartTypeFilter = newType;
			InitModel();
		}
	}

	private void OnClick()
	{
		base.gameObject.SetActive(false);
	}

	private void InitModel()
	{
		if (PartTypeFilter == VehiclePartType.VehicleGadget)
		{
			m_model = GameController.Instance.ItemModelPlaceholder.GetVehicleItemsOfType(PartTypeFilter, GameController.Instance.Character.CurrentVehicle);
		}
		else
		{
			m_model = GameController.Instance.ItemModelPlaceholder.GetItemsOfType(PartTypeFilter);
		}
		if (m_listItems == null)
		{
			m_listItems = new List<GameObject>();
		}
		DestroyAll();
		List<VehiclePart> list = new List<VehiclePart>();
		List<VehiclePart> list2 = new List<VehiclePart>();
		foreach (VehiclePart item in m_model)
		{
			if (item.HasAction || item.HasProtection)
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
		}
		if (list.Count > 0)
		{
			GameObject gameObject = NGUITools.AddChild(Grid.gameObject, itemDelegatePrefab);
			gameObject.GetComponent<EquipListView>().SetData(list);
			gameObject.GetComponent<EquipListView>().GadgetEquipped += GadgetPageList_GadgetEquipped;
			gameObject.GetComponent<EquipListView>().GetCoins += GadgetPageList_GetCoins;
			m_listItems.Add(gameObject);
		}
		if (list2.Count > 0)
		{
			GameObject gameObject2 = NGUITools.AddChild(Grid.gameObject, itemDelegatePrefab);
			gameObject2.GetComponent<EquipListView>().SetData(list2);
			gameObject2.GetComponent<EquipListView>().GadgetEquipped += GadgetPageList_GadgetEquipped;
			gameObject2.GetComponent<EquipListView>().GetCoins += GadgetPageList_GetCoins;
			m_listItems.Add(gameObject2);
		}
		Grid.Reposition();
		GetComponentInChildren<UIDraggablePanel>().ResetPosition();
	}

	private void GadgetPageList_GetCoins()
	{
		NavigateToIAP.ManualTrigger();
	}

	private void GadgetPageList_GadgetEquipped(VehiclePart gadget, bool isEquipped)
	{
		if (this.GadgetEquipped != null)
		{
			this.GadgetEquipped(gadget, isEquipped);
		}
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
			listItem.GetComponent<EquipListView>().GadgetEquipped -= GadgetPageList_GadgetEquipped;
			listItem.GetComponent<EquipListView>().GetCoins -= GadgetPageList_GetCoins;
			Object.Destroy(listItem);
		}
		m_listItems.Clear();
	}

	private void OnDisable()
	{
		DestroyAll();
	}
}
