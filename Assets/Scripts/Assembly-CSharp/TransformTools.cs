using System.Collections.Generic;
using UnityEngine;

public static class TransformTools
{
	public class TransformData
	{
		public Vector3 LocalPosition;

		public Vector3 LocalScale;

		public Quaternion LocalRotation;

		public TransformData(Transform tf)
		{
			Update(tf);
		}

		public void Apply(Transform tf)
		{
			tf.localPosition = LocalPosition;
			tf.localScale = LocalScale;
			tf.localRotation = LocalRotation;
		}

		public void Update(Transform tf)
		{
			LocalPosition = tf.localPosition;
			LocalScale = tf.localScale;
			LocalRotation = tf.localRotation;
		}
	}

	private static Dictionary<int, TransformData> s_transformCache = new Dictionary<int, TransformData>();

	public static void CacheTransformData(Transform transform)
	{
		TransformData value = null;
		if (s_transformCache.TryGetValue(transform.GetInstanceID(), out value))
		{
			value.Update(transform);
			return;
		}
		value = new TransformData(transform);
		s_transformCache.Add(transform.GetInstanceID(), value);
	}

	public static void ApplyCachedTransformData(Transform transform)
	{
		TransformData value = null;
		if (s_transformCache.TryGetValue(transform.GetInstanceID(), out value))
		{
			value.Apply(transform);
		}
		else
		{
			Debug.LogWarning("No cached transform data for: " + transform.name);
		}
	}

	public static TransformData GetCachedTransformData(Transform transform)
	{
		TransformData value = null;
		s_transformCache.TryGetValue(transform.GetInstanceID(), out value);
		return value;
	}
}
