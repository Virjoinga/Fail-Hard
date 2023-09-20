using UnityEngine;

public class ButtonWithTextAndIcon : MonoBehaviour
{
	public UISprite icon;

	public UILabel label;

	public string Label
	{
		get
		{
			return label.text;
		}
		set
		{
			label.text = value;
		}
	}

	public string IconName
	{
		get
		{
			return icon.spriteName;
		}
		set
		{
			Vector3 localPosition = label.transform.localPosition;
			if (value == string.Empty)
			{
				localPosition.x = 91f;
				icon.gameObject.SetActive(false);
			}
			else
			{
				localPosition.x = 56f;
				icon.gameObject.SetActive(true);
				icon.spriteName = value;
			}
			label.transform.localPosition = localPosition;
		}
	}
}
