using UnityEngine;

public class RandomFly : MonoBehaviour
{
	public Vector3 originalPos;

	public Vector3 target;

	private void Start()
	{
		originalPos = base.transform.position;
		InvokeRepeating("NewPosition", 0f, 1f);
	}

	private void Update()
	{
		base.transform.Translate(0.05f * (target - base.transform.position));
	}

	private void NewPosition()
	{
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		target = originalPos + insideUnitSphere;
	}
}
