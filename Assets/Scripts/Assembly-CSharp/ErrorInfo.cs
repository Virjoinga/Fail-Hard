using UnityEngine;

public class ErrorInfo : MonoBehaviour
{
	public UILabel ErrorLabel;

	public void Show(string errorText)
	{
		base.gameObject.SetActive(true);
		ErrorLabel.text = errorText;
	}

	private void OnDisable()
	{
		base.gameObject.SetActive(false);
	}
}
