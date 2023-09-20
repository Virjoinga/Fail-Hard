using UnityEngine;

public class ScreenshotItem : MonoBehaviour
{
	private Texture m_screenShot;

	public void Start()
	{
		UITexture componentInChildren = GetComponentInChildren<UITexture>();
		componentInChildren.material = null;
		componentInChildren.mainTexture = m_screenShot;
	}

	public void SetData(Texture screenShot, float rotation, float zDepth)
	{
		m_screenShot = screenShot;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = zDepth;
		base.transform.localPosition = localPosition;
		base.transform.Rotate(Vector3.forward, rotation, Space.Self);
	}
}
