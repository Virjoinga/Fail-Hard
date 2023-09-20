using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

public abstract class MB2_MeshBakerCommon : MB2_MeshBakerRoot
{
	public List<GameObject> objsToMesh;

	public bool useObjsToMeshFromTexBaker = true;

	[HideInInspector]
	public GameObject resultPrefab;

	[HideInInspector]
	public GameObject resultSceneObject;

	[HideInInspector]
	public MB_RenderType renderType;

	[HideInInspector]
	public MB2_OutputOptions outputOption;

	[HideInInspector]
	public MB2_LightmapOptions lightmapOption = MB2_LightmapOptions.ignore_UV2;

	[HideInInspector]
	public bool doNorm = true;

	[HideInInspector]
	public bool doTan = true;

	[HideInInspector]
	public bool doCol;

	[HideInInspector]
	public bool doUV = true;

	[HideInInspector]
	public bool doUV1;

	public override List<GameObject> GetObjectsToCombine()
	{
		if (objsToMesh == null)
		{
			objsToMesh = new List<GameObject>();
		}
		return objsToMesh;
	}

	public void EnableDisableSourceObjectRenderers(bool show)
	{
		for (int i = 0; i < objsToMesh.Count; i++)
		{
			GameObject gameObject = objsToMesh[i];
			if (gameObject != null)
			{
				Renderer renderer = MB_Utility.GetRenderer(gameObject);
				if (renderer != null)
				{
					renderer.enabled = show;
				}
			}
		}
	}

	public abstract void ClearMesh();

	public abstract void DestroyMesh();

	public abstract int GetNumObjectsInCombined();

	public abstract int GetNumVerticesFor(GameObject go);

	public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs)
	{
		if (textureBakeResults == null)
		{
			Debug.LogError("Material Bake Results is not set.");
			return null;
		}
		return AddDeleteGameObjects(gos, deleteGOs, true, textureBakeResults.fixOutOfBoundsUVs);
	}

	public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource)
	{
		if (textureBakeResults == null)
		{
			Debug.LogError("Material Bake Results is not set.");
			return null;
		}
		return AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource, textureBakeResults.fixOutOfBoundsUVs);
	}

	public abstract Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs);

	public abstract bool CombinedMeshContains(GameObject go);

	public abstract void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true);

	public abstract void Apply();

	public abstract void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool colors, bool uv1, bool uv2, bool bones = false);

	public abstract void SaveMeshsToAssetDatabase(string folderPath, string newFileNameBase);

	public abstract void RebuildPrefab();
}
