using UnityEngine;

public class WorldOrganizer : MonoBehaviour
{
	public float worldOffset;

	private Transform target;

	private Vector3 activeSlot;

	public float tileWidth = 10f;

	private void Start()
	{
		Transform transform = GameObject.Find("SpawnPoint").transform;
		activeSlot = transform.position + Vector3.forward * worldOffset;
		ReCalculatePlanes();
	}

	private void Update()
	{
		if (target == null)
		{
			GameObject gameObject = GameObject.Find("spine_1");
			if (gameObject != null)
			{
				target = gameObject.transform;
			}
		}
		if (target != null)
		{
			bool flag = false;
			while (target.position.x - activeSlot.x > 0f)
			{
				activeSlot += Vector3.right * tileWidth;
				flag = true;
			}
			if (target.position.x - (activeSlot.x - tileWidth) < 0f)
			{
				activeSlot -= Vector3.right * tileWidth;
				flag = true;
			}
			if (flag)
			{
				ReCalculatePlanes();
			}
		}
	}

	private void ReCalculatePlanes()
	{
		Transform[] array = new Transform[4];
		Vector2[] array2 = new Vector2[4]
		{
			new Vector2(activeSlot.x, activeSlot.z),
			new Vector2(activeSlot.x - tileWidth, activeSlot.z),
			new Vector2(activeSlot.x + tileWidth, activeSlot.z),
			new Vector2(activeSlot.x + 2f * tileWidth, activeSlot.z)
		};
		bool[] array3 = new bool[4];
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			bool flag = false;
			for (int j = 0; j < array2.Length; j++)
			{
				if (!array3[j] && IsSame(child, array2[j]))
				{
					flag = (array3[j] = true);
					break;
				}
			}
			if (!flag)
			{
				array[num++] = child;
			}
		}
		num = 0;
		for (int k = 0; k < array2.Length; k++)
		{
			if (!array3[k])
			{
				Vector2 vector = array2[k];
				Transform transform = array[num++];
				transform.position = new Vector3(vector.x, 0f, vector.y);
				transform.GetComponentInChildren<PerlinModifier>().Calculate(vector.x, vector.y);
			}
		}
	}

	private bool IsSame(Transform t, Vector2 pos)
	{
		return Mathf.Approximately(t.position.x, pos.x) && Mathf.Approximately(t.position.z, pos.y);
	}
}
