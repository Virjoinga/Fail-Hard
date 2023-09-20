using UnityEngine;

[PBSerialize("StaticPerlinModifier")]
public class StaticPerlinModifier : MonoBehaviour
{
	private Transform floaters;

	[PBSerializeField]
	public bool blender;

	[PBSerializeField]
	public bool noZChange;

	[PBSerializeField]
	public float xOffset;

	[PBSerializeField]
	public float zOffset;

	[PBSerializeField]
	public bool randomizeOffsets;

	private Vector3[] original;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("floaters");
		if (gameObject != null)
		{
			floaters = gameObject.transform;
		}
		if (randomizeOffsets)
		{
			xOffset = Random.value * 100f;
			zOffset = Random.value * 100f;
		}
		Calculate(base.transform.position.x, base.transform.position.z);
	}

	public void Calculate(float x, float z)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Debug.LogWarning("Adjusting " + child.name);
			MeshFilter component = child.GetComponent<MeshFilter>();
			Vector3[] vertices = component.mesh.vertices;
			Vector3[] normals = component.mesh.normals;
			for (int j = 0; j < vertices.Length; j++)
			{
				if (blender)
				{
					float ix = x + vertices[j].y;
					float num = z + vertices[j].x;
					vertices[j].z = original[j].z + ComplexPerlinHeight(ix, (!noZChange) ? num : 0f);
					normals[j] = CalculateNormal(ix, num);
				}
				else
				{
					float ix = x + vertices[j].x;
					float num = z + vertices[j].z;
					vertices[j].y = ComplexPerlinHeight(ix, num);
					normals[j] = CalculateNormal(ix, num);
				}
			}
			component.mesh.vertices = vertices;
			component.mesh.normals = normals;
			component.mesh.RecalculateBounds();
			MeshCollider component2 = child.GetComponent<MeshCollider>();
			if (component2 != null)
			{
				component2.sharedMesh = null;
				component2.sharedMesh = component.mesh;
			}
		}
		for (int k = 0; k < floaters.childCount; k++)
		{
			Transform child2 = floaters.GetChild(k);
			float ix2 = x + child2.position.x;
			float iz = z + child2.position.z;
			child2.position = new Vector3(child2.position.x, ComplexPerlinHeight(ix2, iz), child2.position.z);
		}
	}

	private float ComplexPerlinHeight(float ix, float iz)
	{
		float num = ix - base.transform.position.x;
		float num2 = iz - base.transform.position.z;
		return ph(ix, iz, 3f, 0.2f) + ph(ix, iz, (!(num > -8f)) ? 0f : Mathf.Clamp((num + 8f) * 0.5f, 0f, 4f), 0.12f) + ph(ix, iz, (!(num > 2f)) ? 0f : Mathf.Clamp((num - 2f) * 0.2f, 0f, 1f), 0.25f) + -0.04f * Mathf.Clamp(0f - num2, 0f, 100f) * Mathf.Clamp(0f - num2, 0f, 100f);
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
		return Mathf.PerlinNoise((ix + xOffset) * scale, (iz + zOffset) * scale) * power;
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
