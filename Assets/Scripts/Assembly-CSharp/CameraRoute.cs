using UnityEngine;

[PBSerialize("CameraRoute")]
public class CameraRoute : MonoBehaviour
{
	[PBSerializeField]
	public Transform startPoint;

	[PBSerializeField]
	public Transform endPoint;

	public Vector3 GetPosition(float u)
	{
		return startPoint.position + u * (endPoint.position - startPoint.position);
	}

	public float GetU(Vector3 pos)
	{
		float x = pos.x;
		if ((startPoint.position - endPoint.position).sqrMagnitude == 0f)
		{
			return 1f;
		}
		if (x <= startPoint.position.x)
		{
			return 0f;
		}
		if (x >= endPoint.position.x)
		{
			return 1f;
		}
		return (x - startPoint.position.x) / (endPoint.position.x - startPoint.position.x);
	}
}
