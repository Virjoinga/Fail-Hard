using UnityEngine;

public class LabelLocalizer : MonoBehaviour
{
	private void Start()
	{
		GetComponent<UILabel>().text = Language.Get(GetComponent<UILabel>().text);
	}
}
