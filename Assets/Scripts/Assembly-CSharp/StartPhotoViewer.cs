using System.Collections.Generic;
using UnityEngine;

public class StartPhotoViewer : MonoBehaviour
{
	private void OnClick()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in base.transform)
		{
			list.Add(item.gameObject);
		}
	}
}
