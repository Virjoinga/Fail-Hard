using UnityEngine;

public class OverridePositionPlatform : MonoBehaviour
{
	public RuntimePlatform Platform;

	public Vector3 Position;

	private void Start()
	{
		if (Platform == Configuration.CurrentPlatform)
		{
			base.transform.localPosition = Position;
		}
	}
}
