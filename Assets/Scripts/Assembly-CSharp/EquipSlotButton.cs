using UnityEngine;

public class EquipSlotButton : MonoBehaviour
{
	public GadgetPageList Grid;

	public VehiclePartType Filter;

	private void OnClick()
	{
		if (Grid.gameObject.activeInHierarchy && Grid.PartTypeFilter == Filter)
		{
			Grid.gameObject.SetActive(false);
		}
		else
		{
			Grid.SetData(Filter);
		}
	}

	private void OnDisable()
	{
		Grid.gameObject.SetActive(false);
	}
}
