using System.Collections.Generic;
using Game;
using Game.Progress;
using UnityEngine;

public class ThemeListView : MonoBehaviour
{
	public GameObject itemDelegatePrefab;

	public ThemeCategory themeCategory;

	public GarageCameraController GarageCamera;

	public NavigateFromTo NavigateToIAP;

	public GameObject dialogPrefab;

	public Transform DialogParent;

	private GameObject m_dialog;

	public UIPanel ClippingPanel;

	private Dictionary<string, Bundle> model;

	private Bundle m_selectedBundle;

	private List<GameObject> m_listItems;

	public SpringPanel SpringPanel;

	private bool m_started;

	private void Start()
	{
		m_started = true;
		m_listItems = new List<GameObject>();
		OnEnable();
	}

	public void InitModel()
	{
		GameController instance = GameController.Instance;
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1.3333334f;
		Vector4 clipRange = ClippingPanel.clipRange;
		clipRange.z *= num / num2;
		ClippingPanel.clipRange = clipRange;
		UIGrid component = GetComponent<UIGrid>();
		component.cellWidth = 1024f * num / num2;
		model = instance.BundleModel.Model;
		int num3 = 100;
		ThemePage themePage = null;
		foreach (KeyValuePair<string, Bundle> item in model)
		{
			if (item.Value.TotalTargets() == 0)
			{
				Debug.LogWarning("Bundle with no targets " + item.Value.Name);
			}
			else if (themePage == null)
			{
				GameObject gameObject = NGUITools.AddChild(base.gameObject, itemDelegatePrefab);
				gameObject.name = item.Value.Name;
				themePage = gameObject.GetComponent<ThemePage>();
				themePage.ItemSelected += ThemeSelected;
				themePage.AddBundle(item.Value);
				m_listItems.Add(gameObject);
				num3++;
				gameObject.name = num3.ToString();
			}
			else
			{
				themePage.AddBundle(item.Value);
				themePage = null;
			}
		}
		component.Reposition();
		float num4 = 0f;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			ThemePage component2 = base.transform.GetChild(i).gameObject.GetComponent<ThemePage>();
			ThemeListDelegate themeListDelegate = component2.RequiresCompleteAnimation();
			if (themeListDelegate != null)
			{
				themeListDelegate.AnimateCompleted();
				num4 = (float)i * GetComponent<UIGrid>().cellWidth;
				break;
			}
		}
		SpringPanel.target = (0f - num4) * Vector3.right;
		SpringPanel.enabled = true;
	}

	public void OnEnable()
	{
		if (m_started)
		{
			if (model == null)
			{
				InitModel();
			}
			GarageCamera.ToLevelPosition();
		}
	}

	public void HideBackground()
	{
	}

	public void ThemeSelected(ThemeListDelegate data)
	{
		m_selectedBundle = data.Bundle;
		switch (m_selectedBundle.State)
		{
		case BundleState.BundleUnlocked:
			OpenPurchaseDialog("Purchase the level bundle?", m_selectedBundle.Description, m_selectedBundle.Price);
			break;
		case BundleState.BundleLocked:
		case BundleState.BundleCompletePending:
			break;
		case BundleState.BundlePurchased:
		case BundleState.BundleCompleted:
			GameController.Instance.CurrentBundle = m_selectedBundle;
			GameController.Instance.BundleToShow = m_selectedBundle;
			GetComponent<NavigateFromTo>().OnClick();
			break;
		}
	}

	public void OpenPurchaseDialog(string text, string description, Price price)
	{
		m_dialog = Object.Instantiate(dialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = DialogParent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = description;
		component.Character = "character_agent";
		Vector3 localScale = component.CharacterSprite.cachedTransform.localScale;
		localScale.x = 0f - localScale.x;
		component.CharacterSprite.cachedTransform.localScale = localScale;
		if (!GameController.Instance.Character.Afford(price))
		{
			component.Cost = "Get coins";
		}
		else
		{
			component.Cost = price.Amount.ToString();
		}
		component.DialogClosed += OnPopupClosed;
	}

	public void OnPopupClosed(bool isYes)
	{
		if (isYes)
		{
			if (!GameController.Instance.Character.Afford(m_selectedBundle.Price))
			{
				NavigateToIAP.ManualTrigger();
				return;
			}
			GameController.Instance.Character.Pay(m_selectedBundle.Price);
			GameController.Instance.Agent.BundlePurchased(m_selectedBundle);
			Tracking.DesignEvent(Tracking.Events.ItemPurchased, m_selectedBundle.Id, m_selectedBundle.Price.Amount);
		}
	}
}
