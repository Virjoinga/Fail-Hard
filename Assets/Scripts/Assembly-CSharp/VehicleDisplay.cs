using Game;
using UnityEngine;

public class VehicleDisplay : MonoBehaviour
{
	public UIFilledSprite topSpeed;

	public UIFilledSprite acceleration;

	public UIFilledSprite deltaTopSpeed;

	public UIFilledSprite deltaAcceleration;

	public GameObject NormalState;

	public GameObject PremiumState;

	public UILabel VehicleConditionLabel;

	public UIFilledSprite VehicleCondition;

	public UITimer FixTimer;

	private Vehicle m_vehicle;

	private bool m_refreshNeeded;

	public GameObject dialogPrefab;

	private GameObject dialog;

	public float DeltaTopSpeed { get; set; }

	public float DeltaAcceleration { get; set; }

	private void Start()
	{
		m_vehicle = GameController.Instance.Character.CurrentVehicle;
		m_vehicle.StateChanged += CurrentVehicle_StateChanged;
		RefreshVehicleCondition();
	}

	private void CurrentVehicle_StateChanged(Vehicle vehicle)
	{
		m_refreshNeeded = true;
	}

	private void OnDestroy()
	{
		m_vehicle.StateChanged -= CurrentVehicle_StateChanged;
	}

	private void Update()
	{
		if (m_refreshNeeded)
		{
			RefreshVehicleCondition();
			m_refreshNeeded = false;
		}
		if (DeltaTopSpeed >= 0f)
		{
			topSpeed.fillAmount = MapSpeedToFillAmount(m_vehicle.GetTopSpeed());
			deltaTopSpeed.fillAmount = MapSpeedToFillAmount(m_vehicle.GetTopSpeed() + DeltaTopSpeed);
			deltaTopSpeed.color = new Color(0.6f, 1f, 0.6f, 1f);
		}
		else
		{
			topSpeed.fillAmount = MapSpeedToFillAmount(m_vehicle.GetTopSpeed() + DeltaTopSpeed);
			deltaTopSpeed.fillAmount = MapSpeedToFillAmount(m_vehicle.GetTopSpeed());
			deltaTopSpeed.color = new Color(1f, 0.6f, 0.6f, 1f);
		}
		if (DeltaAcceleration >= 0f)
		{
			acceleration.fillAmount = MapAccelerationToFillAmount(m_vehicle.GetMaxAcceleration(0f));
			deltaAcceleration.fillAmount = MapAccelerationToFillAmount(m_vehicle.GetMaxAcceleration(0f) + DeltaAcceleration);
			deltaAcceleration.color = new Color(0.6f, 1f, 0.6f, 1f);
		}
		else
		{
			acceleration.fillAmount = MapAccelerationToFillAmount(m_vehicle.GetMaxAcceleration(0f) + DeltaAcceleration);
			deltaAcceleration.fillAmount = MapAccelerationToFillAmount(m_vehicle.GetMaxAcceleration(0f));
			deltaAcceleration.color = new Color(1f, 0.6f, 0.6f, 1f);
		}
	}

	private float MapSpeedToFillAmount(float speed)
	{
		return 0.7f * (speed - 20f) / 30f;
	}

	private float MapAccelerationToFillAmount(float accel)
	{
		return 0.7f * (accel - 15f) / 30f;
	}

	private void PermanentFixClicked()
	{
		Debug.LogError("DEPRECATED PermanentFixClicked");
	}

	private void RefreshVehicleCondition()
	{
		Debug.LogError("DEPRECATED VehicleDisplay RefreshVehicleCondition");
	}
}
