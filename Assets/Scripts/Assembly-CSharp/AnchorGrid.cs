using UnityEngine;

public class AnchorGrid : MonoBehaviour
{
	private UIGrid m_grid;

	private Transform m_gridTransform;

	private int m_previousCount;

	private Vector3 m_zeroPosition;

	private void Start()
	{
		m_grid = GetComponent<UIGrid>();
		m_gridTransform = base.transform;
		m_zeroPosition = new Vector3(0f, m_gridTransform.localPosition.y, m_gridTransform.localPosition.z);
	}

	private void LateUpdate()
	{
		int childCount = m_gridTransform.childCount;
		if (childCount != m_previousCount)
		{
			m_previousCount = childCount;
			UpdatePosition(m_previousCount);
		}
	}

	private void UpdatePosition(int newCount)
	{
		if (m_grid == null)
		{
			m_grid = GetComponent<UIGrid>();
		}
		if ((bool)m_grid)
		{
			float cellWidth = m_grid.cellWidth;
			float num = cellWidth * (float)newCount;
			Vector3 zeroPosition = m_zeroPosition;
			zeroPosition.x -= num / 2f - cellWidth / 2f;
			m_gridTransform.localPosition = zeroPosition;
		}
	}
}
