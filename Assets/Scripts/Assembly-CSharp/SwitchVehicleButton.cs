using UnityEngine;

public class SwitchVehicleButton : MonoBehaviour
{
	public int Step;

	public VehicleUpgradingListView VehicleList;

	private void OnClick()
	{
		VehicleList.NextVehicle(Step);
	}
}
