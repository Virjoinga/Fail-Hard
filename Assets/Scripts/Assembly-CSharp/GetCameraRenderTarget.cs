using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class GetCameraRenderTarget : MonoBehaviour
{
	public void Start()
	{
		UITexture component = GetComponent<UITexture>();
		LevelCamera component2 = Camera.main.GetComponent<LevelCamera>();
		if ((bool)component2)
		{
			component2.EnableScreenshot();
			component.mainTexture = Camera.main.targetTexture;
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
