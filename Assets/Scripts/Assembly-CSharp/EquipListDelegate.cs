using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class EquipListDelegate : MonoBehaviour
{
	public delegate void OnItemSelected(EquipListDelegate item);

	public UILabel ConditionLabel;

	public UILabel PriceLabel;

	public UISprite Sprite;

	public GameObject EquippedTag;

	public UISprite Background;

	public GameObject CoinSprite;

	private string m_spriteName;

	public VehiclePart VehiclePart { get; set; }

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	public void SetData(VehiclePart vp)
	{
		VehiclePart = vp;
		m_spriteName = VehiclePart.EquipIconTextureName;
		Sprite.spriteName = m_spriteName + "_on";
		Sprite.MakePixelPerfect();
		RefreshItem();
	}

	public void RefreshItem()
	{
		if (VehiclePart == null)
		{
			return;
		}
		if (GameController.Instance.Character.IsPurchased(VehiclePart))
		{
			PriceLabel.text = string.Empty;
			CoinSprite.SetActive(false);
			if (VehiclePart.MaxCondition < 0)
			{
				ConditionLabel.text = string.Empty;
			}
			else
			{
				ConditionLabel.text = VehiclePart.CurrentCondition.ToString();
			}
		}
		else
		{
			ConditionLabel.text = string.Empty;
			PriceLabel.text = VehiclePart.NextUpgradePrice().Amount.ToString();
			CoinSprite.SetActive(true);
		}
	}

	private void OnEnable()
	{
		RefreshItem();
	}

	public void Highlight(bool isOn)
	{
		if (isOn)
		{
			EquippedTag.SetActive(true);
			Background.alpha = 1f;
		}
		else
		{
			EquippedTag.SetActive(false);
			Background.alpha = 0.7f;
		}
	}

	private void OnClick()
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(this);
		}
	}
}
