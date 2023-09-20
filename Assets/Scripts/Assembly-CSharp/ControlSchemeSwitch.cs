using UnityEngine;

public class ControlSchemeSwitch : MonoBehaviour
{
	public GameObject LeftButton;

	public GameObject RightButton;

	public GameObject JumpButton;

	public GameObject JumpOffButton;

	public GameObject SkipButton;

	private void ToggleScheme()
	{
		LeftButton.SetActive(GetComponent<UICheckbox>().isChecked);
		RightButton.SetActive(GetComponent<UICheckbox>().isChecked);
		JumpButton.SetActive(GetComponent<UICheckbox>().isChecked);
		JumpOffButton.SetActive(GetComponent<UICheckbox>().isChecked);
		Vector3 localPosition = SkipButton.transform.localPosition;
		if (GetComponent<UICheckbox>().isChecked)
		{
			localPosition.x = -280f;
		}
		else
		{
			localPosition.x = -30f;
		}
		SkipButton.transform.localPosition = localPosition;
		if (ButtonControlScheme.Instance != null)
		{
			ButtonControlScheme.Instance.UsingButtonScheme = GetComponent<UICheckbox>().isChecked;
		}
	}
}
