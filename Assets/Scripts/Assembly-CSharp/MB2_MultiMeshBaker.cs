using DigitalOpus.MB.Core;
using UnityEngine;

public class MB2_MultiMeshBaker : MB2_MeshBakerCommon
{
	[HideInInspector]
	public MB2_MultiMeshCombiner meshCombiner = new MB2_MultiMeshCombiner();

	public override void ClearMesh()
	{
		_update_MB2_MeshCombiner();
		meshCombiner.ClearMesh();
	}

	public override void DestroyMesh()
	{
		_update_MB2_MeshCombiner();
		meshCombiner.DestroyMesh();
	}

	public override int GetNumObjectsInCombined()
	{
		return meshCombiner.GetNumObjectsInCombined();
	}

	public override int GetNumVerticesFor(GameObject go)
	{
		return meshCombiner.GetNumVerticesFor(go);
	}

	public override Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs)
	{
		if (resultSceneObject == null)
		{
			resultSceneObject = new GameObject("CombinedMesh-" + base.name);
		}
		_update_MB2_MeshCombiner();
		return meshCombiner.AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource, fixOutOfBoundUVs);
	}

	public override bool CombinedMeshContains(GameObject go)
	{
		return meshCombiner.CombinedMeshContains(go);
	}

	public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true)
	{
		_update_MB2_MeshCombiner();
		meshCombiner.UpdateGameObjects(gos, recalcBounds);
	}

	public override void Apply()
	{
		_update_MB2_MeshCombiner();
		meshCombiner.Apply();
	}

	public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool colors, bool uv1, bool uv2, bool bones = false)
	{
		_update_MB2_MeshCombiner();
		meshCombiner.Apply(triangles, vertices, normals, tangents, uvs, colors, uv1, uv2, bones);
	}

	public override void SaveMeshsToAssetDatabase(string folderPath, string newFileNameBase)
	{
		meshCombiner.SaveMeshsToAssetDatabase(folderPath, newFileNameBase);
	}

	public override void RebuildPrefab()
	{
		if (renderType == MB_RenderType.skinnedMeshRenderer)
		{
			Debug.LogWarning("Prefab will not be updated for skinned mesh. This is because all bones need to be included in the prefab for it to be usefull.");
		}
		else
		{
			meshCombiner.RebuildPrefab(resultPrefab);
		}
	}

	private void _update_MB2_MeshCombiner()
	{
		meshCombiner.name = base.name;
		meshCombiner.textureBakeResults = textureBakeResults;
		meshCombiner.resultSceneObject = resultSceneObject;
		meshCombiner.renderType = renderType;
		meshCombiner.outputOption = outputOption;
		meshCombiner.lightmapOption = lightmapOption;
		meshCombiner.doNorm = doNorm;
		meshCombiner.doTan = doTan;
		meshCombiner.doCol = doCol;
		meshCombiner.doUV = doUV;
		meshCombiner.doUV1 = doUV1;
	}
}
