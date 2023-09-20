using UnityEngine;

[ExecuteInEditMode]
public class GlobalPerlinModifier : MonoBehaviour
{
	public string modifierTag = "modifiedground";

	public string floaterTag = "groundobject";

	public float heightModifier = 3f;

	public float scaleModifier = 0.15f;

	public bool recalculate;

	private GameObject[] objectsToModify;

	private GameObject[] objectsToFloat;

	private void Start()
	{
		objectsToModify = GameObject.FindGameObjectsWithTag(modifierTag);
		GameObject[] array = objectsToModify;
		foreach (GameObject gameObject in array)
		{
			Modify(gameObject.transform);
		}
	}

	private void Update()
	{
		if (recalculate)
		{
			recalculate = false;
			Start();
		}
	}

	public void Float(Transform target)
	{
		float x = target.position.x;
		float z = target.position.z;
		target.position = new Vector3(x, PerlinHeight(x, z) + 0.5f, z);
	}

	public void Modify(Transform target)
	{
		Vector2 vector = new Vector2(target.position.x, target.position.z);
		MeshFilter component = target.GetComponent<MeshFilter>();
		Vector3[] vertices = component.sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			float iz = vector.y - vertices[i].y;
			vertices[i].z += PerlinHeight(0f, iz);
		}
		component.mesh.vertices = vertices;
		component.mesh.RecalculateBounds();
		component.mesh.RecalculateNormals();
		MeshCollider component2 = target.GetComponent<MeshCollider>();
		if (component2 != null)
		{
			component2.sharedMesh = null;
			component2.sharedMesh = component.mesh;
		}
	}

	private float PerlinHeight(float ix, float iz)
	{
		float num = heightModifier;
		float num2 = scaleModifier;
		float x = ix * num2;
		float y = iz * num2;
		return (Mathf.PerlinNoise(x, y) - 0.5f) * num;
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
