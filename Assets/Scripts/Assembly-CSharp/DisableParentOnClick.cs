using UnityEngine;

public class DisableParentOnClick : MonoBehaviour
{
	private void OnClick()
	{
		base.transform.parent.gameObject.SetActive(false);
	}
}
