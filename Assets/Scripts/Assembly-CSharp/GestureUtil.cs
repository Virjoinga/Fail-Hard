using UnityEngine;

public class GestureUtil
{
	public static Vector2 ScreenPercentage(Vector2 pixelDelta)
	{
		Vector2 result = default(Vector2);
		result.x = pixelDelta.x / (float)Screen.width;
		result.y = pixelDelta.y / (float)Screen.height;
		return result;
	}
}
