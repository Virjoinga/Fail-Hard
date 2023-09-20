using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB2_TextureBaker : MB2_MeshBakerRoot
{
	private static bool VERBOSE;

	[HideInInspector]
	public int maxTilingBakeSize = 1024;

	[HideInInspector]
	public bool doMultiMaterial;

	[HideInInspector]
	public bool fixOutOfBoundsUVs;

	[HideInInspector]
	public Material resultMaterial;

	public MB_MultiMaterial[] resultMaterials = new MB_MultiMaterial[0];

	[HideInInspector]
	public int atlasPadding = 1;

	[HideInInspector]
	public bool resizePowerOfTwoTextures = true;

	public List<string> customShaderPropNames = new List<string>();

	public List<GameObject> objsToMesh;

	public override List<GameObject> GetObjectsToCombine()
	{
		return objsToMesh;
	}

	public MB_AtlasesAndRects[] CreateAtlases(ProgressUpdateDelegate progressInfo, bool saveAtlasesAsAssets = false, MB_TextureCombiner.FileSaveFunction fileSaveFunction = null)
	{
		if (doMultiMaterial)
		{
			for (int i = 0; i < resultMaterials.Length; i++)
			{
				MB_MultiMaterial mB_MultiMaterial = resultMaterials[i];
				if (mB_MultiMaterial.combinedMaterial == null)
				{
					Debug.LogError("Combined Material is null please create and assign a result material.");
					return null;
				}
				Shader shader = mB_MultiMaterial.combinedMaterial.shader;
				for (int j = 0; j < mB_MultiMaterial.sourceMaterials.Count; j++)
				{
					if (mB_MultiMaterial.sourceMaterials[j] == null)
					{
						Debug.LogError("There are null entries in the list of Source Materials");
						return null;
					}
					if (shader != mB_MultiMaterial.sourceMaterials[j].shader)
					{
						Debug.LogWarning(string.Concat("Source material ", mB_MultiMaterial.sourceMaterials[j], " does not use shader ", shader, " it may not have the required textures. If not empty textures will be generated."));
					}
				}
			}
		}
		else
		{
			if (resultMaterial == null)
			{
				Debug.LogError("Combined Material is null please create and assign a result material.");
				return null;
			}
			Shader shader2 = resultMaterial.shader;
			for (int k = 0; k < objsToMesh.Count; k++)
			{
				Material[] gOMaterials = MB_Utility.GetGOMaterials(objsToMesh[k]);
				foreach (Material material in gOMaterials)
				{
					if (material == null)
					{
						Debug.LogError(string.Concat("Game object ", objsToMesh[k], " has a null material. Can't build atlases"));
						return null;
					}
					if (material.shader != shader2)
					{
						Debug.LogWarning(string.Concat("Game object ", objsToMesh[k], " does not use shader ", shader2, " it may not have the required textures. If not empty textures will be generated."));
					}
				}
			}
		}
		int num = 1;
		if (doMultiMaterial)
		{
			num = resultMaterials.Length;
		}
		MB_AtlasesAndRects[] array = new MB_AtlasesAndRects[num];
		for (int m = 0; m < array.Length; m++)
		{
			array[m] = new MB_AtlasesAndRects();
		}
		MB_TextureCombiner mB_TextureCombiner = new MB_TextureCombiner();
		Material material2 = null;
		List<Material> sourceMaterials = null;
		for (int n = 0; n < array.Length; n++)
		{
			if (doMultiMaterial)
			{
				sourceMaterials = resultMaterials[n].sourceMaterials;
				material2 = resultMaterials[n].combinedMaterial;
			}
			else
			{
				material2 = resultMaterial;
			}
			Debug.Log("Creating atlases for result material " + material2);
			if (!mB_TextureCombiner.combineTexturesIntoAtlases(progressInfo, array[n], material2, objsToMesh, sourceMaterials, atlasPadding, customShaderPropNames, resizePowerOfTwoTextures, fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction))
			{
				return null;
			}
		}
		if (array != null)
		{
			textureBakeResults.combinedMaterialInfo = array;
			textureBakeResults.doMultiMaterial = doMultiMaterial;
			textureBakeResults.resultMaterial = resultMaterial;
			textureBakeResults.resultMaterials = resultMaterials;
			textureBakeResults.fixOutOfBoundsUVs = fixOutOfBoundsUVs;
			unpackMat2RectMap(textureBakeResults);
			if (Application.isPlaying)
			{
				if (doMultiMaterial)
				{
					for (int num2 = 0; num2 < resultMaterials.Length; num2++)
					{
						Material combinedMaterial = resultMaterials[num2].combinedMaterial;
						Texture2D[] atlases = array[num2].atlases;
						for (int num3 = 0; num3 < atlases.Length; num3++)
						{
							combinedMaterial.SetTexture(array[num2].texPropertyNames[num3], atlases[num3]);
						}
					}
				}
				else
				{
					Material material3 = resultMaterial;
					Texture2D[] atlases2 = array[0].atlases;
					for (int num4 = 0; num4 < atlases2.Length; num4++)
					{
						material3.SetTexture(array[0].texPropertyNames[num4], atlases2[num4]);
					}
				}
			}
		}
		if (VERBOSE)
		{
			Debug.Log("Created Atlases");
		}
		return array;
	}

	public MB_AtlasesAndRects[] CreateAtlases()
	{
		return CreateAtlases(null);
	}

	private void unpackMat2RectMap(MB2_TextureBakeResults results)
	{
		List<Material> list = new List<Material>();
		List<Rect> list2 = new List<Rect>();
		for (int i = 0; i < results.combinedMaterialInfo.Length; i++)
		{
			MB_AtlasesAndRects mB_AtlasesAndRects = results.combinedMaterialInfo[i];
			Dictionary<Material, Rect> mat2rect_map = mB_AtlasesAndRects.mat2rect_map;
			foreach (Material key in mat2rect_map.Keys)
			{
				list.Add(key);
				list2.Add(mat2rect_map[key]);
			}
		}
		results.materials = list.ToArray();
		results.prefabUVRects = list2.ToArray();
	}

	public void CreateAndSaveAtlases(ProgressUpdateDelegate progressInfo, MB_TextureCombiner.FileSaveFunction fileSaveFunction)
	{
		MB_AtlasesAndRects[] array = null;
		try
		{
			if (!MB2_MeshBakerRoot.doCombinedValidate(this, MB_ObjsToCombineTypes.dontCare))
			{
				return;
			}
			array = CreateAtlases(progressInfo, true, fileSaveFunction);
			if (array != null)
			{
				MB2_MeshBakerCommon component = GetComponent<MB2_MeshBakerCommon>();
				if (component != null)
				{
					component.textureBakeResults = textureBakeResults;
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
			if (array != null)
			{
				foreach (MB_AtlasesAndRects mB_AtlasesAndRects in array)
				{
					if (mB_AtlasesAndRects == null || mB_AtlasesAndRects.atlases == null)
					{
						continue;
					}
					for (int j = 0; j < mB_AtlasesAndRects.atlases.Length; j++)
					{
						if (mB_AtlasesAndRects.atlases[j] != null)
						{
							MB_Utility.Destroy(mB_AtlasesAndRects.atlases[j]);
						}
					}
				}
			}
		}
	}
}
