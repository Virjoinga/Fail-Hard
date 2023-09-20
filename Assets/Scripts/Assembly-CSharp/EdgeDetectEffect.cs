using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[RequireComponent(typeof(Camera))]
internal class EdgeDetectEffect : MonoBehaviour
{
	public float sensitivityDepth = 1f;

	public float sensitivityNormals = 1f;

	public float edgesOnly;

	public Color edgesOnlyBgColor = Color.white;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;

	public void OnEnable()
	{
		edgeDetectMaterial = new Material(edgeDetectShader);
		base.camera.depthTextureMode = DepthTextureMode.DepthNormals;
	}

	public void OnDisable()
	{
		if ((bool)edgeDetectMaterial)
		{
			Object.DestroyImmediate(edgeDetectMaterial);
		}
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Vector2 vector = new Vector2(sensitivityDepth, sensitivityNormals);
		source.filterMode = FilterMode.Point;
		edgeDetectMaterial.SetVector("sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
		edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
		Vector4 vector2 = edgesOnlyBgColor;
		edgeDetectMaterial.SetVector("_BgColor", vector2);
		Graphics.Blit(source, destination, edgeDetectMaterial, 0);
	}
}
