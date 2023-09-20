using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class ThemePage : MonoBehaviour
{
	public delegate void OnItemSelected(ThemeListDelegate listItem);

	public GameObject itemDelegatePrefab;

	private List<GameObject> m_bundles;

	public List<Transform> BundleRoots;

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	public bool AddBundle(Bundle bundle)
	{
		GameController instance = GameController.Instance;
		if (m_bundles == null)
		{
			m_bundles = new List<GameObject>();
		}
		if (m_bundles.Count > 1)
		{
			return false;
		}
		if (bundle != null)
		{
			GameObject gameObject = NGUITools.AddChild(BundleRoots[m_bundles.Count].gameObject, itemDelegatePrefab);
			gameObject.name = bundle.Name;
			gameObject.GetComponent<ThemeListDelegate>().SetData(bundle);
			gameObject.GetComponent<ThemeListDelegate>().ItemSelected += ThemeSelected;
			m_bundles.Add(gameObject);
		}
		return true;
	}

	public void ThemeSelected(ThemeListDelegate data)
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(data);
		}
	}

	public ThemeListDelegate RequiresCompleteAnimation()
	{
		foreach (GameObject bundle in m_bundles)
		{
			if (bundle.GetComponent<ThemeListDelegate>().Bundle.State == BundleState.BundleCompletePending)
			{
				return bundle.GetComponent<ThemeListDelegate>();
			}
		}
		return null;
	}
}
