using Game;
using UnityEngine;

public class SpeedoMeter : MonoBehaviour
{
	public Transform cruiseControlTarget;

	public Transform m_pointer;

	public float m_minSpeedAngle;

	public float m_maxSpeedAngle;

	private UIFilledSprite cruiseControlIndicator;

	public void Start()
	{
		cruiseControlIndicator = GetComponentInChildren<UIFilledSprite>();
	}

	public void Update()
	{
		GameController instance = GameController.Instance;
		VehicleBase vehicle = instance.Character.Vehicle;
		if ((bool)vehicle)
		{
			RotateElement(m_pointer, vehicle.CurrentSpeed);
			UpdateCruiseControlIndicator(vehicle.TargetSpeed);
		}
	}

	private void RotateElement(Transform tr, float speed)
	{
		float num = speed / GameController.Instance.Character.Vehicle.vehicleData.UltimateMaxSpeed;
		float num2 = m_minSpeedAngle + num * (m_maxSpeedAngle - m_minSpeedAngle);
		if (num2 < m_maxSpeedAngle)
		{
			num2 = m_maxSpeedAngle;
		}
		tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0f, 0f, num2), 0.1f);
	}

	private void UpdateCruiseControlIndicator(float speed)
	{
		cruiseControlIndicator.fillAmount = 0.65f * GameController.Instance.Character.Vehicle.Throttle;
	}
}
