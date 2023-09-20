using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class ItemListDelegate : MonoBehaviour
{
	public delegate void OnItemSelected(ItemListDelegate listItem);

	public delegate void OnItemStateChanged(ItemListDelegate listItem);

	public UILabel PriceLabel;

	public UISprite itemPicture;

	public UISprite ProgressBar;

	public UISprite DeltaBar;

	public VehiclePart itemData;

	public UISprite UpgradeButton;

	public GameObject CoinSprite;

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	[method: MethodImpl(32)]
	public event OnItemStateChanged StateChanged;

	public void SetData(Vehicle vehicleToInstall, VehiclePart item)
	{
		bool flag = false;
		itemData = item;
		ShowIcon(item.IconTextureName);
		if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeAcceleration)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightAcceleration);
		}
		else if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeSpin)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightSpin);
		}
		else if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeTopSpeed)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightTopSpeed);
		}
		else if (itemData.UpgradeType != VehicleUpgradeType.VehicleUpgradeLift)
		{
			Debug.LogWarning("Invalid upgrade type.");
		}
		if (vehicleToInstall.State != ItemState.Purchased)
		{
			UpgradeButton.spriteName = "plus_disabled";
			GetComponent<Collider>().enabled = false;
			DeltaBar.fillAmount = 0f;
			PriceLabel.text = string.Empty;
			CoinSprite.SetActive(false);
			flag = true;
		}
		else if (item.CurrentLevel == item.MaxLevel - 1)
		{
			UpgradeButton.spriteName = "plus_disabled";
			GetComponent<Collider>().enabled = false;
			DeltaBar.fillAmount = 0f;
			PriceLabel.text = "MAX LEVEL";
			CoinSprite.SetActive(false);
			flag = true;
		}
		else
		{
			UpgradeButton.spriteName = "plus";
			GetComponent<Collider>().enabled = true;
			PriceLabel.text = itemData.UpgradePrices[itemData.CurrentLevel + 1].ToString();
			CoinSprite.SetActive(true);
		}
		switch (item.UpgradeType)
		{
		case VehicleUpgradeType.VehicleUpgradeTopSpeed:
			ProgressBar.fillAmount = (vehicleToInstall.GetTopSpeed() - 10f) / 50f;
			if (!flag)
			{
				float num4 = itemData.UpgradeDeltas[itemData.CurrentLevel + 1] - itemData.UpgradeDeltas[itemData.CurrentLevel];
				DeltaBar.fillAmount = (vehicleToInstall.GetTopSpeed() + num4 - 10f) / 50f;
			}
			break;
		case VehicleUpgradeType.VehicleUpgradeAcceleration:
			ProgressBar.fillAmount = (vehicleToInstall.GetMaxAcceleration(0f) - 4f) / 16f;
			if (!flag)
			{
				float num2 = itemData.UpgradeDeltas[itemData.CurrentLevel + 1] - itemData.UpgradeDeltas[itemData.CurrentLevel];
				DeltaBar.fillAmount = (vehicleToInstall.GetMaxAcceleration(0f) + num2 - 4f) / 16f;
			}
			break;
		case VehicleUpgradeType.VehicleUpgradeSpin:
			ProgressBar.fillAmount = (vehicleToInstall.CurrentMaxSpinVelocity - 100f) / 300f;
			if (!flag)
			{
				float num3 = itemData.UpgradeDeltas[itemData.CurrentLevel + 1] - itemData.UpgradeDeltas[itemData.CurrentLevel];
				DeltaBar.fillAmount = (vehicleToInstall.CurrentMaxSpinVelocity + num3 - 100f) / 300f;
			}
			break;
		case VehicleUpgradeType.VehicleUpgradeLift:
			ProgressBar.fillAmount = (vehicleToInstall.CurrentMaxLift - 5f) / 15f;
			if (!flag)
			{
				float num = itemData.UpgradeDeltas[itemData.CurrentLevel + 1] - itemData.UpgradeDeltas[itemData.CurrentLevel];
				DeltaBar.fillAmount = (vehicleToInstall.CurrentMaxLift + num - 5f) / 15f;
			}
			break;
		}
	}

	public void SetData(Vehicle vehicleToInstall, VehiclePart item, bool maxed)
	{
		itemData = item;
		ShowIcon(item.IconTextureName);
		if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeAcceleration)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightAcceleration);
		}
		else if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeSpin)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightSpin);
		}
		else if (itemData.UpgradeType == VehicleUpgradeType.VehicleUpgradeTopSpeed)
		{
			GetComponent<HighlightOnNotification>().ReconfigureSubscription(Notification.Types.HighlightTopSpeed);
		}
		else
		{
			Debug.LogWarning("Invalid upgrade type.");
		}
		if (vehicleToInstall.State != ItemState.Purchased)
		{
			UpgradeButton.spriteName = "plus_disabled";
			GetComponent<Collider>().enabled = false;
			DeltaBar.fillAmount = 0f;
			PriceLabel.text = string.Empty;
			CoinSprite.SetActive(false);
			maxed = true;
		}
		else if (maxed)
		{
			UpgradeButton.spriteName = "plus_disabled";
			GetComponent<Collider>().enabled = false;
			DeltaBar.fillAmount = 0f;
			PriceLabel.text = "MAX LEVEL";
			CoinSprite.SetActive(false);
		}
		else
		{
			UpgradeButton.spriteName = "plus";
			GetComponent<Collider>().enabled = true;
			PriceLabel.text = itemData.NextUpgradePrice().Amount.ToString();
			CoinSprite.SetActive(true);
		}
		switch (item.UpgradeType)
		{
		case VehicleUpgradeType.VehicleUpgradeTopSpeed:
			ProgressBar.fillAmount = (vehicleToInstall.GetTopSpeed() - 10f) / 50f;
			if (!maxed)
			{
				DeltaBar.fillAmount = (vehicleToInstall.GetTopSpeed() + item.TopSpeedDelta - 10f) / 50f;
			}
			break;
		case VehicleUpgradeType.VehicleUpgradeAcceleration:
			ProgressBar.fillAmount = (vehicleToInstall.GetMaxAcceleration(0f) - 4f) / 16f;
			if (!maxed)
			{
				DeltaBar.fillAmount = (vehicleToInstall.GetMaxAcceleration(0f) + item.AccelerationDelta - 4f) / 16f;
			}
			break;
		case VehicleUpgradeType.VehicleUpgradeSpin:
			ProgressBar.fillAmount = (vehicleToInstall.CurrentMaxSpinVelocity - 100f) / 300f;
			if (!maxed)
			{
				DeltaBar.fillAmount = (vehicleToInstall.CurrentMaxSpinVelocity + item.SpinDelta - 100f) / 300f;
			}
			break;
		}
	}

	public void OnClick()
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(this);
		}
	}

	private void ShowIcon(string textureName)
	{
		itemPicture.spriteName = textureName;
		itemPicture.MakePixelPerfect();
	}
}
