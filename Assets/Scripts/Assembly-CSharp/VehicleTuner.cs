using Game;
using UnityEngine;

public class VehicleTuner : MonoBehaviour
{
	public UILabel Label1;

	public UISlider Slider1;

	public float Slider1Max;

	public UILabel Label2;

	public UISlider Slider2;

	public float Slider2Max;

	public EquipListView[] EquipLists;

	private void Start()
	{
		EquipListView[] equipLists = EquipLists;
		foreach (EquipListView equipListView in equipLists)
		{
			equipListView.GadgetEquipped += elv_GadgetEquipped;
		}
	}

	private void elv_GadgetEquipped(VehiclePart gadget, bool isEquipped)
	{
		if (gadget.Name == "Rocket 1")
		{
			GameObject gameObject = GameObject.Find("RocketPack(Clone)");
			if (gameObject != null)
			{
				gameObject.GetComponent<GadgetRocket>().boostForce = Slider2Max * Slider2.sliderValue;
				gameObject.GetComponent<GadgetRocket>().boostDuration = Slider1Max * Slider1.sliderValue;
			}
		}
	}

	public void OnSpeedChange(float val)
	{
		float[] array = new float[5] { 30f, 34f, 38f, 41f, 44f };
		Label1.text = array[Mathf.RoundToInt(4f * val)].ToString();
		if (GameController.Instance.Character.Player != null)
		{
			GameController.Instance.Character.Player.currentVehicle.vehicleData.SetTopSpeed(array[Mathf.RoundToInt(4f * val)]);
		}
	}

	public void OnAccelChange(float val)
	{
		float[] array = new float[4] { 7f, 10f, 13f, 16f };
		Label2.text = array[Mathf.RoundToInt(3f * val)].ToString();
		if (GameController.Instance.Character.Player != null)
		{
			GameController.Instance.Character.Player.currentVehicle.vehicleData.SetMaxAcceleration(array[Mathf.RoundToInt(3f * val)]);
		}
	}

	public void OnSlider1Change(float val)
	{
		Label1.text = (Slider1Max * val).ToString();
		GameObject gameObject = GameObject.Find("RocketPack(Clone)");
		if (gameObject != null)
		{
			gameObject.GetComponent<GadgetRocket>().boostDuration = Slider1Max * val;
		}
	}

	public void OnSlider2Change(float val)
	{
		Label2.text = (Slider2Max * val).ToString();
		GameObject gameObject = GameObject.Find("RocketPack(Clone)");
		if (gameObject != null)
		{
			gameObject.GetComponent<GadgetRocket>().boostForce = Slider2Max * val;
		}
	}
}
