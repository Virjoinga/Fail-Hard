using UnityEngine;

public class ShowRealMoneyStore : MonoBehaviour
{
	public GameObject StorePrefab;

	public Transform StoreParentTransform;

	private GameObject m_storeObject;

	public void Show()
	{
		if (!(m_storeObject != null) || !m_storeObject.activeInHierarchy)
		{
			m_storeObject = NGUITools.AddChild(StoreParentTransform.gameObject, StorePrefab);
			Vector3 localPosition = m_storeObject.transform.localPosition;
			localPosition.z -= 2f;
			m_storeObject.transform.localPosition = localPosition;
		}
	}
}
