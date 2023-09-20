using Game;
using UnityEngine;

public class ResetClient : MonoBehaviour
{
	public void ResetClientId()
	{
		CloudStorageST.Instance.ResetClientId();
		Application.Quit();
	}
}
