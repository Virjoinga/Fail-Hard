using Game;
using UnityEngine;

public class UISetSelectedLevel : MonoBehaviour
{
	public void OnClick()
	{
		GameController instance = GameController.Instance;
		instance.SetSelectedArea(1);
	}
}
