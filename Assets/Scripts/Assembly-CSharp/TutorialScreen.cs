using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
	public UILabel TutorialText;

	public UISprite RightButton;

	public UISprite LeftButton;

	public UITexture TutorialImage;

	public void SetData(string image, string text, string leftButton, string rightButton)
	{
		TutorialText.text = text;
		Texture2D mainTexture = (Texture2D)Resources.Load(image);
		TutorialImage.mainTexture = mainTexture;
		TutorialImage.MakePixelPerfect();
		if (leftButton == string.Empty)
		{
			LeftButton.gameObject.SetActive(false);
		}
		else
		{
			LeftButton.spriteName = leftButton;
		}
		if (rightButton == string.Empty)
		{
			RightButton.gameObject.SetActive(false);
		}
		else
		{
			RightButton.spriteName = rightButton;
		}
	}
}
