using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ConvertToCurrencyPrefab : MonoBehaviour
{
	public delegate void Clicked(ConvertToCurrencyItem item);

	public UILabel GetAmount;

	public UILabel CostAmount;

	public UISprite PackIcon;

	public string SmallPackIcon;

	public string MediumPackIcon;

	public string LargePackIcon;

	public string HugePackIcon;

	private ConvertToCurrencyItem m_item;

	[method: MethodImpl(32)]
	public event Clicked OnClicked;

	private void OnClick()
	{
		if (this.OnClicked != null)
		{
			this.OnClicked(m_item);
		}
	}

	public void SetData(ConvertToCurrencyItem item)
	{
		m_item = item;
		GetAmount.text = item.ToCurrency.Amount.ToString();
		CostAmount.text = item.FromCurrency.Amount.ToString();
		switch (item.Type)
		{
		case ConvertToCurrencyItem.PackType.Small:
			PackIcon.spriteName = SmallPackIcon;
			break;
		case ConvertToCurrencyItem.PackType.Medium:
			PackIcon.spriteName = MediumPackIcon;
			break;
		case ConvertToCurrencyItem.PackType.Large:
			PackIcon.spriteName = LargePackIcon;
			break;
		case ConvertToCurrencyItem.PackType.Huge:
			PackIcon.spriteName = HugePackIcon;
			break;
		}
	}
}
