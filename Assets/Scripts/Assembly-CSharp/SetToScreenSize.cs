using UnityEngine;

public class SetToScreenSize : MonoBehaviour
{
	public float Scale = 1f;

	private int m_previousWidth;

	private int m_previousHeight;

	private void Start()
	{
		AdjustScale();
	}

	private void Update()
	{
		if (!Application.isEditor)
		{
			base.enabled = false;
		}
		if (m_previousWidth != Screen.width || m_previousHeight != Screen.height)
		{
			AdjustScale();
		}
	}

	private void AdjustScale()
	{
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		float num = 1f;
		if ((bool)uIRoot)
		{
			num = (float)uIRoot.manualHeight / (float)Screen.height;
		}
		int num2 = (int)((float)Screen.height * num * Scale);
		int num3 = (int)((float)Screen.width * num * Scale);
		base.transform.localScale = new Vector3(num3, num2, 1f);
		m_previousHeight = num2;
		m_previousWidth = num3;
	}
}
