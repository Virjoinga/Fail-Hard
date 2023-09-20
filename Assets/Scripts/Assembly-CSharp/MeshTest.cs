using UnityEngine;

public class MeshTest : MonoBehaviour
{
	public int Segments = 15;

	private Vector3[] newVertices;

	private Vector2[] newUV;

	private int[] sideTriangles;

	private int[] topTriangles;

	public GameObject GrassPrefab;

	private void Start()
	{
		CreateSpline();
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GenerateVertices(Segments);
		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.subMeshCount = 2;
		mesh.SetTriangles(sideTriangles, 0);
		mesh.SetTriangles(topTriangles, 1);
		base.gameObject.AddComponent<MeshCollider>();
		GenerateGrass(3);
	}

	private void CreateSpline()
	{
		foreach (Vector3 datum in GetComponent<VectorData>().data)
		{
			GameObject gameObject = GetComponent<Spline>().AddSplineNode();
			gameObject.transform.position = datum;
		}
	}

	private Vector3 GetPosition(float u)
	{
		return GetComponent<Spline>().GetPositionOnSpline(u);
	}

	private void GenerateGrass(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			float param = Random.Range(0f, 1f);
			GameObject gameObject = Object.Instantiate(GrassPrefab, base.transform.position + GetComponent<Spline>().GetPositionOnSpline(param), Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition += Random.Range(-0.4f, 0.4f) * Vector3.forward;
			gameObject.transform.localScale *= Random.Range(0.4f, 0.6f);
			Vector3 tangentToSpline = GetComponent<Spline>().GetTangentToSpline(param);
			Quaternion quaternion = Quaternion.FromToRotation(gameObject.transform.right, tangentToSpline);
			gameObject.transform.rotation *= quaternion;
		}
	}

	private void GenerateVertices(int slices)
	{
		newVertices = new Vector3[3 * slices];
		newUV = new Vector2[3 * slices];
		sideTriangles = new int[6 * slices];
		topTriangles = new int[6 * slices];
		for (int i = 0; i < slices; i++)
		{
			float u = (float)i / (float)(slices - 1);
			Vector3 position = GetPosition(u);
			newVertices[3 * i] = position;
			newVertices[3 * i].z = 0.5f;
			newVertices[3 * i + 1] = position;
			newVertices[3 * i + 1].z = -0.5f;
			newVertices[3 * i + 2] = position;
			newVertices[3 * i + 2].y -= 0.2f;
			newVertices[3 * i + 2].y = 0f;
			newVertices[3 * i + 2].z = -0.5f;
			float num = 0.2f;
			newUV[3 * i].x = position.x / num;
			newUV[3 * i].y = 0f;
			newUV[3 * i + 1].x = position.x / num;
			newUV[3 * i + 1].y = 1f;
			newUV[3 * i + 2].x = position.x / num;
			newUV[3 * i + 2].y = 0f;
			if (i > 0)
			{
				topTriangles[6 * (i - 1)] = 3 * (i - 1);
				topTriangles[6 * (i - 1) + 1] = 3 * i;
				topTriangles[6 * (i - 1) + 2] = 3 * (i - 1) + 1;
				topTriangles[6 * (i - 1) + 3] = 3 * i;
				topTriangles[6 * (i - 1) + 4] = 3 * i + 1;
				topTriangles[6 * (i - 1) + 5] = 3 * (i - 1) + 1;
				sideTriangles[6 * (i - 1)] = 3 * (i - 1) + 1;
				sideTriangles[6 * (i - 1) + 1] = 3 * i + 1;
				sideTriangles[6 * (i - 1) + 2] = 3 * (i - 1) + 2;
				sideTriangles[6 * (i - 1) + 3] = 3 * i + 1;
				sideTriangles[6 * (i - 1) + 4] = 3 * i + 2;
				sideTriangles[6 * (i - 1) + 5] = 3 * (i - 1) + 2;
			}
		}
	}
}
