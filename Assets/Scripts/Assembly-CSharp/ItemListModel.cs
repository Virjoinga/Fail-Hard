using System.Collections.Generic;
using Game;

public class ItemListModel
{
	private static ItemListModel s_instance;

	private Dictionary<string, VehiclePart> parts;

	private HashSet<int> partSets;

	public static ItemListModel Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new ItemListModel();
			}
			return s_instance;
		}
	}

	private ItemListModel()
	{
		parts = new Dictionary<string, VehiclePart>();
		partSets = new HashSet<int>();
		InitWithServerData();
	}

	private void InitWithServerData()
	{
		List<VehiclePartParameters> list = Storage.Instance.LoadVehiclePartParameters();
		foreach (VehiclePartParameters item in list)
		{
			partSets.Add(item.SetIndex);
			VehiclePartInfo i = Storage.Instance.LoadVehiclePartInfo(string.Empty + item.SetIndex + item.Index);
			parts.Add(string.Empty + item.SetIndex + item.Index, new VehiclePart(item, i));
		}
	}

	public int Count()
	{
		return partSets.Count;
	}

	public Dictionary<string, VehiclePart> GetAllParts()
	{
		return parts;
	}

	public VehiclePart GetItem(string id)
	{
		if (parts.ContainsKey(id))
		{
			return parts[id];
		}
		return null;
	}

	public VehiclePart GetItem(int partSet, int index)
	{
		string key = string.Empty + partSet + index;
		if (parts.ContainsKey(key))
		{
			return parts[key];
		}
		return null;
	}

	public VehiclePart GetNextItem(VehiclePart part)
	{
		string key = string.Empty + part.SetIndex + (part.Index + 1);
		if (parts.ContainsKey(key))
		{
			return parts[key];
		}
		return null;
	}

	public List<VehiclePart> GetPartSet(int partSet)
	{
		List<VehiclePart> list = new List<VehiclePart>();
		for (VehiclePart vehiclePart = GetItem(partSet, 0); vehiclePart != null; vehiclePart = GetNextItem(vehiclePart))
		{
			list.Add(vehiclePart);
		}
		return list;
	}

	public List<VehiclePart> GetItemsOfType(VehiclePartType itemType)
	{
		List<VehiclePart> list = new List<VehiclePart>();
		foreach (VehiclePart value in parts.Values)
		{
			if (value.ItemType == itemType)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<VehiclePart> GetVehicleItemsOfType(VehiclePartType itemType, Vehicle forVehicle)
	{
		List<VehiclePart> list = new List<VehiclePart>();
		foreach (VehiclePart value in parts.Values)
		{
			if (value.ItemType == itemType && value.RequiresId == forVehicle.Id)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public VehiclePart GetVehicleUpgrade(Vehicle forVehicle, VehicleUpgradeType itemType)
	{
		VehiclePart result = null;
		foreach (VehiclePart value in parts.Values)
		{
			if (value.ItemType == VehiclePartType.VehicleUpgrade && value.UpgradeType == itemType && value.RequiresId == forVehicle.Id)
			{
				if (value.MaxLevel > 0)
				{
					result = value;
				}
				break;
			}
		}
		return result;
	}
}
