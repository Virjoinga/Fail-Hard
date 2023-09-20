using UnityEngine;

public class DisableOnClick : MonoBehaviour
{
	private void OnClick()
	{
		base.gameObject.SetActive(false);
	}
}
