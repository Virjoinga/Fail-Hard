using System.Diagnostics;
using Game;
using UnityEngine;
using Valinta;

public class AndroidBackButton : MonoBehaviour
{
	public bool AskExit;

	public GameController.GameUIView view;

	public bool ExitGig;

	public bool IsBackNavigation;

	public NavigateFromTo BackNavigator;

	public GameObject YesNoDialog;

	private GameObject m_dialog;

	public Transform DialogParent;

	public Ads Ads;

	private bool m_buttonFilter;

	private void Update()
	{
		if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && Input.GetKeyUp(KeyCode.Escape) && !m_buttonFilter)
		{
			m_buttonFilter = true;
			if (ValintaUI.Instance.IsPlaylistVisible())
			{
				ValintaUI.Instance.TogglePlaylistWindow();
				m_buttonFilter = false;
			}
			else if (AskExit)
			{
				OpenDialog("Exit game?");
			}
			else if (ExitGig)
			{
				Ads.Show();
				QuitStunt();
			}
			else if (IsBackNavigation)
			{
				BackNavigator.NavigateBack();
			}
			else
			{
				BackNavigator.ManualTrigger();
			}
		}
	}

	private void QuitStunt()
	{
		GameController.Instance.CurrentView = view;
		Application.LoadLevel(1);
	}

	public void OpenDialog(string text)
	{
		m_dialog = Object.Instantiate(YesNoDialog, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = DialogParent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = "Thank you, come back soon.";
		component.Character = "character_agent";
		Vector3 localScale = component.CharacterSprite.cachedTransform.localScale;
		localScale.x = 0f - localScale.x;
		component.CharacterSprite.cachedTransform.localScale = localScale;
		component.Cost = "Bye bye";
		component.HideCurrencyIcon(false);
		component.DialogClosed += OnPopupClosed;
	}

	public void OnPopupClosed(bool isYes)
	{
		m_buttonFilter = false;
		if (isYes)
		{
			LocalStorage.Instance.Shutdown();
			Process.GetCurrentProcess().Kill();
		}
	}

	private void OnDisable()
	{
		m_buttonFilter = false;
	}

	private void OnEnable()
	{
		m_buttonFilter = false;
	}
}
