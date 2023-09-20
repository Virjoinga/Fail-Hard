using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_TextureCombiner
	{
		private class MeshBakerMaterialTexture
		{
			public Vector2 offset = new Vector2(0f, 0f);

			public Vector2 scale = new Vector2(1f, 1f);

			public Vector2 obUVoffset = new Vector2(0f, 0f);

			public Vector2 obUVscale = new Vector2(1f, 1f);

			public Texture2D t;

			public MeshBakerMaterialTexture()
			{
			}

			public MeshBakerMaterialTexture(Texture2D tx)
			{
				t = tx;
			}

			public MeshBakerMaterialTexture(Texture2D tx, Vector2 o, Vector2 s, Vector2 oUV, Vector2 sUV)
			{
				t = tx;
				offset = o;
				scale = s;
				obUVoffset = oUV;
				obUVscale = sUV;
			}
		}

		private class MB_TexSet
		{
			public MeshBakerMaterialTexture[] ts;

			public List<Material> mats;

			public int idealWidth;

			public int idealHeight;

			public MB_TexSet(MeshBakerMaterialTexture[] tss)
			{
				ts = tss;
				mats = new List<Material>();
			}

			public override bool Equals(object obj)
			{
				if (!(obj is MB_TexSet))
				{
					return false;
				}
				MB_TexSet mB_TexSet = (MB_TexSet)obj;
				if (mB_TexSet.ts.Length != ts.Length)
				{
					return false;
				}
				for (int i = 0; i < ts.Length; i++)
				{
					if (ts[i].offset != mB_TexSet.ts[i].offset)
					{
						return false;
					}
					if (ts[i].scale != mB_TexSet.ts[i].scale)
					{
						return false;
					}
					if (ts[i].t != mB_TexSet.ts[i].t)
					{
						return false;
					}
					if (ts[i].obUVoffset != mB_TexSet.ts[i].obUVoffset)
					{
						return false;
					}
					if (ts[i].obUVscale != mB_TexSet.ts[i].obUVscale)
					{
						return false;
					}
				}
				return true;
			}

			public override int GetHashCode()
			{
				return 0;
			}
		}

		public delegate void FileSaveFunction(string pth, byte[] bytes);

		private static bool VERBOSE = false;

		private static string[] shaderTexPropertyNames = new string[14]
		{
			"_MainTex", "_BumpMap", "_Normal", "_BumpSpecMap", "_DecalTex", "_Detail", "_GlossMap", "_Illum", "_LightTextureB0", "_ParallaxMap",
			"_ShadowOffset", "_TranslucencyMap", "_SpecMap", "_TranspMap"
		};

		private List<Texture2D> _temporaryTextures = new List<Texture2D>();

		private List<Texture2D> _texturesWithReadWriteFlagSet = new List<Texture2D>();

		public bool combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, List<string> customShaderPropNames, bool resizePowerOfTwoTextures, bool fixOutOfBoundsUVs, int maxTilingBakeSize, bool saveAtlasesAsAssets = false, FileSaveFunction fileSaveFunction = null)
		{
			return _combineTexturesIntoAtlases(progressInfo, results, resultMaterial, objsToMesh, sourceMaterials, atlasPadding, customShaderPropNames, resizePowerOfTwoTextures, fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction);
		}

		private bool _collectPropertyNames(Material resultMaterial, List<string> customShaderPropNames, List<string> texPropertyNames)
		{
			for (int i = 0; i < texPropertyNames.Count; i++)
			{
				string text = customShaderPropNames.Find((string x) => x.Equals(texPropertyNames[i]));
				if (text != null)
				{
					customShaderPropNames.Remove(text);
				}
			}
			if (resultMaterial == null)
			{
				Debug.LogError("Please assign a result material. The combined mesh will use this material.");
				return false;
			}
			string text2 = string.Empty;
			for (int j = 0; j < shaderTexPropertyNames.Length; j++)
			{
				if (resultMaterial.HasProperty(shaderTexPropertyNames[j]))
				{
					text2 = text2 + ", " + shaderTexPropertyNames[j];
					if (!texPropertyNames.Contains(shaderTexPropertyNames[j]))
					{
						texPropertyNames.Add(shaderTexPropertyNames[j]);
					}
					if (resultMaterial.GetTextureOffset(shaderTexPropertyNames[j]) != new Vector2(0f, 0f))
					{
						Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
					}
					if (resultMaterial.GetTextureScale(shaderTexPropertyNames[j]) != new Vector2(1f, 1f))
					{
						Debug.LogWarning("Result material should probably have tiling of 1,1");
					}
				}
			}
			for (int k = 0; k < customShaderPropNames.Count; k++)
			{
				if (resultMaterial.HasProperty(customShaderPropNames[k]))
				{
					text2 = text2 + ", " + customShaderPropNames[k];
					texPropertyNames.Add(customShaderPropNames[k]);
					if (resultMaterial.GetTextureOffset(customShaderPropNames[k]) != new Vector2(0f, 0f))
					{
						Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
					}
					if (resultMaterial.GetTextureScale(customShaderPropNames[k]) != new Vector2(1f, 1f))
					{
						Debug.LogWarning("Result material should probably have tiling of 1,1.");
					}
				}
				else
				{
					Debug.LogWarning("Result material shader does not use property " + customShaderPropNames[k] + " in the list of custom shader property names");
				}
			}
			return true;
		}

		private bool _combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, List<string> customShaderPropNames, bool resizePowerOfTwoTextures, bool fixOutOfBoundsUVs, int maxTilingBakeSize, bool saveAtlasesAsAssets, FileSaveFunction fileSaveFunction)
		{
			bool result = false;
			try
			{
				_temporaryTextures.Clear();
				_texturesWithReadWriteFlagSet.Clear();
				if (objsToMesh == null || objsToMesh.Count == 0)
				{
					Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
					return false;
				}
				if (atlasPadding < 0)
				{
					Debug.LogError("Atlas padding must be zero or greater.");
					return false;
				}
				if (maxTilingBakeSize < 2 || maxTilingBakeSize > 4096)
				{
					Debug.LogError("Invalid value for max tiling bake size.");
					return false;
				}
				if (progressInfo != null)
				{
					progressInfo("Collecting textures for " + objsToMesh.Count + " meshes.", 0.01f);
				}
				List<string> texPropertyNames = new List<string>();
				if (!_collectPropertyNames(resultMaterial, customShaderPropNames, texPropertyNames))
				{
					return false;
				}
				result = __combineTexturesIntoAtlases(progressInfo, results, resultMaterial, texPropertyNames, objsToMesh, sourceMaterials, atlasPadding, resizePowerOfTwoTextures, fixOutOfBoundsUVs, maxTilingBakeSize, saveAtlasesAsAssets, fileSaveFunction);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				_destroyTemporaryTextures();
				_SetReadFlags();
			}
			return result;
		}

		private bool __combineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects results, Material resultMaterial, List<string> texPropertyNames, List<GameObject> objsToMesh, List<Material> sourceMaterials, int atlasPadding, bool resizePowerOfTwoTextures, bool fixOutOfBoundsUVs, int maxTilingBakeSize, bool saveAtlasesAsAssets, FileSaveFunction fileSaveFunction)
		{
			int count = texPropertyNames.Count;
			bool flag = false;
			List<MB_TexSet> list = new List<MB_TexSet>();
			if (VERBOSE)
			{
				Debug.Log("__combineTexturesIntoAtlases atlases:" + texPropertyNames.Count + " objsToMesh:" + objsToMesh.Count + " fixOutOfBoundsUVs:" + fixOutOfBoundsUVs);
			}
			for (int i = 0; i < objsToMesh.Count; i++)
			{
				GameObject gameObject = objsToMesh[i];
				if (VERBOSE)
				{
					Debug.Log("Collecting textures for object " + gameObject);
				}
				if (gameObject == null)
				{
					Debug.LogError("The list of objects to mesh contained nulls.");
					return false;
				}
				Mesh mesh = MB_Utility.GetMesh(gameObject);
				if (mesh == null)
				{
					Debug.LogError("Object " + gameObject.name + " in the list of objects to mesh has no mesh.");
					return false;
				}
				Material[] gOMaterials = MB_Utility.GetGOMaterials(gameObject);
				if (gOMaterials == null)
				{
					Debug.LogError("Object " + gameObject.name + " in the list of objects has no materials.");
					return false;
				}
				for (int j = 0; j < gOMaterials.Length; j++)
				{
					Material material = gOMaterials[j];
					if (sourceMaterials != null && !sourceMaterials.Contains(material))
					{
						continue;
					}
					Rect uvBounds = default(Rect);
					bool flag2 = MB_Utility.hasOutOfBoundsUVs(mesh, ref uvBounds, j);
					flag = flag || flag2;
					if (material.name.Contains("(Instance)"))
					{
						Debug.LogError("The sharedMaterial on object " + gameObject.name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor.  The material to UV Rectangle mapping will be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset.");
						return false;
					}
					if (fixOutOfBoundsUVs && !MB_Utility.validateOBuvsMultiMaterial(gOMaterials))
					{
						Debug.LogWarning("Object " + gameObject.name + " uses the same material on multiple submeshes. This may generate strange results especially when used with fix out of bounds uvs. Try duplicating the material.");
					}
					if (progressInfo != null)
					{
						progressInfo("Collecting textures for " + material, 0.01f);
					}
					MeshBakerMaterialTexture[] array = new MeshBakerMaterialTexture[texPropertyNames.Count];
					for (int k = 0; k < texPropertyNames.Count; k++)
					{
						Texture2D texture2D = null;
						Vector2 s = Vector2.one;
						Vector2 o = Vector2.zero;
						Vector2 sUV = Vector2.one;
						Vector2 oUV = Vector2.zero;
						if (material.HasProperty(texPropertyNames[k]))
						{
							Texture texture = material.GetTexture(texPropertyNames[k]);
							if (texture != null)
							{
								if (!(texture is Texture2D))
								{
									Debug.LogError("Object " + gameObject.name + " in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases.");
									return false;
								}
								texture2D = (Texture2D)texture;
								TextureFormat format = texture2D.format;
								if (format != TextureFormat.ARGB32 && format != TextureFormat.RGBA32 && format != TextureFormat.BGRA32 && format != TextureFormat.RGB24 && format != TextureFormat.Alpha8)
								{
									Debug.LogError(string.Concat("Object ", gameObject.name, " in the list of objects to mesh uses Texture ", texture2D.name, " uses format ", format, " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'"));
									return false;
								}
							}
							o = material.GetTextureOffset(texPropertyNames[k]);
							s = material.GetTextureScale(texPropertyNames[k]);
						}
						if (texture2D == null)
						{
							Debug.LogWarning("No texture selected for " + texPropertyNames[k] + " in object " + objsToMesh[i].name);
						}
						if (fixOutOfBoundsUVs && flag2)
						{
							sUV = new Vector2(uvBounds.width, uvBounds.height);
							oUV = new Vector2(uvBounds.x, uvBounds.y);
						}
						array[k] = new MeshBakerMaterialTexture(texture2D, o, s, oUV, sUV);
					}
					MB_TexSet setOfTexs = new MB_TexSet(array);
					MB_TexSet mB_TexSet = list.Find((MB_TexSet x) => x.Equals(setOfTexs));
					if (mB_TexSet != null)
					{
						setOfTexs = mB_TexSet;
					}
					else
					{
						list.Add(setOfTexs);
					}
					if (!setOfTexs.mats.Contains(material))
					{
						setOfTexs.mats.Add(material);
					}
				}
			}
			int num = atlasPadding;
			if (list.Count == 1)
			{
				Debug.Log("All objects use the same textures.");
				num = 0;
			}
			for (int l = 0; l < list.Count; l++)
			{
				MB_TexSet mB_TexSet2 = list[l];
				mB_TexSet2.idealWidth = 1;
				mB_TexSet2.idealHeight = 1;
				int num2 = 1;
				int num3 = 1;
				for (int m = 0; m < mB_TexSet2.ts.Length; m++)
				{
					MeshBakerMaterialTexture meshBakerMaterialTexture = mB_TexSet2.ts[m];
					if (meshBakerMaterialTexture.t == null)
					{
						Debug.LogWarning("Creating empty texture for " + texPropertyNames[m]);
						meshBakerMaterialTexture.t = _createTemporaryTexture(num2, num3, TextureFormat.ARGB32, true);
					}
					if (!meshBakerMaterialTexture.scale.Equals(Vector2.one))
					{
						Debug.LogWarning(string.Concat("Texture ", meshBakerMaterialTexture.t, "is tiled by ", meshBakerMaterialTexture.scale, " tiling will be baked into a texture with maxSize:", maxTilingBakeSize));
					}
					if (!meshBakerMaterialTexture.obUVscale.Equals(Vector2.one))
					{
						Debug.LogWarning(string.Concat("Texture ", meshBakerMaterialTexture.t, "has out of bounds UVs that effectively tile by ", meshBakerMaterialTexture.obUVscale, " tiling will be baked into a texture with maxSize:", maxTilingBakeSize));
					}
					Vector2 adjustedForScaleAndOffset2Dimensions = getAdjustedForScaleAndOffset2Dimensions(meshBakerMaterialTexture, fixOutOfBoundsUVs, maxTilingBakeSize);
					num2 = (int)adjustedForScaleAndOffset2Dimensions.x;
					num3 = (int)adjustedForScaleAndOffset2Dimensions.y;
					if (meshBakerMaterialTexture.t.width * meshBakerMaterialTexture.t.height > num2 * num3)
					{
						num2 = meshBakerMaterialTexture.t.width;
						num3 = meshBakerMaterialTexture.t.height;
					}
				}
				if (resizePowerOfTwoTextures)
				{
					if (IsPowerOfTwo(num2))
					{
						num2 -= num * 2;
					}
					if (IsPowerOfTwo(num3))
					{
						num3 -= num * 2;
					}
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (num3 < 1)
					{
						num3 = 1;
					}
				}
				mB_TexSet2.idealWidth = num2;
				mB_TexSet2.idealHeight = num3;
			}
			Rect[] array2 = null;
			Texture2D[] array3 = new Texture2D[count];
			Rect[] rs = null;
			long num4 = 0L;
			StringBuilder stringBuilder = new StringBuilder();
			if (count > 0)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Report");
				for (int n = 0; n < list.Count; n++)
				{
					MB_TexSet mB_TexSet3 = list[n];
					stringBuilder.AppendLine("----------");
					stringBuilder.Append("Will be resized to:" + mB_TexSet3.idealWidth + "x" + mB_TexSet3.idealHeight);
					for (int num5 = 0; num5 < mB_TexSet3.ts.Length; num5++)
					{
						if (mB_TexSet3.ts[num5].t != null)
						{
							stringBuilder.Append(" [" + texPropertyNames[num5] + " " + mB_TexSet3.ts[num5].t.name + " " + mB_TexSet3.ts[num5].t.width + "x" + mB_TexSet3.ts[num5].t.height + "]");
						}
						else
						{
							stringBuilder.Append(" [" + texPropertyNames[num5] + " null]");
						}
					}
					stringBuilder.AppendLine(string.Empty);
					stringBuilder.Append("Materials using:");
					for (int num6 = 0; num6 < mB_TexSet3.mats.Count; num6++)
					{
						stringBuilder.Append(mB_TexSet3.mats[num6].name + ", ");
					}
					stringBuilder.AppendLine(string.Empty);
				}
			}
			if (progressInfo != null)
			{
				progressInfo("Creating txture atlases.", 0.1f);
			}
			int w = 1;
			int h = 1;
			for (int num7 = 0; num7 < count; num7++)
			{
				if (VERBOSE)
				{
					Debug.Log("Beginning loop " + num7 + " num temporary textures " + _temporaryTextures.Count);
				}
				for (int num8 = 0; num8 < list.Count; num8++)
				{
					MB_TexSet mB_TexSet4 = list[num8];
					int idealWidth = mB_TexSet4.idealWidth;
					int idealHeight = mB_TexSet4.idealHeight;
					Texture2D texture2D2 = mB_TexSet4.ts[num7].t;
					if (texture2D2 == null)
					{
						texture2D2 = (mB_TexSet4.ts[num7].t = _createTemporaryTexture(idealWidth, idealHeight, TextureFormat.ARGB32, true));
					}
					if (progressInfo != null)
					{
						progressInfo("Adjusting for scale and offset " + texture2D2, 0.01f);
					}
					setReadWriteFlag(texture2D2, true, true);
					texture2D2 = getAdjustedForScaleAndOffset2(mB_TexSet4.ts[num7], fixOutOfBoundsUVs, maxTilingBakeSize);
					if (texture2D2.width != idealWidth || texture2D2.height != idealHeight)
					{
						if (progressInfo != null)
						{
							progressInfo(string.Concat("Resizing texture '", texture2D2, "'"), 0.01f);
						}
						if (VERBOSE)
						{
							Debug.Log("Copying and resizing texture " + texPropertyNames[num7] + " from " + texture2D2.width + "x" + texture2D2.height + " to " + idealWidth + "x" + idealHeight);
						}
						setReadWriteFlag(texture2D2, true, true);
						texture2D2 = _resizeTexture(texture2D2, idealWidth, idealHeight);
					}
					mB_TexSet4.ts[num7].t = texture2D2;
				}
				Texture2D[] array4 = new Texture2D[list.Count];
				for (int num9 = 0; num9 < list.Count; num9++)
				{
					Texture2D t = list[num9].ts[num7].t;
					num4 += t.width * t.height;
					array4[num9] = t;
				}
				if (Math.Sqrt(num4) > 3500.0)
				{
					Debug.LogWarning("The maximum possible atlas size is 4096. Textures may be shrunk");
				}
				array3[num7] = new Texture2D(1, 1, TextureFormat.ARGB32, true);
				if (progressInfo != null)
				{
					progressInfo("Packing texture atlas " + texPropertyNames[num7], 0.25f);
				}
				if (num7 == 0)
				{
					if (progressInfo != null)
					{
						progressInfo("Estimated min size of atlases: " + Math.Sqrt(num4).ToString("F0"), 0.1f);
					}
					Debug.Log("Estimated texture minimum size:" + Math.Sqrt(num4).ToString("F0"));
					if (list.Count == 1)
					{
						Rect[] obj = new Rect[1]
						{
							new Rect(0f, 0f, 1f, 1f)
						};
						rs = obj;
						array2 = obj;
						array3[num7] = _copyTexturesIntoAtlas(array4, num, rs, array4[0].width, array4[0].height);
					}
					else
					{
						int maximumAtlasSize = 4096;
						array2 = (rs = array3[num7].PackTextures(array4, num, maximumAtlasSize, false));
					}
					Debug.Log("After pack textures size " + array3[num7].width + " " + array3[num7].height);
					w = array3[num7].width;
					h = array3[num7].height;
					array3[num7].Apply();
				}
				else
				{
					array3[num7] = _copyTexturesIntoAtlas(array4, num, rs, w, h);
				}
				if (saveAtlasesAsAssets)
				{
					_saveAtlasToAssetDatabase(array3[num7], texPropertyNames[num7], num7, resultMaterial, fileSaveFunction);
				}
				_destroyTemporaryTextures();
			}
			Dictionary<Material, Rect> dictionary = new Dictionary<Material, Rect>();
			for (int num10 = 0; num10 < list.Count; num10++)
			{
				List<Material> mats = list[num10].mats;
				for (int num11 = 0; num11 < mats.Count; num11++)
				{
					if (!dictionary.ContainsKey(mats[num11]))
					{
						dictionary.Add(mats[num11], array2[num10]);
					}
				}
			}
			results.atlases = array3;
			results.texPropertyNames = texPropertyNames.ToArray();
			results.mat2rect_map = dictionary;
			_destroyTemporaryTextures();
			_SetReadFlags();
			if (stringBuilder != null)
			{
				Debug.Log(stringBuilder.ToString());
			}
			return true;
		}

		private Texture2D _copyTexturesIntoAtlas(Texture2D[] texToPack, int padding, Rect[] rs, int w, int h)
		{
			Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, true);
			MB_Utility.setSolidColor(texture2D, Color.clear);
			for (int i = 0; i < rs.Length; i++)
			{
				Rect rect = rs[i];
				Texture2D texture2D2 = texToPack[i];
				int x = Mathf.RoundToInt(rect.x * (float)w);
				int y = Mathf.RoundToInt(rect.y * (float)h);
				int num = Mathf.RoundToInt(rect.width * (float)w);
				int num2 = Mathf.RoundToInt(rect.height * (float)h);
				if (texture2D2.width != num && texture2D2.height != num2)
				{
					texture2D2 = MB_Utility.resampleTexture(texture2D2, num, num2);
					_temporaryTextures.Add(texture2D2);
				}
				texture2D.SetPixels(x, y, num, num2, texture2D2.GetPixels());
			}
			texture2D.Apply();
			return texture2D;
		}

		private bool IsPowerOfTwo(int x)
		{
			return (x & (x - 1)) == 0;
		}

		private void setReadWriteFlag(Texture2D tx, bool isReadable, bool addToList)
		{
		}

		private void setTextureSize(Texture2D tx)
		{
		}

		private bool _isCompressed(Texture2D tx)
		{
			return false;
		}

		private Vector2 getAdjustedForScaleAndOffset2Dimensions(MeshBakerMaterialTexture source, bool fixOutOfBoundsUVs, int maxSize)
		{
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f)
			{
				if (!fixOutOfBoundsUVs)
				{
					return new Vector2(source.t.width, source.t.height);
				}
				if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f)
				{
					return new Vector2(source.t.width, source.t.height);
				}
			}
			if (VERBOSE)
			{
				Debug.Log(string.Concat("getAdjustedForScaleAndOffset2Dimensions: ", source.t, " ", source.obUVoffset, " ", source.obUVscale));
			}
			float num = (float)source.t.width * source.scale.x;
			float num2 = (float)source.t.height * source.scale.y;
			if (fixOutOfBoundsUVs)
			{
				num *= source.obUVscale.x;
				num2 *= source.obUVscale.y;
			}
			if (num > (float)maxSize)
			{
				num = maxSize;
			}
			if (num2 > (float)maxSize)
			{
				num2 = maxSize;
			}
			if (num < 1f)
			{
				num = 1f;
			}
			if (num2 < 1f)
			{
				num2 = 1f;
			}
			return new Vector2(num, num2);
		}

		private Texture2D getAdjustedForScaleAndOffset2(MeshBakerMaterialTexture source, bool fixOutOfBoundsUVs, int maxSize)
		{
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f)
			{
				if (!fixOutOfBoundsUVs)
				{
					return source.t;
				}
				if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f)
				{
					return source.t;
				}
			}
			Vector2 adjustedForScaleAndOffset2Dimensions = getAdjustedForScaleAndOffset2Dimensions(source, fixOutOfBoundsUVs, maxSize);
			if (VERBOSE)
			{
				Debug.Log(string.Concat("getAdjustedForScaleAndOffset2: ", source.t, " ", source.obUVoffset, " ", source.obUVscale));
			}
			float x = adjustedForScaleAndOffset2Dimensions.x;
			float y = adjustedForScaleAndOffset2Dimensions.y;
			float num = source.scale.x;
			float num2 = source.scale.y;
			float num3 = source.offset.x;
			float num4 = source.offset.y;
			if (fixOutOfBoundsUVs)
			{
				num *= source.obUVscale.x;
				num2 *= source.obUVscale.y;
				num3 += source.obUVoffset.x;
				num4 += source.obUVoffset.y;
			}
			Texture2D texture2D = _createTemporaryTexture((int)x, (int)y, TextureFormat.ARGB32, true);
			for (int i = 0; i < texture2D.width; i++)
			{
				for (int j = 0; j < texture2D.height; j++)
				{
					float u = (float)i / x * num + num3;
					float v = (float)j / y * num2 + num4;
					texture2D.SetPixel(i, j, source.t.GetPixelBilinear(u, v));
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		private Texture2D _createTemporaryTexture(int w, int h, TextureFormat texFormat, bool mipMaps)
		{
			Texture2D texture2D = new Texture2D(w, h, texFormat, mipMaps);
			MB_Utility.setSolidColor(texture2D, Color.clear);
			_temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private Texture2D _createTextureCopy(Texture2D t)
		{
			Texture2D texture2D = MB_Utility.createTextureCopy(t);
			_temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private Texture2D _resizeTexture(Texture2D t, int w, int h)
		{
			Texture2D texture2D = MB_Utility.resampleTexture(t, w, h);
			_temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private void _destroyTemporaryTextures()
		{
			if (VERBOSE)
			{
				Debug.Log("Destroying " + _temporaryTextures.Count + " temporary textures");
			}
			for (int i = 0; i < _temporaryTextures.Count; i++)
			{
				_destroy(_temporaryTextures[i]);
			}
			_temporaryTextures.Clear();
		}

		private void _SetReadFlags()
		{
			for (int i = 0; i < _texturesWithReadWriteFlagSet.Count; i++)
			{
				setReadWriteFlag(_texturesWithReadWriteFlagSet[i], false, false);
			}
			_texturesWithReadWriteFlagSet.Clear();
		}

		private void _destroy(UnityEngine.Object o)
		{
			UnityEngine.Object.Destroy(o);
		}

		private void _saveAtlasToAssetDatabase(Texture2D atlas, string texPropertyName, int atlasNum, Material resMat, FileSaveFunction fileSaveFunction)
		{
		}

		private void _setMaterialTextureProperty(Material target, string texPropName, string texturePath)
		{
		}

		private void setNormalMap(Texture2D tx)
		{
		}
	}
}
