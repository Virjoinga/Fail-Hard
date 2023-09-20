using UnityEngine;

public class UIUpdateListIndicator : MonoBehaviour
{
	public GameObject listIndicatorPrefab;

	public void SetSize(int size)
	{
		int childCount = base.transform.childCount;
		if (size != childCount)
		{
			if (size < childCount)
			{
				while (size != base.transform.childCount)
				{
					Transform child = base.transform.GetChild(base.transform.childCount - 1);
					Object.Destroy(child);
				}
			}
			else
			{
				while (size != base.transform.childCount)
				{
					GameObject gameObject = (GameObject)Object.Instantiate(listIndicatorPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
					gameObject.transform.parent = base.gameObject.transform;
					gameObject.transform.localScale = new Vector3(22f, 22f, 1f);
				}
			}
		}
		UIGrid component = GetComponent<UIGrid>();
		if ((bool)component)
		{
			component.Reposition();
		}
	}

	public void SetPosition(int pos)
	{
		UIListPageIndicator[] componentsInChildren = GetComponentsInChildren<UIListPageIndicator>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (i != pos)
			{
				componentsInChildren[i].SetState(false);
			}
			else
			{
				componentsInChildren[i].SetState(true);
			}
		}
	}
}
