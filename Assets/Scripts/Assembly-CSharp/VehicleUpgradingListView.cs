using Game;
using UnityEngine;

public class VehicleUpgradingListView : MonoBehaviour
{
	public UIGrid VehicleList;

	public VehicleUpgradingDelegate VehicleUpgrading;

	public UILabel VehicleName;

	public UILabel VehiclePrice;

	public GameObject PurchaseButton;

	public GameObject BuyGadgetsButton;

	public GameObject PlayButton;

	public NavigateFromTo NavigateToIAP;

	public GameObject DialogPrefab;

	public Transform DialogParent;

	private GameObject m_dialog;

	private bool m_playPressed;

	public GadgetPageList PlayerGadgets;

	public GadgetPageList VehicleGadgets;

	public UITexture RenderTexture;

	public Ads Ads;

	private int m_currentVehicleindex;

	private GigStatus m_gigStatus;

	private GigStatus GigController
	{
		get
		{
			if (m_gigStatus == null)
			{
				GameObject gameObject = GameObject.Find("Level:root");
				if (gameObject != null)
				{
					m_gigStatus = gameObject.GetComponentInChildren<GigStatus>();
				}
			}
			return m_gigStatus;
		}
	}

	private void Start()
	{
		PlayerGadgets.GadgetEquipped += PlayerGadgets_GadgetEquipped;
		VehicleGadgets.GadgetEquipped += VehicleGadgets_GadgetEquipped;
		PurchaseButton.GetComponent<ButtonSignaler>().ButtonClick += Purchase_ButtonClick;
		PlayButton.GetComponent<ButtonSignaler>().ButtonClick += Play_ButtonClick;
	}

	private void Purchase_ButtonClick()
	{
		OpenPurchaseDialog("Buy vehicle?", GameController.Instance.VehicleModel.Model[m_currentVehicleindex].Description, GameController.Instance.VehicleModel.Model[m_currentVehicleindex].PriceTag);
	}

	private void Play_ButtonClick()
	{
		m_playPressed = true;
		if (GameController.Instance.VehicleModel.Model[m_currentVehicleindex].State == ItemState.Unlocked)
		{
			OpenPurchaseDialog("Buy vehicle?", GameController.Instance.VehicleModel.Model[m_currentVehicleindex].Description, GameController.Instance.VehicleModel.Model[m_currentVehicleindex].PriceTag);
		}
		else
		{
			ExitGarage();
		}
	}

	private void VehicleGadgets_GadgetEquipped(VehiclePart gadget, bool isEquipped)
	{
		if (isEquipped)
		{
			GigController.GigConfig.SetVehicleGadget(gadget);
		}
		else
		{
			GigController.GigConfig.RemoveGadget(gadget.ItemType);
		}
	}

	private void PlayerGadgets_GadgetEquipped(VehiclePart gadget, bool isEquipped)
	{
		if (isEquipped)
		{
			GigController.GigConfig.SetPlayerGadget(gadget);
		}
		else
		{
			GigController.GigConfig.RemoveGadget(gadget.ItemType);
		}
	}

	public void NextVehicle(int step)
	{
		GameController instance = GameController.Instance;
		m_currentVehicleindex += step;
		if (m_currentVehicleindex < 0)
		{
			m_currentVehicleindex += instance.VehicleModel.Model.Count;
		}
		else if (m_currentVehicleindex >= instance.VehicleModel.Model.Count)
		{
			m_currentVehicleindex -= instance.VehicleModel.Model.Count;
		}
		if (instance.VehicleModel.Model[m_currentVehicleindex].State == ItemState.Purchased)
		{
			instance.Character.CurrentVehicle = instance.VehicleModel.Model[m_currentVehicleindex];
		}
		GigController.GigConfig.SetVehicle(GameController.Instance.VehicleModel.Model[m_currentVehicleindex]);
		RefreshUI();
	}

