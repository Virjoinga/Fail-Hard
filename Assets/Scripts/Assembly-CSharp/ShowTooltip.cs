using Game;
using UnityEngine;

public class ShowTooltip : MonoBehaviour
{
	public enum ToolTipEnum
	{
		ThrottleButtonTip = 0,
		BrakeButtonTip = 1,
		JumpButtonTip = 2,
		JumpOffButtonTip = 3,
		SpinForwardButtonTip = 4,
		UpgradeAccelerationTip = 5,
		PurchaseFirstVehicleTip = 6,
		UpgradeSpinTip = 7,
		UpgradeTopSpeedTip = 8
	}

	public ToolTipEnum ToolTipType;

	private bool m_tipShown;

	private GigStatus m_gigStatus;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("GigController");
		if (gameObject == null)
		{
			Debug.LogError("No GigController in scene!");
			return;
		}
		m_gigStatus = gameObject.GetComponent<GigStatus>();
		m_gigStatus.StateChanged += m_gigStatus_StateChanged;
	}

	private void m_gigStatus_StateChanged(GigStatus.GigState newState)
	{
		if (!m_tipShown && newState == GigStatus.GigState.StuntReadyToStart)
		{
			switch (ToolTipType)
			{
			case ToolTipEnum.ThrottleButtonTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialThrottle, null);
					NotificationCentre.Send(Notification.Types.HighlightThrottle, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.JumpButtonTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialJump, null);
					NotificationCentre.Send(Notification.Types.HighlightJump, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.JumpOffButtonTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialJumpOff, null);
					NotificationCentre.Send(Notification.Types.HighlightJump, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.SpinForwardButtonTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialSpinForward, null);
					NotificationCentre.Send(Notification.Types.HighlightBrake, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.BrakeButtonTip:
				break;
			}
		}
		else if (m_tipShown && newState == GigStatus.GigState.StuntActive)
		{
			NotificationCentre.Send(Notification.Types.HideTutorials, null);
		}
		else
		{
			if (m_tipShown || newState != GigStatus.GigState.InGarage)
			{
				return;
			}
			switch (ToolTipType)
			{
			case ToolTipEnum.UpgradeAccelerationTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialUpgradeAcceleration, null);
					NotificationCentre.Send(Notification.Types.HighlightAcceleration, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.UpgradeSpinTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialUpgradeSpin, null);
					NotificationCentre.Send(Notification.Types.HighlightSpin, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.UpgradeTopSpeedTip:
				if (!GameController.Instance.CurrentLevel.IsCompleted)
				{
					NotificationCentre.Send(Notification.Types.TutorialUpgradeTopSpeed, null);
					NotificationCentre.Send(Notification.Types.HighlightTopSpeed, null);
				}
				m_tipShown = true;
				break;
			case ToolTipEnum.PurchaseFirstVehicleTip:
				if (GameController.Instance.Character.CurrentVehicle == null)
				{
					NotificationCentre.Send(Notification.Types.TutorialPurchaseFirstVehicle, null);
					NotificationCentre.Send(Notification.Types.HighlightVehiclePurchase, null);
				}
				m_tipShown = true;
				break;
			}
		}
	}
}
