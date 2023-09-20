using System.Collections.Generic;
using UnityEngine;

public class ButtonControlScheme : MonoBehaviour
{
	public enum VehicleControl
	{
		Left = 0,
		Right = 1,
		Jump = 2,
		JumpOff = 3,
		ActionHead = 4,
		ActionTorso = 5,
		ActionVehicle = 6,
		None = 7
	}

	private class ButtonControlState
	{
		public bool IsPressed { get; set; }

		public bool ButtonDown { get; set; }

		public bool ButtonUp { get; set; }

		public bool ButtonClicked { get; set; }

		public bool ClickStarted { get; set; }
	}

	public static ButtonControlScheme Instance;

	public List<VehicleControlButton> VehicleButtons;

	private List<ButtonControlState> m_buttons;

	private Camera m_uiCamera;

	public bool UsingButtonScheme;

	public bool UsingKeyboard = true;

	public bool LeftPressed
	{
		get
		{
			return m_buttons[0].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.Left, value);
		}
	}

	public bool LeftDown
	{
		get
		{
			return StateDownGetter(VehicleControl.Left);
		}
		private set
		{
			m_buttons[0].ButtonDown = value;
		}
	}

	public bool LeftClicked
	{
		get
		{
			return StateClickGetter(VehicleControl.Left);
		}
		private set
		{
		}
	}

	public bool RightPressed
	{
		get
		{
			return m_buttons[1].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.Right, value);
		}
	}

	public bool RightClicked
	{
		get
		{
			return StateClickGetter(VehicleControl.Right);
		}
		private set
		{
		}
	}

	public bool RightDown
	{
		get
		{
			return StateDownGetter(VehicleControl.Right);
		}
		private set
		{
			m_buttons[1].ButtonDown = value;
		}
	}

	public bool JumpOffPressed { get; set; }

	public bool JumpPressed
	{
		get
		{
			return m_buttons[2].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.Jump, value);
		}
	}

	public bool JumpDown
	{
		get
		{
			return StateDownGetter(VehicleControl.Jump);
		}
		private set
		{
			m_buttons[2].ButtonDown = value;
		}
	}

	public bool ActionHeadPressed
	{
		get
		{
			return m_buttons[4].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.ActionHead, value);
		}
	}

	public bool ActionHeadDown
	{
		get
		{
			return StateDownGetter(VehicleControl.ActionHead);
		}
		private set
		{
			m_buttons[4].ButtonDown = value;
		}
	}

	public bool ActionTorsoPressed
	{
		get
		{
			return m_buttons[5].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.ActionTorso, value);
		}
	}

	public bool ActionTorsoDown
	{
		get
		{
			return StateDownGetter(VehicleControl.ActionTorso);
		}
		private set
		{
			m_buttons[5].ButtonDown = value;
		}
	}

	public bool ActionVehiclePressed
	{
		get
		{
			return m_buttons[6].IsPressed;
		}
		set
		{
			StateSetter(VehicleControl.ActionVehicle, value);
		}
	}

	public bool ActionVehicleDown
	{
		get
		{
			return StateDownGetter(VehicleControl.ActionVehicle);
		}
		private set
		{
			m_buttons[6].ButtonDown = value;
		}
	}

	private bool StateDownGetter(VehicleControl button)
	{
		bool buttonDown = m_buttons[(int)button].ButtonDown;
		m_buttons[(int)button].ButtonDown = false;
		return buttonDown;
	}

	private bool StateClickGetter(VehicleControl button)
	{
		bool buttonClicked = m_buttons[(int)button].ButtonClicked;
		m_buttons[(int)button].ButtonClicked = false;
		return buttonClicked;
	}

	private void StateSetter(VehicleControl button, bool value)
	{
		if (m_buttons[(int)button].IsPressed != value)
		{
			if (value)
			{
				m_buttons[(int)button].ButtonDown = true;
				m_buttons[(int)button].ClickStarted = true;
				Invoke("CheckClick", 0.1f);
			}
			else
			{
				m_buttons[(int)button].ButtonUp = true;
			}
		}
		m_buttons[(int)button].IsPressed = value;
	}

	private void CheckClick()
	{
		foreach (ButtonControlState button in m_buttons)
		{
			if (button.ClickStarted && !button.IsPressed)
			{
				button.ButtonClicked = true;
			}
			button.ClickStarted = false;
		}
	}

	private void Start()
	{
		Instance = this;
		if (m_buttons == null)
		{
			m_buttons = new List<ButtonControlState>();
			for (int i = 0; i < 8; i++)
			{
				m_buttons.Add(new ButtonControlState());
			}
		}
		Camera[] allCameras = Camera.allCameras;
		Camera[] array = allCameras;
		foreach (Camera camera in array)
		{
			if (camera.tag == "UICamera")
			{
				m_uiCamera = camera;
				break;
			}
		}
	}

	public void Reset()
	{
		foreach (ButtonControlState button in m_buttons)
		{
			button.ButtonClicked = false;
			button.ButtonDown = false;
			button.ButtonUp = false;
		}
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.D))
		{
			UsingKeyboard = true;
		}
		if (UsingKeyboard)
		{
			UpdateVehicleKeyboardControlStates();
		}
		else
		{
			UpdateTouchControls();
		}
	}

	private void UpdateTouchControls()
	{
		int touchCount = Input.touchCount;
		List<VehicleControl> list = new List<VehicleControl>();
		for (int i = 0; i < touchCount; i++)
		{
			VehicleControlButton vehicleControlButton = FindButton(Input.touches[i].position);
			if ((bool)vehicleControlButton)
			{
				list.Add(vehicleControlButton.VehicleControlProperty);
			}
		}
		for (int j = 0; j < VehicleButtons.Count; j++)
		{
			VehicleControlButton vehicleControlButton2 = VehicleButtons[j];
			if (list.Contains(vehicleControlButton2.VehicleControlProperty))
			{
				if (!vehicleControlButton2.IsPressed)
				{
					vehicleControlButton2.SetPressed(true);
				}
			}
			else if (vehicleControlButton2.IsPressed)
			{
				vehicleControlButton2.SetPressed(false);
			}
		}
	}

	private VehicleControlButton FindButton(Vector2 position)
	{
		Ray ray = m_uiCamera.ScreenPointToRay(position);
		VehicleControlButton result = null;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 5f, Layers.UILayer))
		{
			result = hitInfo.collider.gameObject.GetComponent<VehicleControlButton>();
		}
		return result;
	}

	private void UpdateVehicleKeyboardControlStates()
	{
		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E))
		{
			JumpPressed = true;
		}
		else
		{
			JumpPressed = false;
		}
		if (Input.GetKey(KeyCode.E))
		{
			ActionHeadPressed = true;
		}
		else
		{
			ActionHeadPressed = false;
		}
		if (Input.GetKey(KeyCode.W))
		{
			ActionTorsoPressed = true;
		}
		else
		{
			ActionTorsoPressed = false;
		}
		if (Input.GetKey(KeyCode.S))
		{
			ActionVehiclePressed = true;
		}
		else
		{
			ActionVehiclePressed = false;
		}
		if (Input.GetKey(KeyCode.D))
		{
			RightPressed = true;
		}
		else
		{
			RightPressed = false;
		}
		if (Input.GetKey(KeyCode.A))
		{
			LeftPressed = true;
		}
		else
		{
			LeftPressed = false;
		}
	}
}
