using UnityEngine;

public class PerlinModifier : MonoBehaviour
{
	public Transform[] floaters;

	public bool blender;

	public bool noZChange;

	private Vector3[] original;

	private bool initialized;

	public void Calculate(float x, float z)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Vector2 vector = new Vector2(x + child.localPosition.x, z + child.localPosition.z);
			MeshFilter component = child.GetComponent<MeshFilter>();
			Vector3[] vertices = component.mesh.vertices;
			Vector3[] normals = component.mesh.normals;
			if (original == null)
			{
				original = new Vector3[vertices.Length];
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				if (!initialized)
				{
					original[j] = vertices[j];
				}
				if (blender)
				{
					float ix = vector.x - vertices[j].y;
					float num = vector.y - vertices[j].x;
					vertices[j].z = original[j].z + ComplexPerlinHeight(ix, (!noZChange) ? num : 0f);
					normals[j] = CalculateNormal(ix, num);
				}
				else
				{
					float ix = vector.x + vertices[j].x;
					float num = vector.y + vertices[j].z;
					vertices[j].y = ComplexPerlinHeight(ix, num);
					normals[j] = CalculateNormal(ix, num);
				}
			}
			initialized = true;
			component.mesh.vertices = vertices;
			component.mesh.normals = normals;
			component.mesh.RecalculateBounds();
			MeshCollider component2 = child.GetComponent<MeshCollider>();
			if (component2 != null)
			{
				component2.sharedMesh = null;
				component2.sharedMesh = component.mesh;
			}
			Transform[] array = floaters;
			foreach (Transform transform in array)
			{
				float ix2 = vector.x + transform.localPosition.x;
				float iz = vector.y + transform.localPosition.z;
				transform.localPosition = new Vector3(transform.localPosition.x, ComplexPerlinHeight(ix2, iz) + 0.5f, transform.localPosition.z);
			}
		}
	}

	private Vector3 CalculateNormal(float ix, float iz)
	{
		float num = 0.1f;
		Vector3 vector = new Vector3(ix - num, ComplexPerlinHeight(ix - num, iz - num), iz - num);
		Vector3 vector2 = new Vector3(ix, ComplexPerlinHeight(ix, iz + num), iz + num);
		Vector3 vector3 = new Vector3(ix + num, ComplexPerlinHeight(ix + num, iz), iz);
		Vector3 lhs = vector2 - vector;
		Vector3 rhs = vector3 - vector;
		return Vector3.Cross(lhs, rhs).normalized;
	}

	private float ComplexPerlinHeight(float ix, float iz)
	{
		return ph(ix, iz, Mathf.Clamp(2f + ix * ix / 500f, 1f, 10f), 0.05f) + ph(ix, iz, 1.2f, 0.25f) + ph(ix, iz, Mathf.Clamp(((!(ix > 100f)) ? 0f : (ix - 100f)) * 0.1f, 0f, 1.5f), 0.3f) + ph(ix, iz, Mathf.Clamp(((!(ix > 600f)) ? 0f : (ix - 600f)) * 0.1f, 0f, 1.3f), 0.27f);
	}

	private float PerlinHeight(float ix, float iz)
	{
		float num = 3f;
		float num2 = 0.05f;
		float x = ix * num2;
		float y = iz * num2;
		float sqrMagnitude = new Vector2(ix, iz).sqrMagnitude;
		return (Mathf.PerlinNoise(x, y) - 0.5f) * Mathf.Clamp(num + sqrMagnitude / 500f, 1f, 10f);
	}

	private float ph(float ix, float iz, float power, float scale)
	{
		return (Mathf.PerlinNoise(ix * scale, iz * scale) - 0.5f) * power;
	}

	private float MandelHeight(float x, float z)
	{
		float num = 0.03f;
		int num2 = 100;
		float num3 = x * num;
		float num4 = z * num;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		int num9 = 0;
		while (num7 + num8 < 4f && num9 < num2)
		{
			float num10 = num7 - num8 + num3;
			num5 += num5;
			num6 = num5 * num6 + num4;
			num5 = num10;
			num7 = num5 * num5;
			num8 = num6 * num6;
			num9++;
		}
		return -num9 / 40;
	}
}
