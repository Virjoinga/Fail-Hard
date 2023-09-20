using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
	private void OnClick()
	{
		Object.Destroy(base.gameObject);
	}
}