	private void RefreshUI()
	{
		Vehicle vehicle = GameController.Instance.VehicleModel.Model[m_currentVehicleindex];
		VehicleUpgrading.SetData(vehicle);
		if (vehicle.State == ItemState.Unlocked)
		{
			PurchaseButton.SetActive(true);
			BuyGadgetsButton.SetActive(false);
			VehiclePrice.text = vehicle.PriceTag.Amount.ToString();
		}
		else
		{
			PurchaseButton.SetActive(false);
			BuyGadgetsButton.SetActive(true);
		}
		VehicleName.text = vehicle.Name;
	}

	private void SelectVehicle(Vehicle vehicle)
	{
		m_currentVehicleindex = 0;
		for (int i = 0; i < GameController.Instance.VehicleModel.Model.Count; i++)
		{
			if (GameController.Instance.VehicleModel.Model[i] == vehicle)
			{
				m_currentVehicleindex = i;
				break;
			}
		}
		NextVehicle(0);
	}

	private void OnEnable()
	{
		Ads.Show();
		if (GameController.Instance.Character.CurrentVehicle == null)
		{
			GigController.GigConfig.ShowPlayerAndVehicle(true, GameController.Instance.VehicleModel.Model[m_currentVehicleindex]);
			SelectVehicle(GameController.Instance.VehicleModel.Model[m_currentVehicleindex]);
		}
		else
		{
			GigController.GigConfig.ShowPlayerAndVehicle(true, GameController.Instance.Character.CurrentVehicle);
			SelectVehicle(GameController.Instance.Character.CurrentVehicle);
		}
		GigController.PointCameraAtGarage();
		if (!Ads.IsAdFree() && !VideoAds.WatchedDuringSession && VideoAds.VideoCount() > 0 && Random.Range(0f, 1f) > 0.9f && GameController.Instance.Character.ContainsEvent(new GameEvent(GameEvent.GameEventType.LevelCompleted, "SuburbPoolAndVan")))
		{
			OpenVideoTip("Get free coins?", "Take a look at these fine videos.");
		}
	}

	private void OnDisable()
	{
		Ads.Hide();
	}

	public void OpenVideoTip(string text, string description)
	{
		m_dialog = Object.Instantiate(DialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = base.transform.parent.parent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = description;
		component.Character = "character_agent";
		component.Cost = "Let's do it!";
		component.DialogClosed += OnVideoTipClosed;
	}

	private void OnVideoTipClosed(bool isYes)
	{
		GameController instance = GameController.Instance;
		if (isYes)
		{
			NavigateToIAP.ManualTrigger();
		}
		else
		{
			VideoAds.WatchedDuringSession = true;
		}
	}

	public void OpenPurchaseDialog(string text, string description, Price price)
	{
		m_dialog = Object.Instantiate(DialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = base.transform.parent.parent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = description;
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

	private void OnPopupClosed(bool isYes)
	{
		GameController instance = GameController.Instance;
		if (isYes)
		{
			Price priceTag = instance.VehicleModel.Model[m_currentVehicleindex].PriceTag;
			if (instance.Character.Pay(priceTag))
			{
				instance.VehicleModel.Model[m_currentVehicleindex].State = ItemState.Purchased;
				instance.Character.CurrentVehicle = instance.VehicleModel.Model[m_currentVehicleindex];
				Tracking.DesignEvent(Tracking.Events.ItemPurchased, instance.Character.CurrentVehicle.Id, priceTag.Amount);
				if (m_playPressed)
				{
					ExitGarage();
				}
				else
				{
					RefreshUI();
				}
			}
			else
			{
				NavigateToIAP.ManualTrigger();
			}
		}
		m_playPressed = false;
	}

	private void ExitGarage()
	{
		GameController.Instance.Character.CurrentVehicle = GameController.Instance.VehicleModel.Model[m_currentVehicleindex];
		GameObject gameObject = GameObject.Find("Level:root");
		gameObject.GetComponentInChildren<GigStatus>().RestartStunt();
		Invoke("Navigate", 0.2f);
	}

	private void Navigate()
	{
		GigStatus gigController = GigController;
		if (gigController != null)
		{
			gigController.GigConfig.ShowPlayerAndVehicle(false, null);
		}
		GetComponent<NavigateFromTo>().ManualTrigger();
	}
}
