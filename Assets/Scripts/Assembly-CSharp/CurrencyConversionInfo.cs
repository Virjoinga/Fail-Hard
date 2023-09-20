using Game;
using UnityEngine;

public class CurrencyConversionInfo : MonoBehaviour
{
	public UILabel FromAmountLabel;

	public UILabel ToAmountLabel;

	public void SetData(ConvertToCurrencyItem item)
	{
		FromAmountLabel.text = item.FromCurrency.Amount.ToString();
		ToAmountLabel.text = item.ToCurrency.Amount.ToString();
	}
}
