using UnityEngine;

public class ReverseControlsButton : MonoBehaviour
{
	public UISprite ButtonSprite;

	private bool m_inverted;

	public GameObject DialogPrefab;

	public Transform DialogParent;

	private GameObject m_dialog;

	private void Start()
	{
		int @int = PlayerPrefs.GetInt("InvertControls");
		if (@int > 0)
		{
			ToggleInvert();
		}
	}

	private void OnClick()
	{
		if (m_inverted)
		{
			OpenConfirmationDialog("Invert spin controls?", "Back spin with throttle button.");
		}
		else
		{
			OpenConfirmationDialog("Invert spin controls?", "Forward spin with throttle button.");
		}
	}

	private void ToggleInvert()
	{
		m_inverted = !m_inverted;
		if (m_inverted)
		{
			ButtonSprite.spriteName = "spin_right";
			PlayerPrefs.SetInt("InvertControls", 1);
		}
		else
		{
			ButtonSprite.spriteName = "spin_left";
			PlayerPrefs.SetInt("InvertControls", 0);
		}
		PlayerPrefs.Save();
	}

	public void OpenConfirmationDialog(string text, string description)
	{
		m_dialog = Object.Instantiate(DialogPrefab, base.transform.position, base.transform.rotation) as GameObject;
		m_dialog.transform.parent = DialogParent;
		m_dialog.transform.localScale = Vector3.one;
		Dialog component = m_dialog.GetComponent<Dialog>();
		component.PanelsParent = DialogParent;
		component.Title = text;
		component.Description = description;
		component.Character = "character_mechanic";
		Vector3 localScale = component.CharacterSprite.cachedTransform.localScale;
		localScale.x = 0f - localScale.x;
		component.CharacterSprite.cachedTransform.localScale = localScale;
		component.Cost = "Let's do it";
		component.HideCurrencyIcon(false);
		component.DialogClosed += OnPopupClosed;
	}

	public void OnPopupClosed(bool isYes)
	{
		if (isYes)
		{
			ToggleInvert();
		}
	}
}
