using UnityEngine;

public class UVAnimator : MonoBehaviour
{
	public Vector2 uv_OffsetSpeed = default(Vector2);

	public string textureName;

	private Material m_material;

	private void Start()
	{
		if (textureName != null && textureName.Length <= 0)
		{
			Debug.LogWarning("Texture name must be given!");
			base.enabled = false;
		}
		m_material = base.renderer.material;
	}

	private void Update()
	{
		Vector2 textureOffset = m_material.GetTextureOffset(textureName);
		textureOffset += uv_OffsetSpeed * Time.deltaTime;
		m_material.SetTextureOffset(textureName, textureOffset);
	}
}
