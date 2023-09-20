using UnityEngine;

public class UIListPageIndicator : MonoBehaviour
{
	public UISprite sprite;

	public string Default;

	public string CurrentPosition;

	public void SetState(bool newState)
	{
		if (newState)
		{
			sprite.spriteName = CurrentPosition;
		}
		else
		{
			sprite.spriteName = Default;
		}
	}
}
