using System.Collections.Generic;
using UnityEngine;

public class VehicleControlButton : MonoBehaviour
{
	public ButtonControlScheme.VehicleControl VehicleControlProperty;

	public UISprite ControlIcon;

	public UILabel RemainingActionsLabel;

	public GameObject RemainingActions;

	public GameObject KeyboardLabel;

	public List<Transform> KeyboardScaledTransforms;

	private float KeyboardScale = 5f / 6f;

	public bool IsPressed { get; private set; }

	private void Start()
	{
		if (KeyboardLabel != null)
		{
			KeyboardLabel.SetActive(false);
		}
	}

	public void SetIcon(string iconName)
	{
		if (!(ControlIcon == null))
		{
			ControlIcon.spriteName = iconName;
			ControlIcon.MakePixelPerfect();
			ControlIcon.cachedTransform.localScale *= 0.5f;
		}
	}

	public void SetLabel(string remainingActions)
	{
		if (remainingActions == string.Empty)
		{
			RemainingActions.SetActive(false);
			return;
		}
		RemainingActions.SetActive(true);
		RemainingActionsLabel.text = remainingActions;
	}

	public void SetPressed(bool pressed)
	{
		ButtonControlScheme.Instance.UsingKeyboard = false;
		IsPressed = pressed;
		base.gameObject.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
		switch (VehicleControlProperty)
		{
		case ButtonControlScheme.VehicleControl.Left:
			ButtonControlScheme.Instance.LeftPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.Right:
			ButtonControlScheme.Instance.RightPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.Jump:
			ButtonControlScheme.Instance.JumpPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.JumpOff:
			ButtonControlScheme.Instance.JumpOffPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.ActionHead:
			ButtonControlScheme.Instance.ActionHeadPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.ActionTorso:
			ButtonControlScheme.Instance.ActionTorsoPressed = pressed;
			break;
		case ButtonControlScheme.VehicleControl.ActionVehicle:
			ButtonControlScheme.Instance.ActionVehiclePressed = pressed;
			break;
		}
	}
}
