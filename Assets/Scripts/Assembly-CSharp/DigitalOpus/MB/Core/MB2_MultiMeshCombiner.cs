using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB2_MultiMeshCombiner
	{
		[Serializable]
		public class CombinedMesh
		{
			public MB2_MeshCombiner combinedMesh;

			public int extraSpace = -1;

			public int numVertsInListToDelete;

			public int numVertsInListToAdd;

			public List<GameObject> gosToAdd;

			public List<GameObject> gosToDelete;

			public List<GameObject> gosToUpdate;

			public bool isDirty;

			public CombinedMesh(int maxNumVertsInMesh)
			{
				combinedMesh = new MB2_MeshCombiner();
				extraSpace = maxNumVertsInMesh;
				numVertsInListToDelete = 0;
				numVertsInListToAdd = 0;
				gosToAdd = new List<GameObject>();
				gosToDelete = new List<GameObject>();
				gosToUpdate = new List<GameObject>();
			}

			public bool isEmpty()
			{
				List<GameObject> list = new List<GameObject>();
				list.AddRange(combinedMesh.GetObjectsInCombined());
				for (int i = 0; i < gosToDelete.Count; i++)
				{
					list.Remove(gosToDelete[i]);
				}
				if (list.Count == 0)
				{
					return true;
				}
				return false;
			}
		}

		private static bool VERBOSE = false;

		private static GameObject[] empty = new GameObject[0];

		private Dictionary<GameObject, CombinedMesh> obj2MeshCombinerMap = new Dictionary<GameObject, CombinedMesh>();

		private List<CombinedMesh> meshCombiners = new List<CombinedMesh>();

		[SerializeField]
		private int _maxVertsInMesh = 65535;

		[SerializeField]
		private string __name;

		[SerializeField]
		private MB2_TextureBakeResults __textureBakeResults;

		[SerializeField]
		private GameObject __resultSceneObject;

		[SerializeField]
		private MB_RenderType __renderType;

		[SerializeField]
		private MB2_OutputOptions __outputOption;

		[SerializeField]
		private MB2_LightmapOptions __lightmapOption;

		[SerializeField]
		private bool __doNorm;

		[SerializeField]
		private bool __doTan;

		[SerializeField]
		private bool __doCol;

		[SerializeField]
		private bool __doUV;

		[SerializeField]
		private bool __doUV1;

		public int maxVertsInMesh
		{
			get
			{
				return _maxVertsInMesh;
			}
			set
			{
				if (obj2MeshCombinerMap.Count > 0)
				{
					Debug.LogError("Can't set the max verts in meshes once there are objects in the mesh.");
				}
				else
				{
					_maxVertsInMesh = value;
				}
			}
		}

		public string name
		{
			get
			{
				return __name;
			}
			set
			{
				__name = value;
			}
		}

		public MB2_TextureBakeResults textureBakeResults
		{
			get
			{
				return __textureBakeResults;
			}
			set
			{
				__textureBakeResults = value;
			}
		}

		public GameObject resultSceneObject
		{
			get
			{
				return __resultSceneObject;
			}
			set
			{
				__resultSceneObject = value;
			}
		}

		public MB_RenderType renderType
		{
			get
			{
				return __renderType;
			}
			set
			{
				__renderType = value;
			}
		}

		public MB2_OutputOptions outputOption
		{
			get
			{
				return __outputOption;
			}
			set
			{
				__outputOption = value;
			}
		}

		public MB2_LightmapOptions lightmapOption
		{
			get
			{
				return __lightmapOption;
			}
			set
			{
				if (obj2MeshCombinerMap.Count > 0 && __lightmapOption != value)
				{
					Debug.LogWarning("Can't change lightmap option once objects are in the combined mesh.");
				}
				__lightmapOption = value;
			}
		}

		public bool doNorm
		{
			get
			{
				return __doNorm;
			}
			set
			{
				__doNorm = value;
			}
		}

		public bool doTan
		{
			get
			{
				return __doTan;
			}
			set
			{
				__doTan = value;
			}
		}

		public bool doCol
		{
			get
			{
				return __doCol;
			}
			set
			{
				__doCol = value;
			}
		}

		public bool doUV
		{
			get
			{
				return __doUV;
			}
			set
			{
				__doUV = value;
			}
		}

		public bool doUV1
		{
			get
			{
				return __doUV1;
			}
			set
			{
				__doUV1 = value;
			}
		}

		public int GetNumObjectsInCombined()
		{
			return obj2MeshCombinerMap.Count;
		}

		public int GetNumVerticesFor(GameObject go)
		{
			CombinedMesh value = null;
			if (obj2MeshCombinerMap.TryGetValue(go, out value))
			{
				return value.combinedMesh.GetNumVerticesFor(go);
			}
			return -1;
		}

		public List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				list.AddRange(meshCombiners[i].combinedMesh.GetObjectsInCombined());
			}
			return list;
		}

		public int GetLightmapIndex()
		{
			if (meshCombiners.Count > 0)
			{
				return meshCombiners[0].combinedMesh.GetLightmapIndex();
			}
			return -1;
		}

		public bool CombinedMeshContains(GameObject go)
		{
			return obj2MeshCombinerMap.ContainsKey(go);
		}

		private bool _validateTextureBakeResults()
		{
			if (textureBakeResults == null)
			{
				Debug.LogError("Material Bake Results is null. Can't combine meshes.");
				return false;
			}
			if (textureBakeResults.materials == null || textureBakeResults.materials.Length == 0)
			{
				Debug.LogError("Material Bake Results has no materials in material to uvRect map. Try baking materials. Can't combine meshes.");
				return false;
			}
			if (textureBakeResults.doMultiMaterial)
			{
				if (textureBakeResults.resultMaterials == null || textureBakeResults.resultMaterials.Length == 0)
				{
					Debug.LogError("Material Bake Results has no result materials. Try baking materials. Can't combine meshes.");
					return false;
				}
			}
			else if (textureBakeResults.resultMaterial == null)
			{
				Debug.LogError("Material Bake Results has no result material. Try baking materials. Can't combine meshes.");
				return false;
			}
			return true;
		}

		public void Apply()
		{
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				if (meshCombiners[i].isDirty)
				{
					meshCombiners[i].combinedMesh.Apply();
					meshCombiners[i].isDirty = false;
				}
			}
		}

		public void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool colors, bool uv1, bool uv2, bool bones = false)
		{
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				if (meshCombiners[i].isDirty)
				{
					meshCombiners[i].combinedMesh.Apply(triangles, vertices, normals, tangents, uvs, colors, uv1, uv2, bones);
					meshCombiners[i].isDirty = false;
				}
			}
		}

		public void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true)
		{
			if (gos == null)
			{
				Debug.LogError("list of game objects cannot be null");
				return;
			}
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				meshCombiners[i].gosToUpdate.Clear();
			}
			for (int j = 0; j < gos.Length; j++)
			{
				CombinedMesh value = null;
				obj2MeshCombinerMap.TryGetValue(gos[j], out value);
				if (value != null)
				{
					value.gosToUpdate.Add(gos[j]);
				}
				else
				{
					Debug.LogWarning(string.Concat("Object ", gos[j], " is not in the combined mesh."));
				}
			}
			for (int k = 0; k < meshCombiners.Count; k++)
			{
				if (meshCombiners[k].gosToUpdate.Count > 0)
				{
					GameObject[] gos2 = meshCombiners[k].gosToUpdate.ToArray();
					meshCombiners[k].combinedMesh.UpdateGameObjects(gos2, recalcBounds);
				}
			}
		}

		public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs)
		{
			return AddDeleteGameObjects(gos, deleteGOs, true, textureBakeResults.fixOutOfBoundsUVs);
		}

		public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource)
		{
			return AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource, textureBakeResults.fixOutOfBoundsUVs);
		}

		public Mesh AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs)
		{
			if (!_validate(gos, deleteGOs))
			{
				return null;
			}
			if (VERBOSE)
			{
				Debug.Log("AddDeleteGameObjects adding:" + gos.Length + " deleting:" + deleteGOs.Length + " disableRendererInSource:" + disableRendererInSource + " fixOutOfBoundUVs:" + fixOutOfBoundUVs);
			}
			_distributeAmongBakers(gos, deleteGOs);
			return _bakeStep1(gos, deleteGOs, disableRendererInSource, fixOutOfBoundUVs);
		}

		private bool _validate(GameObject[] gos, GameObject[] deleteGOs)
		{
			_validateTextureBakeResults();
			if (gos != null)
			{
				for (int i = 0; i < gos.Length; i++)
				{
					if (gos[i] == null)
					{
						Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
						return false;
					}
					for (int j = i + 1; j < gos.Length; j++)
					{
						if (gos[i] == gos[j])
						{
							Debug.LogError(string.Concat("GameObject ", gos[i], "appears twice in list of game objects to add"));
							return false;
						}
					}
					if (!obj2MeshCombinerMap.ContainsKey(gos[i]))
					{
						continue;
					}
					bool flag = false;
					if (deleteGOs != null)
					{
						for (int k = 0; k < deleteGOs.Length; k++)
						{
							if (deleteGOs[k] == gos[i])
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						Debug.LogError(string.Concat("GameObject ", gos[i], " is already in the combined mesh"));
						return false;
					}
				}
			}
			if (deleteGOs != null)
			{
				for (int l = 0; l < deleteGOs.Length; l++)
				{
					for (int m = l + 1; m < deleteGOs.Length; m++)
					{
						if (deleteGOs[l] == deleteGOs[m] && deleteGOs[l] != null)
						{
							Debug.LogError(string.Concat("GameObject ", deleteGOs[l], "appears twice in list of game objects to delete"));
							return false;
						}
					}
					if (!obj2MeshCombinerMap.ContainsKey(deleteGOs[l]))
					{
						Debug.LogWarning(string.Concat("GameObject ", deleteGOs[l], " on the list of objects to delete is not in the combined mesh."));
					}
				}
			}
			return true;
		}

		private void _distributeAmongBakers(GameObject[] gos, GameObject[] deleteGOs)
		{
			if (gos == null)
			{
				gos = empty;
			}
			if (deleteGOs == null)
			{
				deleteGOs = empty;
			}
			if (resultSceneObject == null)
			{
				resultSceneObject = new GameObject("CombinedMesh-" + name);
			}
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				meshCombiners[i].extraSpace = _maxVertsInMesh - meshCombiners[i].combinedMesh.GetMesh().vertexCount;
			}
			for (int j = 0; j < deleteGOs.Length; j++)
			{
				CombinedMesh value = null;
				if (obj2MeshCombinerMap.TryGetValue(deleteGOs[j], out value))
				{
					if (VERBOSE)
					{
						Debug.Log(string.Concat("Removing ", deleteGOs[j], " from meshCombiner ", meshCombiners.IndexOf(value)));
					}
					value.numVertsInListToDelete += value.combinedMesh.GetNumVerticesFor(deleteGOs[j]);
					value.gosToDelete.Add(deleteGOs[j]);
				}
				else
				{
					Debug.LogWarning(string.Concat("Object ", deleteGOs[j], " in the list of objects to delete is not in the combined mesh."));
				}
			}
			for (int k = 0; k < gos.Length; k++)
			{
				GameObject gameObject = gos[k];
				int vertexCount = MB_Utility.GetMesh(gameObject).vertexCount;
				CombinedMesh combinedMesh = null;
				for (int l = 0; l < meshCombiners.Count; l++)
				{
					if (meshCombiners[l].extraSpace + meshCombiners[l].numVertsInListToDelete - meshCombiners[l].numVertsInListToAdd > vertexCount)
					{
						combinedMesh = meshCombiners[l];
						if (VERBOSE)
						{
							Debug.Log(string.Concat("Added ", gos[k], " to combinedMesh ", l));
						}
						break;
					}
				}
				if (combinedMesh == null)
				{
					combinedMesh = new CombinedMesh(maxVertsInMesh);
					_setMBValues(combinedMesh.combinedMesh);
					meshCombiners.Add(combinedMesh);
					if (VERBOSE)
					{
						Debug.Log("Created new combinedMesh");
					}
				}
				combinedMesh.gosToAdd.Add(gameObject);
				combinedMesh.numVertsInListToAdd += vertexCount;
			}
		}

		private Mesh _bakeStep1(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource, bool fixOutOfBoundUVs)
		{
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				CombinedMesh combinedMesh = meshCombiners[i];
				if (combinedMesh.combinedMesh.targetRenderer == null)
				{
					GameObject gameObject = combinedMesh.combinedMesh.buildSceneMeshObject(resultSceneObject, combinedMesh.combinedMesh.GetMesh(), true);
					combinedMesh.combinedMesh.targetRenderer = gameObject.GetComponent<Renderer>();
				}
				else if (combinedMesh.combinedMesh.targetRenderer.transform.parent != resultSceneObject.transform)
				{
					Debug.LogError("targetRender objects must be children of resultSceneObject");
					return null;
				}
				if (combinedMesh.gosToAdd.Count > 0 || combinedMesh.gosToDelete.Count > 0)
				{
					combinedMesh.combinedMesh.AddDeleteGameObjects(combinedMesh.gosToAdd.ToArray(), combinedMesh.gosToDelete.ToArray(), disableRendererInSource, textureBakeResults.fixOutOfBoundsUVs);
				}
				Renderer targetRenderer = combinedMesh.combinedMesh.targetRenderer;
				if (targetRenderer is MeshRenderer)
				{
					MeshFilter component = targetRenderer.gameObject.GetComponent<MeshFilter>();
					component.sharedMesh = combinedMesh.combinedMesh.GetMesh();
				}
				else
				{
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
					skinnedMeshRenderer.sharedMesh = combinedMesh.combinedMesh.GetMesh();
				}
			}
			for (int j = 0; j < meshCombiners.Count; j++)
			{
				CombinedMesh combinedMesh2 = meshCombiners[j];
				for (int k = 0; k < combinedMesh2.gosToDelete.Count; k++)
				{
					obj2MeshCombinerMap.Remove(combinedMesh2.gosToDelete[k]);
					if (combinedMesh2.gosToDelete[k] != null)
					{
					}
				}
			}
			for (int l = 0; l < meshCombiners.Count; l++)
			{
				CombinedMesh combinedMesh3 = meshCombiners[l];
				for (int m = 0; m < combinedMesh3.gosToAdd.Count; m++)
				{
					obj2MeshCombinerMap.Add(combinedMesh3.gosToAdd[m], combinedMesh3);
				}
				if (combinedMesh3.gosToAdd.Count > 0 || combinedMesh3.gosToDelete.Count > 0)
				{
					combinedMesh3.gosToDelete.Clear();
					combinedMesh3.gosToAdd.Clear();
					combinedMesh3.numVertsInListToDelete = 0;
					combinedMesh3.numVertsInListToAdd = 0;
					combinedMesh3.isDirty = true;
				}
			}
			if (VERBOSE)
			{
				string text = "Meshes in combined:";
				for (int n = 0; n < meshCombiners.Count; n++)
				{
					string text2 = text;
					text = text2 + " mesh" + n + "(" + meshCombiners[n].combinedMesh.GetObjectsInCombined().Count + ")\n";
				}
				text = text + "children in result: " + resultSceneObject.transform.childCount;
				Debug.Log(text);
			}
			if (meshCombiners.Count > 0)
			{
				return meshCombiners[0].combinedMesh.GetMesh();
			}
			return null;
		}

		public void ClearMesh()
		{
			DestroyMesh();
		}

		public void DestroyMesh()
		{
			for (int i = 0; i < meshCombiners.Count; i++)
			{
				if (meshCombiners[i].combinedMesh.targetRenderer != null)
				{
					UnityEngine.Object.Destroy(meshCombiners[i].combinedMesh.targetRenderer.gameObject);
				}
				meshCombiners[i].combinedMesh.ClearMesh();
			}
			obj2MeshCombinerMap.Clear();
			meshCombiners.Clear();
		}

		private void _setMBValues(MB2_MeshCombiner targ)
		{
			targ.renderType = renderType;
			targ.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
			targ.lightmapOption = lightmapOption;
			targ.textureBakeResults = textureBakeResults;
			targ.doNorm = doNorm;
			targ.doTan = doTan;
			targ.doCol = doCol;
			targ.doUV = doUV;
			targ.doUV1 = doUV1;
		}

		public void SaveMeshsToAssetDatabase(string folderPath, string newFileNameBase)
		{
		}

		public void RebuildPrefab(GameObject prefabRoot)
		{
		}
	}
}
