using UnityEngine;

namespace Valinta
{
	public class ValintaPositioner : MonoBehaviour
	{
		[SerializeField]
		private Transform buttonStartPlayerTransform;

		[SerializeField]
		private Transform playerTransform;

		[SerializeField]
		private Transform buttonPositionInMenu;

		[SerializeField]
		private Transform playerPositionInMenu;

		[SerializeField]
		private Transform buttonPositionInGame;

		[SerializeField]
		private Transform playerPositionInGame;

		private GarageUI GarageUI;

		private GigUI GigUI;

		private GameObject LevelSelection;

		private GameObject ThemeSelection;

		private GameObject GigUILoadingScreen;

		private GameObject GigUIFader;

		private GameObject HiddenGridView;

		private GameObject HiddenThemeList;

		private int CurrentLoadedLevel = -1;

		private bool NewLevelLoaded;

		private void Awake()
		{
			buttonStartPlayerTransform.gameObject.SetActive(false);
			playerTransform.gameObject.SetActive(false);
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (Application.loadedLevel != CurrentLoadedLevel)
			{
				buttonStartPlayerTransform.gameObject.SetActive(false);
				playerTransform.gameObject.SetActive(false);
				CurrentLoadedLevel = Application.loadedLevel;
			}
			if (Application.loadedLevel == 1)
			{
				if (GarageUI == null)
				{
					GarageUI = Object.FindObjectOfType<GarageUI>();
					return;
				}
				if (LevelSelection == null)
				{
					LevelSelection = GarageUI.levelSelectionView;
				}
				if (ThemeSelection == null)
				{
					ThemeSelection = GarageUI.themeSelectionView;
				}
				if (HiddenGridView == null)
				{
					HiddenGridView = ThemeSelection.transform.FindChild("GridItemDisplay").gameObject;
				}
				if (HiddenThemeList == null)
				{
					HiddenThemeList = ThemeSelection.transform.FindChild("ThemeList").gameObject;
				}
				buttonStartPlayerTransform.gameObject.SetActive(LevelSelection.activeInHierarchy || ThemeSelection.activeInHierarchy);
				playerTransform.gameObject.SetActive(ValintaUI.Instance.IsPlayerVisible());
				buttonStartPlayerTransform.localPosition = buttonPositionInMenu.localPosition;
				playerTransform.localPosition = playerPositionInMenu.localPosition;
				HiddenGridView.SetActive(!ValintaUI.Instance.IsPlaylistVisible());
				HiddenThemeList.SetActive(!ValintaUI.Instance.IsPlaylistVisible());
			}
			else if (Application.loadedLevel > 1)
			{
				if (GigUI == null)
				{
					GigUI = Object.FindObjectOfType<GigUI>();
					return;
				}
				if (GigUILoadingScreen == null || GigUIFader == null)
				{
					GigUILoadingScreen = GigUI.ActivateOnAwake[0];
					GigUIFader = GigUI.ActivateOnAwake[1];
					return;
				}
				if (GigUILoadingScreen.activeInHierarchy || GigUIFader.activeInHierarchy)
				{
					buttonStartPlayerTransform.gameObject.SetActive(false);
					playerTransform.gameObject.SetActive(false);
					return;
				}
				buttonStartPlayerTransform.gameObject.SetActive(GigUI.Stars.activeInHierarchy);
				playerTransform.gameObject.SetActive(ValintaUI.Instance.IsPlayerVisible());
				buttonStartPlayerTransform.localPosition = buttonPositionInGame.localPosition;
				playerTransform.localPosition = playerPositionInGame.localPosition;
			}
			else
			{
				buttonStartPlayerTransform.gameObject.SetActive(false);
			}
			if (!buttonStartPlayerTransform.gameObject.activeInHierarchy)
			{
				playerTransform.gameObject.SetActive(false);
			}
		}
	}
}
