using UnityEngine;

[PBSerialize("ConfigLights")]
public class ConfigLights : MonoBehaviour
{
	[PBSerializeField]
	public float Intensity;

	[PBSerializeField]
	public Vector3 AmbientLightColor;

	private void Start()
	{
		RenderSettings.ambientLight = new Color(AmbientLightColor[0], AmbientLightColor[1], AmbientLightColor[2]);
		GameObject gameObject = GameObject.Find("Directional light");
		if (gameObject != null)
		{
			gameObject.GetComponent<Light>().intensity = Intensity;
		}
	}
}
