using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
	public delegate void OnPopupClosed(bool isYes);

	public UILabel titleLabel;

	public UILabel descriptionLabel;

	public UISprite itemIcon;

	public ButtonWithTextAndIcon yesButton;

	public ButtonWithTextAndIcon noButton;

	private VehiclePart vehiclePart;

	public string Title
	{
		get
		{
			return titleLabel.text;
		}
		set
		{
			titleLabel.text = value;
		}
	}

	public string YesButtonLabel
	{
		get
		{
			return yesButton.Label;
		}
		set
		{
			yesButton.Label = value;
		}
	}

	public string YesButtonIcon
	{
		get
		{
			return yesButton.IconName;
		}
		set
		{
			yesButton.IconName = value;
		}
	}

	public string NoButtonLabel
	{
		get
		{
			return noButton.Label;
		}
		set
		{
			noButton.Label = value;
		}
	}

	public string NoButtonIcon
	{
		get
		{
			return noButton.IconName;
		}
		set
		{
			noButton.IconName = value;
		}
	}

	[method: MethodImpl(32)]
	public event OnPopupClosed PopupClosed;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetData(VehiclePart partData)
	{
		vehiclePart = partData;
		descriptionLabel.text = vehiclePart.Description;
		YesButtonLabel = vehiclePart.NextUpgradePrice().Amount.ToString();
		YesButtonIcon = "coin";
		NoButtonIcon = string.Empty;
		NoButtonLabel = "No thanks";
		itemIcon.spriteName = partData.IconTextureName;
		itemIcon.MakePixelPerfect();
		itemIcon.transform.localScale *= 0.5f;
	}

	public void OnYes()
	{
		if (this.PopupClosed != null)
		{
			this.PopupClosed(true);
		}
	}

	public void OnNo()
	{
		if (this.PopupClosed != null)
		{
			this.PopupClosed(false);
		}
	}
}
