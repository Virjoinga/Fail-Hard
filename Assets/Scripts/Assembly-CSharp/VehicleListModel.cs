using System.Collections.Generic;
using Game;

public class VehicleListModel
{
	private static VehicleListModel s_instance;

	public List<Vehicle> Model;

	public static VehicleListModel Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new VehicleListModel();
			}
			return s_instance;
		}
	}

	private VehicleListModel()
	{
		Model = new List<Vehicle>();
		InitWithServerData();
	}

	private void InitWithServerData()
	{
		List<VehicleParameters> list = Storage.Instance.LoadVehicleParameters();
		foreach (VehicleParameters item in list)
		{
			VehicleInfo i = Storage.Instance.LoadVehicleInfo(item.Id);
			Model.Add(new Vehicle(item, i));
		}
	}

	private void InitWithDummyData()
	{
		VehicleParameters vehicleParameters = new VehicleParameters();
		vehicleParameters.Id = "PappaT";
		vehicleParameters.Name = "Pappa Tunturi";
		vehicleParameters.ResourceName = "MopedPappaT";
		vehicleParameters.MaxCondition = 30;
		vehicleParameters.PriceTag = new Price(0);
		vehicleParameters.DefaultAcceleration = 6f;
		vehicleParameters.DefaultTopSpeed = 25f;
		vehicleParameters.DefaultSpinVelocity = 160f;
		vehicleParameters.DefaultJumpOffForce = 7.5f;
		vehicleParameters.DefaultJumpOffForceAbsorb = 0f;
		vehicleParameters.DefaultJumpingForce = 9f;
		VehicleInfo vehicleInfo = new VehicleInfo();
		vehicleInfo.CurrentCondition = 99;
		vehicleInfo.State = ItemState.Purchased;
		Vehicle vehicle = new Vehicle(vehicleParameters, vehicleInfo);
		vehicle.SetTopSpeed(28f);
		Model.Add(vehicle);
		vehicleParameters = new VehicleParameters();
		vehicleParameters.Id = "PV50";
		vehicleParameters.Name = "HANDO APE";
		vehicleParameters.ResourceName = "MopedSharp";
		vehicleParameters.MaxCondition = 30;
		vehicleParameters.FixingDelay = 60f;
		vehicleParameters.PriceTag = new Price(0);
		vehicleParameters.DefaultAcceleration = 7f;
		vehicleParameters.DefaultTopSpeed = 30f;
		vehicleParameters.DefaultSpinVelocity = 210f;
		vehicleParameters.DefaultJumpOffForce = 6.75f;
		vehicleParameters.DefaultJumpOffForceAbsorb = 0.4f;
		vehicleParameters.DefaultJumpingForce = 11f;
		vehicleInfo = new VehicleInfo();
		vehicleInfo.CurrentCondition = vehicleParameters.MaxCondition;
		vehicleInfo.State = ItemState.Purchased;
		vehicle = new Vehicle(vehicleParameters, vehicleInfo);
		Model.Add(vehicle);
		vehicleParameters = new VehicleParameters();
		vehicleParameters.Id = "VESPA";
		vehicleParameters.Name = "Vespa";
		vehicleParameters.ResourceName = "MopedSkooter";
		vehicleParameters.MaxCondition = 30;
		vehicleParameters.FixingDelay = 60f;
		vehicleParameters.PriceTag = new Price(0);
		vehicleParameters.DefaultAcceleration = 9f;
		vehicleParameters.DefaultTopSpeed = 25f;
		vehicleParameters.DefaultSpinVelocity = 300f;
		vehicleParameters.DefaultJumpOffForce = 6.75f;
		vehicleParameters.DefaultJumpOffForceAbsorb = 0.8f;
		vehicleParameters.DefaultJumpingForce = 12f;
		vehicleInfo = new VehicleInfo();
		vehicleInfo.CurrentCondition = vehicleParameters.MaxCondition;
		vehicleInfo.State = ItemState.Purchased;
		vehicle = new Vehicle(vehicleParameters, vehicleInfo);
		Model.Add(vehicle);
	}

	public Vehicle GetVehicle(string id)
	{
		foreach (Vehicle item in Model)
		{
			if (item.Id == id)
			{
				return item;
			}
		}
		return null;
	}
}
