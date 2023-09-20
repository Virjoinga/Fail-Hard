using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AmbientLightControl : MonoBehaviour
{
	public Color AmbientLight;

	public void Start()
	{
		RenderSettings.ambientLight = AmbientLight;
	}
}
