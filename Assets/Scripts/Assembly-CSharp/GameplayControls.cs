using Holoville.HOTween;
using UnityEngine;

public class GameplayControls : MonoBehaviour
{
	public GameObject ActionHead;

	public GameObject ActionTorso;

	public GameObject ActionVehicle;

	public GameObject LeftButtons;

	public GameObject RightButtons;

	public int SlideInLength;

	public float SlideInTime;

	private bool m_shown;

	private Vector3 leftCachePos;

	private Vector3 rightCachePos;

	private void Start()
	{
		leftCachePos = LeftButtons.transform.localPosition;
		rightCachePos = RightButtons.transform.localPosition;
	}

	public void DisableEnable(GameObject target, bool isEnabled)
	{
		target.SetActive(isEnabled);
	}

	public void Show(string headIcon, string headLabel, string torsoIcon, string torsoLabel, string vehicleIcon, string vehicleLabel)
	{
		if (!m_shown)
		{
			if (headIcon == string.Empty)
			{
				ActionHead.SetActive(false);
			}
			else
			{
				ActionHead.SetActive(true);
				ActionHead.GetComponent<VehicleControlButton>().SetIcon(headIcon);
				ActionHead.GetComponent<VehicleControlButton>().SetLabel(headLabel);
			}
			if (torsoIcon == string.Empty)
			{
				ActionTorso.SetActive(false);
			}
			else
			{
				ActionTorso.SetActive(true);
				ActionTorso.GetComponent<VehicleControlButton>().SetIcon(torsoIcon);
				ActionTorso.GetComponent<VehicleControlButton>().SetLabel(torsoLabel);
			}
			if (vehicleIcon == string.Empty)
			{
				ActionVehicle.SetActive(false);
			}
			else
			{
				ActionVehicle.SetActive(true);
				ActionVehicle.GetComponent<VehicleControlButton>().SetIcon(vehicleIcon);
				ActionVehicle.GetComponent<VehicleControlButton>().SetLabel(vehicleLabel);
			}
			m_shown = true;
			SlideButton(LeftButtons, leftCachePos, SlideInLength, m_shown);
			SlideButton(RightButtons, rightCachePos, -SlideInLength, m_shown);
		}
	}

	public void Show()
	{
		Show(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
	}

	public void UpdateLabel(GameObject target, string label)
	{
		target.GetComponent<VehicleControlButton>().SetLabel(label);
	}

	public void UpdateLabels(string headLabel, string torsoLabel, string vehicleLabel)
	{
		ActionHead.GetComponent<VehicleControlButton>().SetLabel(headLabel);
		ActionTorso.GetComponent<VehicleControlButton>().SetLabel(torsoLabel);
		ActionVehicle.GetComponent<VehicleControlButton>().SetLabel(vehicleLabel);
	}

	public void Hide()
	{
		if (m_shown)
		{
			m_shown = false;
			SlideButton(LeftButtons, leftCachePos, 0f, m_shown);
			SlideButton(RightButtons, rightCachePos, 0f, m_shown);
		}
	}

	private void SlideButton(GameObject button, Vector3 origo, float slideAmount, bool collisions)
	{
		TweenParms p_parms = new TweenParms().Prop("localPosition", origo + slideAmount * Vector3.right, false).Ease(EaseType.EaseOutBounce);
		HOTween.To(button.transform, SlideInTime, p_parms);
		Collider[] componentsInChildren = button.GetComponentsInChildren<Collider>(true);
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = collisions;
		}
	}
}
