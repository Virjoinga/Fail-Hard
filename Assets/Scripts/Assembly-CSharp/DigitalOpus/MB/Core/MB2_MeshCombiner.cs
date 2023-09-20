using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB2_MeshCombiner
	{
		[Serializable]
		private class MB_DynamicGameObject : IComparable<MB_DynamicGameObject>
		{
			public GameObject go;

			public int vertIdx;

			public int numVerts;

			public int bonesIdx;

			public int numBones;

			public int lightmapIndex = -1;

			public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

			public int[] submeshTriIdxs;

			public int[] submeshNumTris;

			public int[] targetSubmeshIdxs;

			public Rect[] uvRects;

			public Rect[] obUVRects;

			public List<int>[] _submeshTris;

			public Mesh sharedMesh;

			public bool _beingDeleted;

			public int _triangleIdxAdjustment;

			public int CompareTo(MB_DynamicGameObject b)
			{
				return vertIdx - b.vertIdx;
			}
		}

		private static bool VERBOSE;

		[SerializeField]
		private bool _doNorm = true;

		[SerializeField]
		private bool _doTan = true;

		[SerializeField]
		private bool _doCol = true;

		[SerializeField]
		private bool _doUV = true;

		[SerializeField]
		private bool _doUV1 = true;

		[SerializeField]
		private int lightmapIndex = -1;

		[SerializeField]
		private List<GameObject> objectsInCombinedMesh = new List<GameObject>();

		[SerializeField]
		private List<MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB_DynamicGameObject>();

		private Dictionary<GameObject, MB_DynamicGameObject> _instance2combined_map = new Dictionary<GameObject, MB_DynamicGameObject>();

		[SerializeField]
		private Vector3[] verts = new Vector3[0];

		[SerializeField]
		private Vector3[] normals = new Vector3[0];

		[SerializeField]
		private Vector4[] tangents = new Vector4[0];

		[SerializeField]
		private Vector2[] uvs = new Vector2[0];

		[SerializeField]
		private Vector2[] uv1s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv2s = new Vector2[0];

		[SerializeField]
		private Color[] colors = new Color[0];

		[SerializeField]
		private Matrix4x4[] bindPoses = new Matrix4x4[0];

		[SerializeField]
		private Transform[] bones = new Transform[0];

		[SerializeField]
		private Mesh _mesh;

		private int[][] submeshTris = new int[0][];

		private BoneWeight[] boneWeights = new BoneWeight[0];

		private GameObject[] empty = new GameObject[0];

		[SerializeField]
		private string __name;

		[SerializeField]
		private MB2_TextureBakeResults __textureBakeResults;

		[SerializeField]
		private Renderer __targetRenderer;

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

		private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);

		public static bool EVAL_VERSION
		{
			get
			{
				return false;
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
				if (objectsInCombinedMesh.Count > 0 && __textureBakeResults != value && __textureBakeResults != null)
				{
					Debug.LogWarning("If material bake result is changed then objects currently in combined mesh may be invalid.");
				}
				__textureBakeResults = value;
			}
		}

		public Renderer targetRenderer
		{
			get
			{
				return __targetRenderer;
			}
			set
			{
				__targetRenderer = value;
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
				if (value == MB_RenderType.skinnedMeshRenderer && __renderType == MB_RenderType.meshRenderer && boneWeights.Length != verts.Length)
				{
					Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleteing the CombinedMesh scene object.");
				}
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
				if (objectsInCombinedMesh.Count > 0 && __lightmapOption != value)
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

		private MB_DynamicGameObject instance2Combined_MapGet(GameObject key)
		{
			return _instance2combined_map[key];
		}

		private void instance2Combined_MapAdd(GameObject key, MB_DynamicGameObject dgo)
		{
			_instance2combined_map.Add(key, dgo);
		}

		private void instance2Combined_MapRemove(GameObject key)
		{
			_instance2combined_map.Remove(key);
		}

		private bool instance2Combined_MapTryGetValue(GameObject key, out MB_DynamicGameObject dgo)
		{
			return _instance2combined_map.TryGetValue(key, out dgo);
		}

		private int instance2Combined_MapCount()
		{
			return _instance2combined_map.Count;
		}

		private void instance2Combined_MapClear()
		{
			_instance2combined_map.Clear();
		}

		private bool instance2Combined_MapContainsKey(GameObject key)
		{
			return _instance2combined_map.ContainsKey(key);
		}

		public bool doUV2()
		{
			return lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping;
		}

		public int GetNumObjectsInCombined()
		{
			return objectsInCombinedMesh.Count;
		}

		public List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			list.AddRange(objectsInCombinedMesh);
			return list;
		}

		public Mesh GetMesh()
		{
			if (_mesh == null)
			{
				_mesh = new Mesh();
			}
			return _mesh;
		}

		public Transform[] GetBones()
		{
			return bones;
		}

		public int GetLightmapIndex()
		{
			if (lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				return lightmapIndex;
			}
			return -1;
		}

		public int GetNumVerticesFor(GameObject go)
		{
			MB_DynamicGameObject dgo = null;
			if (instance2Combined_MapTryGetValue(go, out dgo))
			{
				return dgo.numVerts;
			}
			return -1;
		}

		private void _initialize()
		{
			if (objectsInCombinedMesh.Count == 0)
			{
				lightmapIndex = -1;
			}
			if (_mesh == null)
			{
				if (VERBOSE)
				{
					Debug.Log("_initialize Creating new Mesh");
				}
				_mesh = new Mesh();
			}
			if (instance2Combined_MapCount() != objectsInCombinedMesh.Count)
			{
				instance2Combined_MapClear();
				for (int i = 0; i < objectsInCombinedMesh.Count; i++)
				{
					instance2Combined_MapAdd(objectsInCombinedMesh[i], mbDynamicObjectsInCombinedMesh[i]);
				}
				boneWeights = _mesh.boneWeights;
				submeshTris = new int[_mesh.subMeshCount][];
				for (int j = 0; j < submeshTris.Length; j++)
				{
					submeshTris[j] = _mesh.GetTriangles(j);
				}
			}
		}

		private bool _collectMaterialTriangles(Mesh m, MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
		{
			int num = m.subMeshCount;
			if (sharedMaterials.Length < num)
			{
				num = sharedMaterials.Length;
			}
			dgo._submeshTris = new List<int>[num];
			dgo.targetSubmeshIdxs = new int[num];
			for (int i = 0; i < num; i++)
			{
				dgo._submeshTris[i] = new List<int>();
				if (textureBakeResults.doMultiMaterial)
				{
					if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[i]))
					{
						Debug.LogError(string.Concat("Object ", dgo.go, " has a material that was not found in the result materials maping. ", sharedMaterials[i]));
						return false;
					}
					dgo.targetSubmeshIdxs[i] = (int)sourceMats2submeshIdx_map[sharedMaterials[i]];
				}
				else
				{
					dgo.targetSubmeshIdxs[i] = 0;
				}
				List<int> list = dgo._submeshTris[i];
				int[] triangles = m.GetTriangles(i);
				for (int j = 0; j < triangles.Length; j++)
				{
					list.Add(triangles[j]);
				}
				if (VERBOSE)
				{
					Debug.Log("Collecting triangles for: " + dgo.go.name + " submesh:" + i + " maps to submesh:" + dgo.targetSubmeshIdxs[i] + " added:" + triangles.Length);
				}
			}
			return true;
		}

		private bool _collectOutOfBoundsUVRects2(Mesh m, MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
		{
			if (textureBakeResults == null)
			{
				Debug.LogError("Need to bake textures into combined material");
				return false;
			}
			int num = m.subMeshCount;
			if (sharedMaterials.Length < num)
			{
				num = sharedMaterials.Length;
			}
			dgo.obUVRects = new Rect[num];
			for (int i = 0; i < dgo.obUVRects.Length; i++)
			{
				dgo.obUVRects[i] = new Rect(0f, 0f, 1f, 1f);
			}
			for (int j = 0; j < num; j++)
			{
				Rect uvBounds = default(Rect);
				MB_Utility.hasOutOfBoundsUVs(m, ref uvBounds, j);
				dgo.obUVRects[j] = uvBounds;
			}
			return true;
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

		private bool _validateMeshFlags()
		{
			if (objectsInCombinedMesh.Count > 0 && ((!_doNorm && doNorm) || (!_doTan && doTan) || (!_doCol && doCol) || (!_doUV && doUV) || (!_doUV1 && doUV1)))
			{
				Debug.LogError("The channels have changed. There are already objects in the combined mesh that were added with a different set of channels.");
				return false;
			}
			_doNorm = doNorm;
			_doTan = doTan;
			_doCol = doCol;
			_doUV = doUV;
			_doUV1 = doUV1;
			return true;
		}

		private bool getIsGameObjectActive(GameObject g)
		{
			return g.activeInHierarchy;
		}

		private bool _addToCombined(GameObject[] goToAdd, GameObject[] goToDelete, bool disableRendererInSource)
		{
			if (!_validateTextureBakeResults())
			{
				return false;
			}
			if (!_validateMeshFlags())
			{
				return false;
			}
			if (outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && renderType == MB_RenderType.skinnedMeshRenderer && (targetRenderer == null || !(__targetRenderer is SkinnedMeshRenderer)))
			{
				Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
				return false;
			}
			GameObject[] _goToAdd;
			if (goToAdd == null)
			{
				_goToAdd = empty;
			}
			else
			{
				_goToAdd = (GameObject[])goToAdd.Clone();
			}
			GameObject[] array = ((goToDelete != null) ? ((GameObject[])goToDelete.Clone()) : empty);
			if (_mesh == null)
			{
				DestroyMesh();
			}
			Dictionary<Material, Rect> mat2RectMap = textureBakeResults.GetMat2RectMap();
			_initialize();
			int num = 1;
			if (textureBakeResults.doMultiMaterial)
			{
				num = textureBakeResults.resultMaterials.Length;
			}
			if (VERBOSE)
			{
				Debug.Log("_addToCombined objs adding:" + _goToAdd.Length + " objs deleting:" + array.Length + " fixOutOfBounds:" + textureBakeResults.fixOutOfBoundsUVs + " doMultiMaterial:" + textureBakeResults.doMultiMaterial + " disableRenderersInSource:" + disableRendererInSource);
			}
			OrderedDictionary orderedDictionary = null;
			if (textureBakeResults.doMultiMaterial)
			{
				orderedDictionary = new OrderedDictionary();
				for (int j = 0; j < num; j++)
				{
					MB_MultiMaterial mB_MultiMaterial = textureBakeResults.resultMaterials[j];
					for (int k = 0; k < mB_MultiMaterial.sourceMaterials.Count; k++)
					{
						if (mB_MultiMaterial.sourceMaterials[k] == null)
						{
							Debug.LogError("Found null material in source materials for combined mesh materials " + j);
							return false;
						}
						if (!orderedDictionary.Contains(mB_MultiMaterial.sourceMaterials[k]))
						{
							orderedDictionary.Add(mB_MultiMaterial.sourceMaterials[k], j);
						}
					}
				}
			}
			if (submeshTris.Length != num)
			{
				submeshTris = new int[num][];
				for (int l = 0; l < submeshTris.Length; l++)
				{
					submeshTris[l] = new int[0];
				}
			}
			int num2 = 0;
			int num3 = 0;
			int[] array2 = new int[num];
			for (int m = 0; m < array.Length; m++)
			{
				MB_DynamicGameObject dgo;
				if (instance2Combined_MapTryGetValue(array[m], out dgo))
				{
					num2 += dgo.numVerts;
					if (renderType == MB_RenderType.skinnedMeshRenderer)
					{
						num3 += dgo.numBones;
					}
					for (int n = 0; n < dgo.submeshNumTris.Length; n++)
					{
						array2[n] += dgo.submeshNumTris[n];
					}
				}
				else
				{
					Debug.LogWarning("Trying to delete an object that is not in combined mesh");
				}
			}
			List<MB_DynamicGameObject> list = new List<MB_DynamicGameObject>();
			int num4 = 0;
			int num5 = 0;
			int[] array3 = new int[num];
			for (int i = 0; i < _goToAdd.Length; i++)
			{
				if (!instance2Combined_MapContainsKey(_goToAdd[i]) || (bool)Array.Find(array, (GameObject o) => o == _goToAdd[i]))
				{
					MB_DynamicGameObject mB_DynamicGameObject = new MB_DynamicGameObject();
					GameObject gameObject = _goToAdd[i];
					Material[] gOMaterials = MB_Utility.GetGOMaterials(gameObject);
					if (gOMaterials == null)
					{
						Debug.LogError("Object " + gameObject.name + " does not have a Renderer");
						_goToAdd[i] = null;
						return false;
					}
					Mesh mesh = MB_Utility.GetMesh(gameObject);
					if (mesh == null)
					{
						Debug.LogError("Object " + gameObject.name + " MeshFilter or SkinedMeshRenderer had no mesh");
						_goToAdd[i] = null;
						return false;
					}
					Rect[] array4 = new Rect[gOMaterials.Length];
					for (int num6 = 0; num6 < gOMaterials.Length; num6++)
					{
						if (!mat2RectMap.TryGetValue(gOMaterials[num6], out array4[num6]))
						{
							Debug.LogError(string.Concat("Object ", gameObject.name, " has an unknown material ", gOMaterials[num6], ". Try baking textures"));
							_goToAdd[i] = null;
						}
					}
					if (!(_goToAdd[i] != null))
					{
						continue;
					}
					mB_DynamicGameObject.go = _goToAdd[i];
					mB_DynamicGameObject.uvRects = array4;
					mB_DynamicGameObject.sharedMesh = mesh;
					mB_DynamicGameObject.numVerts = mesh.vertexCount;
					Renderer renderer = MB_Utility.GetRenderer(mB_DynamicGameObject.go);
					mB_DynamicGameObject.numBones = _getNumBones(renderer);
					if (lightmapIndex == -1)
					{
						lightmapIndex = renderer.lightmapIndex;
					}
					if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
					{
						if (lightmapIndex != renderer.lightmapIndex)
						{
							Debug.LogWarning("Object " + gameObject.name + " has a different lightmap index. Lightmapping will not work.");
						}
						if (!getIsGameObjectActive(mB_DynamicGameObject.go))
						{
							Debug.LogWarning("Object " + gameObject.name + " is inactive. Can only get lightmap index of active objects.");
						}
						if (renderer.lightmapIndex == -1)
						{
							Debug.LogWarning("Object " + gameObject.name + " does not have an index to a lightmap.");
						}
					}
					mB_DynamicGameObject.lightmapIndex = renderer.lightmapIndex;
					mB_DynamicGameObject.lightmapTilingOffset = renderer.lightmapTilingOffset;
					if (!_collectMaterialTriangles(mesh, mB_DynamicGameObject, gOMaterials, orderedDictionary))
					{
						return false;
					}
					mB_DynamicGameObject.submeshNumTris = new int[num];
					mB_DynamicGameObject.submeshTriIdxs = new int[num];
					if (textureBakeResults.fixOutOfBoundsUVs && !_collectOutOfBoundsUVRects2(mesh, mB_DynamicGameObject, gOMaterials, orderedDictionary))
					{
						return false;
					}
					list.Add(mB_DynamicGameObject);
					num4 += mB_DynamicGameObject.numVerts;
					if (renderType == MB_RenderType.skinnedMeshRenderer)
					{
						num5 += mB_DynamicGameObject.numBones;
					}
					for (int num7 = 0; num7 < mB_DynamicGameObject._submeshTris.Length; num7++)
					{
						array3[mB_DynamicGameObject.targetSubmeshIdxs[num7]] += mB_DynamicGameObject._submeshTris[num7].Count;
					}
				}
				else
				{
					Debug.LogWarning("Object " + _goToAdd[i].name + " has already been added");
					_goToAdd[i] = null;
				}
			}
			for (int num8 = 0; num8 < _goToAdd.Length; num8++)
			{
				if (_goToAdd[num8] != null && disableRendererInSource)
				{
					MB_Utility.DisableRendererInSource(_goToAdd[num8]);
				}
			}
			int num9 = verts.Length + num4 - num2;
			int num10 = bindPoses.Length + num5 - num3;
			int[] array5 = new int[num];
			if (VERBOSE)
			{
				Debug.Log("Verts adding:" + num4 + " deleting:" + num2);
			}
			if (VERBOSE)
			{
				Debug.Log("Submeshes:" + array5.Length);
			}
			for (int num11 = 0; num11 < array5.Length; num11++)
			{
				array5[num11] = submeshTris[num11].Length + array3[num11] - array2[num11];
				if (VERBOSE)
				{
					Debug.Log("    submesh :" + num11 + " already contains:" + submeshTris[num11].Length + " trisAdded:" + array3[num11] + " trisDeleted:" + array2[num11]);
				}
			}
			if (num9 > 65534)
			{
				Debug.LogError("Cannot add objects. Resulting mesh will have more than 64k vertices .");
				return false;
			}
			Vector3[] destinationArray = null;
			Vector4[] destinationArray2 = null;
			Vector2[] destinationArray3 = null;
			Vector2[] destinationArray4 = null;
			Vector2[] destinationArray5 = null;
			Color[] destinationArray6 = null;
			Vector3[] destinationArray7 = new Vector3[num9];
			if (_doNorm)
			{
				destinationArray = new Vector3[num9];
			}
			if (_doTan)
			{
				destinationArray2 = new Vector4[num9];
			}
			if (_doUV)
			{
				destinationArray3 = new Vector2[num9];
			}
			if (_doUV1)
			{
				destinationArray4 = new Vector2[num9];
			}
			if (lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				destinationArray5 = new Vector2[num9];
			}
			if (_doCol)
			{
				destinationArray6 = new Color[num9];
			}
			BoneWeight[] destinationArray8 = new BoneWeight[num9];
			Matrix4x4[] array6 = new Matrix4x4[num10];
			Transform[] destinationArray9 = new Transform[num10];
			int[][] array7 = null;
			array7 = new int[num][];
			for (int num12 = 0; num12 < array7.Length; num12++)
			{
				array7[num12] = new int[array5[num12]];
			}
			for (int num13 = 0; num13 < array.Length; num13++)
			{
				MB_DynamicGameObject dgo2 = null;
				if (instance2Combined_MapTryGetValue(array[num13], out dgo2))
				{
					dgo2._beingDeleted = true;
				}
			}
			mbDynamicObjectsInCombinedMesh.Sort();
			int num14 = 0;
			int num15 = 0;
			int[] array8 = new int[num];
			int num16 = 0;
			int num17 = 0;
			for (int num18 = 0; num18 < mbDynamicObjectsInCombinedMesh.Count; num18++)
			{
				MB_DynamicGameObject mB_DynamicGameObject2 = mbDynamicObjectsInCombinedMesh[num18];
				if (!mB_DynamicGameObject2._beingDeleted)
				{
					if (VERBOSE)
					{
						Debug.Log("Copying obj in combined arrays idx:" + num18);
					}
					Array.Copy(verts, mB_DynamicGameObject2.vertIdx, destinationArray7, num14, mB_DynamicGameObject2.numVerts);
					if (_doNorm)
					{
						Array.Copy(normals, mB_DynamicGameObject2.vertIdx, destinationArray, num14, mB_DynamicGameObject2.numVerts);
					}
					if (_doTan)
					{
						Array.Copy(tangents, mB_DynamicGameObject2.vertIdx, destinationArray2, num14, mB_DynamicGameObject2.numVerts);
					}
					if (_doUV)
					{
						Array.Copy(uvs, mB_DynamicGameObject2.vertIdx, destinationArray3, num14, mB_DynamicGameObject2.numVerts);
					}
					if (_doUV1)
					{
						Array.Copy(uv1s, mB_DynamicGameObject2.vertIdx, destinationArray4, num14, mB_DynamicGameObject2.numVerts);
					}
					if (doUV2())
					{
						Array.Copy(uv2s, mB_DynamicGameObject2.vertIdx, destinationArray5, num14, mB_DynamicGameObject2.numVerts);
					}
					if (_doCol)
					{
						Array.Copy(colors, mB_DynamicGameObject2.vertIdx, destinationArray6, num14, mB_DynamicGameObject2.numVerts);
					}
					if (renderType == MB_RenderType.skinnedMeshRenderer)
					{
						for (int num19 = mB_DynamicGameObject2.vertIdx; num19 < mB_DynamicGameObject2.vertIdx + mB_DynamicGameObject2.numVerts; num19++)
						{
							boneWeights[num19].boneIndex0 = boneWeights[num19].boneIndex0 - num17;
							boneWeights[num19].boneIndex1 = boneWeights[num19].boneIndex1 - num17;
							boneWeights[num19].boneIndex2 = boneWeights[num19].boneIndex2 - num17;
							boneWeights[num19].boneIndex3 = boneWeights[num19].boneIndex3 - num17;
						}
						Array.Copy(boneWeights, mB_DynamicGameObject2.vertIdx, destinationArray8, num14, mB_DynamicGameObject2.numVerts);
					}
					Array.Copy(bindPoses, mB_DynamicGameObject2.bonesIdx, array6, num15, mB_DynamicGameObject2.numBones);
					Array.Copy(bones, mB_DynamicGameObject2.bonesIdx, destinationArray9, num15, mB_DynamicGameObject2.numBones);
					for (int num20 = 0; num20 < num; num20++)
					{
						int[] array9 = submeshTris[num20];
						int num21 = mB_DynamicGameObject2.submeshTriIdxs[num20];
						int num22 = mB_DynamicGameObject2.submeshNumTris[num20];
						if (VERBOSE)
						{
							Debug.Log("    Adjusting submesh triangles submesh:" + num20 + " startIdx:" + num21 + " num:" + num22);
						}
						for (int num23 = num21; num23 < num21 + num22; num23++)
						{
							array9[num23] -= num16;
						}
						Array.Copy(array9, num21, array7[num20], array8[num20], num22);
					}
					mB_DynamicGameObject2.bonesIdx = num15;
					mB_DynamicGameObject2.vertIdx = num14;
					for (int num24 = 0; num24 < array8.Length; num24++)
					{
						mB_DynamicGameObject2.submeshTriIdxs[num24] = array8[num24];
						array8[num24] += mB_DynamicGameObject2.submeshNumTris[num24];
					}
					num15 += mB_DynamicGameObject2.numBones;
					num14 += mB_DynamicGameObject2.numVerts;
				}
				else
				{
					if (VERBOSE)
					{
						Debug.Log("Not copying obj: " + num18);
					}
					num16 += mB_DynamicGameObject2.numVerts;
					num17 += mB_DynamicGameObject2.numBones;
				}
			}
			for (int num25 = mbDynamicObjectsInCombinedMesh.Count - 1; num25 >= 0; num25--)
			{
				if (mbDynamicObjectsInCombinedMesh[num25]._beingDeleted)
				{
					instance2Combined_MapRemove(mbDynamicObjectsInCombinedMesh[num25].go);
					objectsInCombinedMesh.RemoveAt(num25);
					mbDynamicObjectsInCombinedMesh.RemoveAt(num25);
				}
			}
			verts = destinationArray7;
			if (_doNorm)
			{
				normals = destinationArray;
			}
			if (_doTan)
			{
				tangents = destinationArray2;
			}
			if (_doUV)
			{
				uvs = destinationArray3;
			}
			if (_doUV1)
			{
				uv1s = destinationArray4;
			}
			if (doUV2())
			{
				uv2s = destinationArray5;
			}
			if (_doCol)
			{
				colors = destinationArray6;
			}
			if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				boneWeights = destinationArray8;
			}
			bindPoses = array6;
			bones = destinationArray9;
			submeshTris = array7;
			Vector2 vector3 = default(Vector2);
			for (int num26 = 0; num26 < list.Count; num26++)
			{
				MB_DynamicGameObject mB_DynamicGameObject3 = list[num26];
				GameObject go = mB_DynamicGameObject3.go;
				int num27 = num14;
				int num28 = num15;
				Mesh sharedMesh = mB_DynamicGameObject3.sharedMesh;
				Matrix4x4 localToWorldMatrix = go.transform.localToWorldMatrix;
				Quaternion rotation = go.transform.rotation;
				destinationArray7 = sharedMesh.vertices;
				Vector3[] array10 = null;
				Vector4[] array11 = null;
				if (_doNorm)
				{
					array10 = _getMeshNormals(sharedMesh);
				}
				if (_doTan)
				{
					array11 = _getMeshTangents(sharedMesh);
				}
				if (renderType != MB_RenderType.skinnedMeshRenderer)
				{
					for (int num29 = 0; num29 < destinationArray7.Length; num29++)
					{
						destinationArray7[num29] = localToWorldMatrix.MultiplyPoint(destinationArray7[num29]);
						if (_doNorm)
						{
							array10[num29] = rotation * array10[num29];
						}
						if (_doTan)
						{
							float w = array11[num29].w;
							array11[num29] = rotation * array11[num29];
							array11[num29].w = w;
						}
					}
				}
				if (_doNorm)
				{
					array10.CopyTo(normals, num27);
				}
				if (_doTan)
				{
					array11.CopyTo(tangents, num27);
				}
				destinationArray7.CopyTo(verts, num27);
				int subMeshCount = sharedMesh.subMeshCount;
				if (mB_DynamicGameObject3.uvRects.Length < subMeshCount)
				{
					Debug.LogWarning("Mesh " + mB_DynamicGameObject3.go.name + " has more submeshes than materials");
					subMeshCount = mB_DynamicGameObject3.uvRects.Length;
				}
				else if (mB_DynamicGameObject3.uvRects.Length > subMeshCount)
				{
					Debug.LogWarning("Mesh " + mB_DynamicGameObject3.go.name + " has fewer submeshes than materials");
				}
				if (_doUV)
				{
					destinationArray3 = _getMeshUVs(sharedMesh);
					int[] array12 = new int[destinationArray3.Length];
					for (int num30 = 0; num30 < array12.Length; num30++)
					{
						array12[num30] = -1;
					}
					bool flag = false;
					Rect rect = default(Rect);
					for (int num31 = 0; num31 < mB_DynamicGameObject3._submeshTris.Length; num31++)
					{
						List<int> list2 = mB_DynamicGameObject3._submeshTris[num31];
						Rect rect2 = mB_DynamicGameObject3.uvRects[num31];
						if (textureBakeResults.fixOutOfBoundsUVs)
						{
							rect = mB_DynamicGameObject3.obUVRects[num31];
						}
						for (int num32 = 0; num32 < list2.Count; num32++)
						{
							int num33 = list2[num32];
							if (array12[num33] == -1)
							{
								array12[num33] = num31;
								if (textureBakeResults.fixOutOfBoundsUVs)
								{
									destinationArray3[num33].x = destinationArray3[num33].x / rect.width - rect.x / rect.width;
									destinationArray3[num33].y = destinationArray3[num33].y / rect.height - rect.y / rect.height;
								}
								destinationArray3[num33].x = rect2.x + destinationArray3[num33].x * rect2.width;
								destinationArray3[num33].y = rect2.y + destinationArray3[num33].y * rect2.height;
							}
							if (array12[num33] != num31)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						Debug.LogWarning(mB_DynamicGameObject3.go.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
					}
					destinationArray3.CopyTo(uvs, num27);
				}
				if (doUV2())
				{
					destinationArray5 = _getMeshUV2s(sharedMesh);
					if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
					{
						Vector4 lightmapTilingOffset = mB_DynamicGameObject3.lightmapTilingOffset;
						Vector2 vector = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
						Vector2 vector2 = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
						for (int num34 = 0; num34 < destinationArray5.Length; num34++)
						{
							vector3.x = vector.x * destinationArray5[num34].x;
							vector3.y = vector.y * destinationArray5[num34].y;
							destinationArray5[num34] = vector2 + vector3;
						}
					}
					destinationArray5.CopyTo(uv2s, num27);
				}
				if (_doUV1)
				{
					destinationArray4 = _getMeshUV1s(sharedMesh);
					destinationArray4.CopyTo(uv1s, num27);
				}
				if (_doCol)
				{
					destinationArray6 = _getMeshColors(sharedMesh);
					destinationArray6.CopyTo(colors, num27);
				}
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					Renderer renderer2 = MB_Utility.GetRenderer(mB_DynamicGameObject3.go);
					destinationArray9 = _getBones(renderer2);
					destinationArray9.CopyTo(bones, num28);
					array6 = _getBindPoses(renderer2);
					array6.CopyTo(bindPoses, num28);
					destinationArray8 = _getBoneWeights(renderer2, mB_DynamicGameObject3.numVerts);
					for (int num35 = 0; num35 < destinationArray8.Length; num35++)
					{
						destinationArray8[num35].boneIndex0 = destinationArray8[num35].boneIndex0 + num28;
						destinationArray8[num35].boneIndex1 = destinationArray8[num35].boneIndex1 + num28;
						destinationArray8[num35].boneIndex2 = destinationArray8[num35].boneIndex2 + num28;
						destinationArray8[num35].boneIndex3 = destinationArray8[num35].boneIndex3 + num28;
					}
					destinationArray8.CopyTo(boneWeights, num27);
				}
				for (int num36 = 0; num36 < array8.Length; num36++)
				{
					mB_DynamicGameObject3.submeshTriIdxs[num36] = array8[num36];
				}
				for (int num37 = 0; num37 < mB_DynamicGameObject3._submeshTris.Length; num37++)
				{
					List<int> list3 = mB_DynamicGameObject3._submeshTris[num37];
					for (int num38 = 0; num38 < list3.Count; num38++)
					{
						list3[num38] += num27;
					}
					int num39 = mB_DynamicGameObject3.targetSubmeshIdxs[num37];
					list3.CopyTo(submeshTris[num39], array8[num39]);
					mB_DynamicGameObject3.submeshNumTris[num39] += list3.Count;
					array8[num39] += list3.Count;
				}
				mB_DynamicGameObject3.vertIdx = num14;
				mB_DynamicGameObject3.bonesIdx = num15;
				instance2Combined_MapAdd(go, mB_DynamicGameObject3);
				objectsInCombinedMesh.Add(go);
				mbDynamicObjectsInCombinedMesh.Add(mB_DynamicGameObject3);
				num14 += destinationArray7.Length;
				num15 += array6.Length;
				for (int num40 = 0; num40 < mB_DynamicGameObject3._submeshTris.Length; num40++)
				{
					mB_DynamicGameObject3._submeshTris[num40].Clear();
				}
				mB_DynamicGameObject3._submeshTris = null;
				if (VERBOSE)
				{
					Debug.Log("Added to combined:" + mB_DynamicGameObject3.go.name + " verts:" + destinationArray7.Length);
				}
			}
			return true;
		}

		private Color[] _getMeshColors(Mesh m)
		{
			Color[] array = m.colors;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no colors. Generating"));
				}
				if (_doCol)
				{
					Debug.LogWarning(string.Concat("Mesh ", m, " didn't have colors. Generating an array of white colors"));
				}
				array = new Color[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Color.white;
				}
			}
			return array;
		}

		private Vector3[] _getMeshNormals(Mesh m)
		{
			Vector3[] array = m.normals;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no normals. Generating"));
				}
				Debug.LogWarning(string.Concat("Mesh ", m, " didn't have normals. Generating normals."));
				Mesh mesh = (Mesh)UnityEngine.Object.Instantiate(m);
				mesh.RecalculateNormals();
				array = mesh.normals;
				MB_Utility.Destroy(mesh);
			}
			return array;
		}

		private Vector4[] _getMeshTangents(Mesh m)
		{
			Vector4[] array = m.tangents;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no tangents. Generating"));
				}
				Debug.LogWarning(string.Concat("Mesh ", m, " didn't have tangents. Generating tangents."));
				Vector3[] vertices = m.vertices;
				Vector2[] array2 = _getMeshUVs(m);
				Vector3[] array3 = _getMeshNormals(m);
				array = new Vector4[m.vertexCount];
				for (int i = 0; i < m.subMeshCount; i++)
				{
					int[] triangles = m.GetTriangles(i);
					_generateTangents(triangles, vertices, array2, array3, array);
				}
			}
			return array;
		}

		private Vector2[] _getMeshUVs(Mesh m)
		{
			Vector2[] array = m.uv;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no uvs. Generating"));
				}
				Debug.LogWarning(string.Concat("Mesh ", m, " didn't have uvs. Generating uvs."));
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _HALF_UV;
				}
			}
			return array;
		}

		private Vector2[] _getMeshUV1s(Mesh m)
		{
			Vector2[] array = m.uv1;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no uv1s. Generating"));
				}
				Debug.LogWarning(string.Concat("Mesh ", m, " didn't have uv1s. Generating uv1s."));
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _HALF_UV;
				}
			}
			return array;
		}

		private Vector2[] _getMeshUV2s(Mesh m)
		{
			Vector2[] array = m.uv2;
			if (array.Length == 0)
			{
				if (VERBOSE)
				{
					Debug.Log(string.Concat("Mesh ", m, " has no uv2s. Generating"));
				}
				Debug.LogWarning(string.Concat("Mesh ", m, " didn't have uv2s. Lightmapping option was set to ", lightmapOption, " Generating uv2s."));
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _HALF_UV;
				}
			}
			return array;
		}

		public void UpdateSkinnedMeshApproximateBounds()
		{
			if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
				return;
			}
			if (bones.Length == 0)
			{
				Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
				return;
			}
			if (targetRenderer == null)
			{
				Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
				return;
			}
			if (!__targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
				return;
			}
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
			Vector3 position = bones[0].position;
			Vector3 position2 = bones[0].position;
			for (int i = 1; i < bones.Length; i++)
			{
				Vector3 position3 = bones[i].position;
				if (position3.x < position2.x)
				{
					position2.x = position3.x;
				}
				if (position3.y < position2.y)
				{
					position2.y = position3.y;
				}
				if (position3.z < position2.z)
				{
					position2.z = position3.z;
				}
				if (position3.x > position.x)
				{
					position.x = position3.x;
				}
				if (position3.y > position.y)
				{
					position.y = position3.y;
				}
				if (position3.z > position.z)
				{
					position.z = position3.z;
				}
			}
			Vector3 vector = (position + position2) / 2f;
			Vector3 vector2 = position - position2;
			Matrix4x4 worldToLocalMatrix = skinnedMeshRenderer.worldToLocalMatrix;
			Bounds localBounds = new Bounds(worldToLocalMatrix * vector, worldToLocalMatrix * vector2);
			skinnedMeshRenderer.localBounds = localBounds;
		}

		private int _getNumBones(Renderer r)
		{
			if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				if (r is SkinnedMeshRenderer)
				{
					return ((SkinnedMeshRenderer)r).bones.Length;
				}
				if (r is MeshRenderer)
				{
					return 1;
				}
				Debug.LogError("Could not _getNumBones. Object does not have a renderer");
				return 0;
			}
			return 0;
		}

		private Transform[] _getBones(Renderer r)
		{
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).bones;
			}
			if (r is MeshRenderer)
			{
				return new Transform[1] { r.transform };
			}
			Debug.LogError("Could not getBones. Object does not have a renderer");
			return null;
		}

		private Matrix4x4[] _getBindPoses(Renderer r)
		{
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
			}
			if (r is MeshRenderer)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				return new Matrix4x4[1] { identity };
			}
			Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
			return null;
		}

		private BoneWeight[] _getBoneWeights(Renderer r, int numVerts)
		{
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
			}
			if (r is MeshRenderer)
			{
				BoneWeight boneWeight = default(BoneWeight);
				int num2 = (boneWeight.boneIndex3 = 0);
				num2 = (boneWeight.boneIndex2 = num2);
				num2 = (boneWeight.boneIndex1 = num2);
				boneWeight.boneIndex0 = num2;
				boneWeight.weight0 = 1f;
				float num6 = (boneWeight.weight3 = 0f);
				num6 = (boneWeight.weight2 = num6);
				boneWeight.weight1 = num6;
				BoneWeight[] array = new BoneWeight[numVerts];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = boneWeight;
				}
				return array;
			}
			Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
			return null;
		}

		public void Apply()
		{
			bool flag = false;
			if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				flag = true;
			}
			Apply(true, true, _doNorm, _doTan, _doUV, _doCol, _doUV1, doUV2(), flag);
		}

		[Obsolete("ApplyAll is deprecated, please use Apply instead.")]
		public void ApplyAll()
		{
			Apply(true, true, _doNorm, _doTan, _doUV, _doCol, _doUV1, doUV2(), true);
		}

		public void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool colors, bool uv1, bool uv2, bool bones = false)
		{
			if (_mesh != null)
			{
				if (_mesh.vertexCount != verts.Length)
				{
					_mesh.Clear(false);
				}
				else if (triangles)
				{
					_mesh.Clear();
				}
				if (vertices)
				{
					_mesh.vertices = verts;
				}
				if (triangles)
				{
					_mesh.triangles = submeshTris[0];
					if (textureBakeResults.doMultiMaterial)
					{
						_mesh.subMeshCount = submeshTris.Length;
						for (int i = 0; i < submeshTris.Length; i++)
						{
							_mesh.SetTriangles(submeshTris[i], i);
						}
					}
				}
				if (normals)
				{
					if (_doNorm)
					{
						_mesh.normals = this.normals;
					}
					else
					{
						Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
					}
				}
				if (tangents)
				{
					if (_doTan)
					{
						_mesh.tangents = this.tangents;
					}
					else
					{
						Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents");
					}
				}
				if (uvs)
				{
					if (_doUV)
					{
						_mesh.uv = this.uvs;
					}
					else
					{
						Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs");
					}
				}
				if (colors)
				{
					if (_doCol)
					{
						_mesh.colors = this.colors;
					}
					else
					{
						Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors");
					}
				}
				if (uv1)
				{
					if (_doUV1)
					{
						_mesh.uv1 = uv1s;
					}
					else
					{
						Debug.LogError("uv1 flag was set in Apply but MeshBaker didn't generate uv1s");
					}
				}
				if (uv2)
				{
					if (doUV2())
					{
						_mesh.uv2 = uv2s;
					}
					else
					{
						Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + lightmapOption);
					}
				}
				bool flag = false;
				if (renderType != MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && !flag)
				{
					Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
				}
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (verts.Length == 0)
					{
						targetRenderer.enabled = false;
					}
					else
					{
						targetRenderer.enabled = true;
					}
				}
				if (bones)
				{
					_mesh.bindposes = bindPoses;
					_mesh.boneWeights = boneWeights;
				}
				if (triangles || vertices)
				{
					_mesh.RecalculateBounds();
				}
			}
			else
			{
				Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
			}
		}

		public void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true)
		{
			_updateGameObjects(gos, recalcBounds);
		}

		private void _updateGameObjects(GameObject[] gos, bool recalcBounds)
		{
			_initialize();
			for (int i = 0; i < gos.Length; i++)
			{
				_updateGameObject(gos[i], false);
			}
			if (recalcBounds)
			{
				_mesh.RecalculateBounds();
			}
		}

		private void _updateGameObject(GameObject go, bool recalcBounds)
		{
			MB_DynamicGameObject dgo = null;
			if (!instance2Combined_MapTryGetValue(go, out dgo))
			{
				Debug.LogError("Object " + go.name + " has not been added");
				return;
			}
			Mesh sharedMesh = dgo.sharedMesh;
			if (dgo.numVerts != sharedMesh.vertexCount)
			{
				Debug.LogError("Object " + go.name + " source mesh has been modified since being added");
				return;
			}
			Matrix4x4 localToWorldMatrix = go.transform.localToWorldMatrix;
			Quaternion rotation = go.transform.rotation;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3[] array = sharedMesh.normals;
			Vector4[] array2 = sharedMesh.tangents;
			for (int i = 0; i < vertices.Length; i++)
			{
				int num = dgo.vertIdx + i;
				verts[num] = localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
				if (_doNorm)
				{
					normals[num] = rotation * array[i];
				}
				if (_doTan)
				{
					float w = array2[i].w;
					tangents[num] = rotation * array2[i];
					tangents[num].w = w;
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
			if (gos != null)
			{
				for (int i = 0; i < gos.Length; i++)
				{
					if (gos[i] == null)
					{
						Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
						return null;
					}
					for (int j = i + 1; j < gos.Length; j++)
					{
						if (gos[i] == gos[j])
						{
							Debug.LogError(string.Concat("GameObject ", gos[i], "appears twice in list of game objects to add"));
							return null;
						}
					}
				}
			}
			if (deleteGOs != null)
			{
				for (int k = 0; k < deleteGOs.Length; k++)
				{
					for (int l = k + 1; l < deleteGOs.Length; l++)
					{
						if (deleteGOs[k] == deleteGOs[l] && deleteGOs[k] != null)
						{
							Debug.LogError(string.Concat("GameObject ", deleteGOs[k], "appears twice in list of game objects to delete"));
							return null;
						}
					}
				}
			}
			if (!_addToCombined(gos, deleteGOs, disableRendererInSource))
			{
				Debug.LogError("Failed to add/delete objects to combined mesh");
				return null;
			}
			if (targetRenderer != null && outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
					skinnedMeshRenderer.bones = bones;
					UpdateSkinnedMeshApproximateBounds();
				}
				targetRenderer.lightmapIndex = GetLightmapIndex();
			}
			return _mesh;
		}

		public bool CombinedMeshContains(GameObject go)
		{
			return objectsInCombinedMesh.Contains(go);
		}

		private void _clearArrays()
		{
			verts = new Vector3[0];
			normals = new Vector3[0];
			tangents = new Vector4[0];
			uvs = new Vector2[0];
			uv1s = new Vector2[0];
			uv2s = new Vector2[0];
			colors = new Color[0];
			bones = new Transform[0];
			bindPoses = new Matrix4x4[0];
			boneWeights = new BoneWeight[0];
			submeshTris = new int[0][];
			mbDynamicObjectsInCombinedMesh.Clear();
			objectsInCombinedMesh.Clear();
			instance2Combined_MapClear();
		}

		public void ClearMesh()
		{
			if (_mesh != null)
			{
				_mesh.Clear(false);
			}
			else
			{
				_mesh = new Mesh();
			}
			_clearArrays();
		}

		public void DestroyMesh()
		{
			if (_mesh != null)
			{
				if (VERBOSE)
				{
					Debug.Log("Destroying Mesh");
				}
				MB_Utility.Destroy(_mesh);
			}
			_mesh = new Mesh();
			_clearArrays();
		}

		private void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
		{
			int num = triangles.Length;
			int num2 = verts.Length;
			Vector3[] array = new Vector3[num2];
			Vector3[] array2 = new Vector3[num2];
			for (int i = 0; i < num; i += 3)
			{
				int num3 = triangles[i];
				int num4 = triangles[i + 1];
				int num5 = triangles[i + 2];
				Vector3 vector = verts[num3];
				Vector3 vector2 = verts[num4];
				Vector3 vector3 = verts[num5];
				Vector2 vector4 = uvs[num3];
				Vector2 vector5 = uvs[num4];
				Vector2 vector6 = uvs[num5];
				float num6 = vector2.x - vector.x;
				float num7 = vector3.x - vector.x;
				float num8 = vector2.y - vector.y;
				float num9 = vector3.y - vector.y;
				float num10 = vector2.z - vector.z;
				float num11 = vector3.z - vector.z;
				float num12 = vector5.x - vector4.x;
				float num13 = vector6.x - vector4.x;
				float num14 = vector5.y - vector4.y;
				float num15 = vector6.y - vector4.y;
				float num16 = num12 * num15 - num13 * num14;
				if (num16 == 0f)
				{
					Debug.LogError("Mesh contains degenerate UVs. Could not compute tangents.");
					return;
				}
				float num17 = 1f / num16;
				Vector3 vector7 = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
				Vector3 vector8 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
				array[num3] += vector7;
				array[num4] += vector7;
				array[num5] += vector7;
				array2[num3] += vector8;
				array2[num4] += vector8;
				array2[num5] += vector8;
			}
			for (int j = 0; j < num2; j++)
			{
				Vector3 vector9 = normals[j];
				Vector3 vector10 = array[j];
				Vector3 normalized = (vector10 - vector9 * Vector3.Dot(vector9, vector10)).normalized;
				outTangents[j] = new Vector4(normalized.x, normalized.y, normalized.z);
				outTangents[j].w = ((!(Vector3.Dot(Vector3.Cross(vector9, vector10), array2[j]) < 0f)) ? 1f : (-1f));
			}
		}

		public void SaveMeshsToAssetDatabase(string folderPath, string newFileNameBase)
		{
			Debug.LogError("Can only save meshes in the editor");
		}

		public void RebuildPrefab(GameObject prefabRoot)
		{
			Debug.LogError("Can only rebuild prefabs in the editor");
		}

		public GameObject buildSceneMeshObject(GameObject root, Mesh m, bool createNewChild = false)
		{
			MeshFilter meshFilter = null;
			MeshRenderer meshRenderer = null;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			Transform transform = null;
			if (!createNewChild)
			{
				foreach (Transform item in root.transform)
				{
					if (item.name.EndsWith("-mesh"))
					{
						transform = item;
					}
				}
			}
			GameObject gameObject = ((!(transform == null)) ? transform.gameObject : new GameObject(name + "-mesh"));
			if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer == null)
				{
					skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			else
			{
				meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}
			if (textureBakeResults.doMultiMaterial)
			{
				Material[] array = new Material[textureBakeResults.resultMaterials.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = textureBakeResults.resultMaterials[i].combinedMaterial;
				}
				if (renderType == MB_RenderType.skinnedMeshRenderer)
				{
					skinnedMeshRenderer.materials = array;
					skinnedMeshRenderer.bones = GetBones();
					skinnedMeshRenderer.updateWhenOffscreen = true;
				}
				else
				{
					meshRenderer.sharedMaterials = array;
				}
			}
			else if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.material = textureBakeResults.resultMaterial;
				skinnedMeshRenderer.bones = GetBones();
			}
			else
			{
				meshRenderer.material = textureBakeResults.resultMaterial;
			}
			if (renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.sharedMesh = m;
				skinnedMeshRenderer.lightmapIndex = GetLightmapIndex();
			}
			else
			{
				meshFilter.sharedMesh = m;
				meshRenderer.lightmapIndex = GetLightmapIndex();
			}
			if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
			{
				gameObject.isStatic = true;
			}
			List<GameObject> objectsInCombined = GetObjectsInCombined();
			if (objectsInCombined.Count > 0 && objectsInCombined[0] != null)
			{
				bool flag = true;
				bool flag2 = true;
				string tag = objectsInCombined[0].tag;
				int layer = objectsInCombined[0].layer;
				for (int j = 0; j < objectsInCombined.Count; j++)
				{
					if (objectsInCombined[j] != null)
					{
						if (!objectsInCombined[j].tag.Equals(tag))
						{
							flag = false;
						}
						if (objectsInCombined[j].layer != layer)
						{
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					root.tag = tag;
					gameObject.tag = tag;
				}
				if (flag2)
				{
					root.layer = layer;
					gameObject.layer = layer;
				}
			}
			gameObject.transform.parent = root.transform;
			return gameObject;
		}
	}
}
