using System.Collections.Generic;
using Game;
using UnityEngine;

public class ConvertCurrencyItemGrid : MonoBehaviour
{
	public GameObject ConvertCurrencyItemPrototype;

	public GameObject ConfirmConversionDialogPrototype;

	public GameObject CurrencyConversionInfoPrototype;

	public GameObject[] DisableOnDialogShow;

	private ConvertToCurrencyItem m_convertRequested;

	public void Start()
	{
		List<ConvertToCurrencyItem> list = GameController.Instance.Store.ConvertCurrencyItems();
		foreach (ConvertToCurrencyItem item in list)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, ConvertCurrencyItemPrototype);
			ConvertToCurrencyPrefab component = gameObject.GetComponent<ConvertToCurrencyPrefab>();
			component.SetData(item);
			component.OnClicked += ConversionClicked;
		}
		UIGrid component2 = GetComponent<UIGrid>();
		if ((bool)component2)
		{
			component2.Reposition();
		}
	}

	private void ConversionClicked(ConvertToCurrencyItem item)
	{
		GameObject gameObject = NGUITools.AddChild(base.transform.parent.gameObject, ConfirmConversionDialogPrototype);
		Vector3 localPosition = gameObject.transform.localPosition;
		localPosition.z -= 1f;
		gameObject.transform.localPosition = localPosition;
		GameObject[] disableOnDialogShow = DisableOnDialogShow;
		foreach (GameObject gameObject2 in disableOnDialogShow)
		{
			gameObject2.SetActive(false);
		}
		Dialog component = gameObject.GetComponent<Dialog>();
		m_convertRequested = item;
		GameObject gameObject3 = NGUITools.AddChild(component.CustomContentParent, CurrencyConversionInfoPrototype);
		CurrencyConversionInfo component2 = gameObject3.GetComponent<CurrencyConversionInfo>();
		component2.SetData(item);
	}

	private void ConvertOrCancel(bool convert)
	{
		Debug.LogWarning("DEPRECATED: NO DIAMONDS IN THE GAME");
		if (convert)
		{
			if (GameController.Instance.Character.Afford(new Price(m_convertRequested.FromCurrency.Amount, Price.CurrencyType.Diamonds)))
			{
				GameController.Instance.Character.Coins += m_convertRequested.ToCurrency.Amount;
				AudioManager.Instance.NGPlay((AudioClip)Resources.Load("Audio/buy_gold2"));
			}
			Object.Destroy(base.gameObject.transform.parent.gameObject);
		}
		else
		{
			GameObject[] disableOnDialogShow = DisableOnDialogShow;
			foreach (GameObject gameObject in disableOnDialogShow)
			{
				gameObject.SetActive(true);
			}
		}
		m_convertRequested = null;
	}
}
