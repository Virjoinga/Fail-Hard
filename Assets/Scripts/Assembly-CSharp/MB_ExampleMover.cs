using UnityEngine;

public class MB_ExampleMover : MonoBehaviour
{
	public int axis;

	private void Update()
	{
		Vector3 position = new Vector3(5f, 5f, 5f);
		int index;
		int index2 = (index = axis);
		float num = position[index];
		position[index2] = num * Mathf.Sin(Time.time);
		base.transform.position = position;
	}
}
