using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class PhotoViewer : MonoBehaviour
{
	public GameObject PhotoPrefab;

	public UISprite DimmerSprite;

	private List<Texture> m_photoTextures;

	private Quaternion m_initialRotation;

	private Vector3 m_initialPosition;

	private bool m_resetGrid;

	private bool m_delayedEnable;

	private void OnEnable()
	{
		m_delayedEnable = true;
	}

	private void Update()
	{
		if (!m_delayedEnable)
		{
			return;
		}
		GetComponent<UIDraggablePanel>().ResetPosition();
		UIGrid component = GetComponent<UIGrid>();
		component.cellWidth = (float)Screen.width * 0.925f * UIRoot.GetPixelSizeAdjustment(base.gameObject);
		for (int i = 0; i < m_photoTextures.Count; i++)
		{
			Texture photo = m_photoTextures[i];
			GameObject gameObject = NGUITools.AddChild(base.gameObject, PhotoPrefab);
			Photo component2 = gameObject.GetComponent<Photo>();
			if (i == 0)
			{
				component2.SetData(photo, m_initialRotation, m_initialPosition);
			}
			else
			{
				component2.SetData(photo, Quaternion.identity, Vector3.zero);
			}
			component2.ItemClicked += ItemClicked;
		}
		component.Reposition();
		GetComponent<UIDraggablePanel>().ResetPosition();
		GetComponent<UICenterOnChild>().Recenter();
		m_delayedEnable = false;
		TweenParms p_parms = new TweenParms().Prop("color", new Color(0f, 0f, 0f, 0.7f));
		HOTween.To(DimmerSprite, 1f, p_parms);
	}

	private void OnDisable()
	{
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<Photo>() != null)
			{
				item.GetComponent<Photo>().ItemClicked -= ItemClicked;
			}
			Object.Destroy(item.gameObject);
		}
		m_photoTextures.Clear();
		DimmerSprite.color = new Color(0f, 0f, 0f, 0f);
	}

	public void SetData(List<Texture> photos, Quaternion initialRotation, Vector3 initialPosition)
	{
		m_photoTextures = photos;
		m_initialRotation = initialRotation;
		m_initialPosition = initialPosition;
		base.transform.parent.gameObject.SetActive(true);
	}

	private void ItemClicked()
	{
		base.transform.parent.gameObject.SetActive(false);
	}
}
