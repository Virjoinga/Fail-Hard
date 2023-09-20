using System.Collections.Generic;
using UnityEngine;

public class FillScreenShotGrid : MonoBehaviour
{
	public GameObject ScreenShotItemPrefab;

	public PhotoViewer PhotoViewer;

	public float TopMostRotation;

	public float BaseRotation;

	public float UnderLayingRotationStep;

	public float Randomness;

	private List<Texture> m_textures;

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
		if (m_textures != null)
		{
			m_textures.Clear();
		}
	}

	private void OnClick()
	{
		if (m_textures.Count > 0)
		{
			List<Texture> photos = new List<Texture>(m_textures);
			Transform child = base.transform.GetChild(0);
			PhotoViewer.SetData(photos, child.localRotation, base.transform.localPosition);
			SendMessageUpwards("PauseStuntOver");
		}
	}
}
