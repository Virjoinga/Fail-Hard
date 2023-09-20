using Game;
using Holoville.HOTween;
using UnityEngine;

public class GarageUI : MonoBehaviour
{
	public GameObject mainView;

	public GameObject themeSelectionView;

	public GameObject levelSelectionView;

	public GameObject connectingView;

	public TextMesh ScoreText;

	public GameObject CurrentVehicle;

	public Transform VehiclePosition;

	private Transform m_vehicleSlot;

	public GameObject GameLogo;

	public Transform LogoAnimationTarget;

	private void Start()
	{
		GameController.Instance.LevelDatabase.LevelDatabasePopulated += LevelsReady;
		if (GameController.Instance.LevelDatabase.Initialized)
		{
			switch (GameController.Instance.CurrentView)
			{
			case GameController.GameUIView.MainView:
				connectingView.SetActive(false);
				ResetLogo();
				mainView.SetActive(true);
				break;
			case GameController.GameUIView.Garage:
				connectingView.SetActive(false);
				ResetLogo();
				mainView.SetActive(false);
				themeSelectionView.SetActive(false);
				break;
			case GameController.GameUIView.ThemeSelection:
				connectingView.SetActive(false);
				ResetLogo();
				mainView.SetActive(false);
				themeSelectionView.SetActive(true);
				break;
			case GameController.GameUIView.LevelGrid:
				connectingView.SetActive(false);
				ResetLogo();
				mainView.SetActive(false);
				themeSelectionView.SetActive(false);
				levelSelectionView.SetActive(true);
				break;
			default:
				Debug.LogWarning("Couldn't recognize requested view type.");
				connectingView.SetActive(false);
				ResetLogo();
				mainView.SetActive(true);
				break;
			}
			if (ScoreText != null)
			{
				ScoreText.text = GameController.Instance.Character.Score.ToString();
			}
			SpawnVehicle();
		}
		GameController.Instance.SetState(GameController.GameState.Menu);
	}

	private void OnDestroy()
	{
		GameController.Instance.LevelDatabase.LevelDatabasePopulated -= LevelsReady;
	}

	private void SpawnVehicle()
	{
		if (GameController.Instance.Character.CurrentVehicle == null)
		{
			return;
		}
		GameObject original = (GameObject)Resources.Load(GameController.Instance.Character.CurrentVehicle.ResourceName);
		CurrentVehicle = Object.Instantiate(original, VehiclePosition.position, VehiclePosition.rotation) as GameObject;
		CurrentVehicle.GetComponentInChildren<VehicleBase>().DisableAllDynamics();
		m_vehicleSlot = RecursiveFind(CurrentVehicle.transform, "GadgetPosition", 0);
		foreach (VehiclePart equippedGadget in GameController.Instance.Character.CurrentVehicle.GetEquippedGadgets())
		{
			if (equippedGadget.ItemType == VehiclePartType.VehicleGadget)
			{
				SetVehicleGadget(equippedGadget);
			}
		}
	}

	private void LevelsReady(LevelDatabase db)
	{
		ConnectingCompleted();
		connectingView.SetActive(false);
		mainView.SetActive(true);
		if (ScoreText != null)
		{
			ScoreText.text = GameController.Instance.Character.Score.ToString();
		}
		SpawnVehicle();
	}

	private void ConnectingCompleted()
	{
		TweenParms p_parms = new TweenParms().Prop("position", LogoAnimationTarget.position, false).Ease(EaseType.EaseOutBounce);
		HOTween.To(GameLogo.transform, 0.7f, p_parms);
		TweenParms tweenParms = new TweenParms().Prop("localScale", LogoAnimationTarget.localScale, false).Ease(EaseType.EaseOutBounce);
		tweenParms.OnComplete(ChangeLogoParent);
		HOTween.To(GameLogo.transform, 0.7f, tweenParms);
	}

	private void ResetLogo()
	{
		GameLogo.transform.position = LogoAnimationTarget.position;
		GameLogo.transform.localScale = LogoAnimationTarget.localScale;
		GameLogo.SetActive(false);
	}

	private void ChangeLogoParent()
	{
		GameLogo.transform.parent = LogoAnimationTarget;
		GameLogo.transform.localScale = Vector3.one;
		GameLogo.transform.localPosition = Vector3.zero;
	}

	public void SetVehicleGadget(VehiclePart gadget)
	{
		GameObject original = (GameObject)Resources.Load(gadget.GadgetPrefabResource);
		GameObject gameObject = Object.Instantiate(original, m_vehicleSlot.position, m_vehicleSlot.rotation) as GameObject;
		gameObject.transform.parent = m_vehicleSlot.parent;
	}

	private Transform RecursiveFind(Transform node, string name, int depth)
	{
		if (depth > 15)
		{
			return null;
		}
		if (node.gameObject.name == name)
		{
			return node;
		}
		for (int i = 0; i < node.childCount; i++)
		{
			Transform transform = RecursiveFind(node.GetChild(i), name, depth + 1);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}
}
