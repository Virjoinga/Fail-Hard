using UnityEngine;

public class Trail : MonoBehaviour
{
	private int vertexCount;

	public GameObject trailPart;

	public GameObject trailSplash;

	private Vector3 lastPoint;

	private bool drawSplash;

	private SurfaceMaterialType material;

	private float startTime;

	public void setMaterial(SurfaceMaterialType material)
	{
		this.material = material;
		drawSplash = true;
		if (material == SurfaceMaterialType.Asphalt)
		{
			drawSplash = false;
		}
	}

	public SurfaceMaterialType getMaterial()
	{
		return material;
	}

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		if (base.transform.childCount == 0 && Time.time - startTime > 1f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void doSplash(Vector3 point, Quaternion lookAt)
	{
		if (drawSplash)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(trailSplash, point, lookAt);
			gameObject.transform.Rotate(0f, Random.Range(0, 360), 0f);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localScale = gameObject.transform.localScale * Random.Range(0.5f, 1.2f);
		}
	}

	public void addVertex(Vector3 point)
	{
		vertexCount++;
		Quaternion quaternion = Quaternion.identity;
		if (vertexCount > 1)
		{
			Vector3 position = (lastPoint + point) / 2f;
			quaternion = Quaternion.LookRotation(point - lastPoint);
			GameObject gameObject = (GameObject)Object.Instantiate(trailPart, position, quaternion);
			float magnitude = (point - lastPoint).magnitude;
			gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, magnitude);
			gameObject.transform.parent = base.transform;
			doSplash(point, quaternion);
		}
		if (vertexCount == 2)
		{
			doSplash(lastPoint, quaternion);
		}
		lastPoint = point;
	}
}
