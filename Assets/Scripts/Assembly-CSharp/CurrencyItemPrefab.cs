using System;
using Game;
using Holoville.HOTween;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CurrencyItemPrefab : MonoBehaviour
{
	public UILabel PackAmount;

	public UILabel RealValue;

	public UISprite PackIcon;

	public GameObject AdFreeLabel;

	public GameObject RejectedPrice;

	public GameObject ForSaleSign;

	public GameObject GetJarIcon;

	public GameObject VideoIcon;

	public UILabel DescrptionLabel;

	public UISprite Background;

	public UISprite PriceBackground;

	public Transform Rotate;

	private Tweener m_tween;

	public string SmallPackIcon;

	public string MediumPackIcon;

	public string LargePackIcon;

	public string ExtraLargePackIcon;

	public string HugePackIcon;

	public string EnormousPackIcon;

	private CurrencyItem m_item;

	private Action<CurrencyItem> m_clickAction;

	private void OnDisable()
	{
		ProcessingAnimation(false);
	}

	private void OnClick()
	{
		if (m_item.IsAvailable && m_clickAction != null)
		{
			ProcessingAnimation(true);
			m_clickAction(m_item);
		}
	}

	public void SetDummyMode()
	{
		ProcessingAnimation(true);
		ForSaleSign.SetActive(false);
		PackIcon.enabled = false;
		PackAmount.enabled = false;
		RealValue.enabled = false;
		Background.alpha = 0.35f;
		PriceBackground.enabled = false;
	}

	public void SetData(CurrencyItem item, Action<CurrencyItem> clickAction)
	{
		m_item = item;
		m_clickAction = clickAction;
		PackAmount.text = item.GameCurrency.Amount.ToString();
		RealValue.text = item.FormattedPrice;
		switch (item.Type)
		{
		case CurrencyItem.PackType.Small:
			PackIcon.spriteName = SmallPackIcon;
			break;
		case CurrencyItem.PackType.Medium:
			PackIcon.spriteName = MediumPackIcon;
			break;
		case CurrencyItem.PackType.Large:
			PackIcon.spriteName = LargePackIcon;
			break;
		case CurrencyItem.PackType.ExtraLarge:
			PackIcon.spriteName = ExtraLargePackIcon;
			break;
		case CurrencyItem.PackType.Huge:
			PackIcon.spriteName = HugePackIcon;
			break;
		case CurrencyItem.PackType.Enormous:
			PackIcon.spriteName = EnormousPackIcon;
			break;
		case CurrencyItem.PackType.GetJar:
			PackIcon.gameObject.SetActive(false);
			ForSaleSign.SetActive(false);
			GetJarIcon.SetActive(true);
			DescrptionLabel.text = item.Description;
			DescrptionLabel.gameObject.SetActive(true);
			break;
		case CurrencyItem.PackType.VideoAds:
			PackIcon.gameObject.SetActive(false);
			ForSaleSign.SetActive(false);
			VideoIcon.SetActive(true);
			if (!item.IsAvailable)
			{
				DescrptionLabel.text = item.NotAvailableDescription;
				VideoIcon.GetComponent<UIWidget>().alpha = 0.25f;
				RealValue.text = string.Empty;
			}
			else
			{
				DescrptionLabel.text = item.Description;
			}
			DescrptionLabel.gameObject.SetActive(true);
			break;
		}
		if (PackIcon.gameObject.activeInHierarchy)
		{
			PackIcon.MakePixelPerfect();
		}
	}

	private void ProcessingAnimation(bool animate)
	{
		if (animate)
		{
			Rotate.gameObject.SetActive(true);
			Rotate.transform.localRotation = Quaternion.identity;
			TweenParms p_parms = new TweenParms().Prop("localRotation", new Vector3(0f, 0f, 360f), true).Loops(-1).Ease(EaseType.Linear);
			m_tween = HOTween.To(Rotate, 1f, p_parms);
		}
		else
		{
			Rotate.gameObject.SetActive(false);
			if (m_tween != null)
			{
				m_tween.Kill();
			}
		}
	}
}
