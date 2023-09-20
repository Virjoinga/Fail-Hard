using System.Collections.Generic;
using UnityEngine;

public class CombineMesh : MonoBehaviour
{
	public Material Material;

	private List<MeshFilter> m_childMeshFilters;

	private MeshFilter m_combinedMesh;

	private void Awake()
	{
		m_childMeshFilters = new List<MeshFilter>();
		m_combinedMesh = base.gameObject.AddComponent<MeshFilter>();
		base.gameObject.AddComponent<MeshRenderer>().material = Material;
		foreach (Transform item in base.transform)
		{
			MeshFilter[] componentsInChildren = item.GetComponentsInChildren<MeshFilter>();
			MeshFilter[] array = componentsInChildren;
			foreach (MeshFilter meshFilter in array)
			{
				m_childMeshFilters.Add(meshFilter);
				meshFilter.gameObject.SetActive(false);
			}
		}
	}

	public void LateUpdate()
	{
		if (m_childMeshFilters.Count != 0)
		{
			CombineInstance[] array = new CombineInstance[m_childMeshFilters.Count];
			for (int i = 0; i < m_childMeshFilters.Count; i++)
			{
				array[i].mesh = m_childMeshFilters[i].sharedMesh;
				array[i].transform = m_childMeshFilters[i].transform.localToWorldMatrix;
			}
			m_combinedMesh.mesh.CombineMeshes(array);
		}
	}
}
