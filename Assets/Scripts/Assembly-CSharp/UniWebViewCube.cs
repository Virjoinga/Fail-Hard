using UnityEngine;

public class UniWebViewCube : MonoBehaviour
{
	public UniWebDemo webViewDemo;

	private float startTime;

	private bool firstHit = true;

	private void Start()
	{
		startTime = Time.time;
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Target")
		{
			webViewDemo.ShowAlertInWebview(Time.time - startTime, firstHit);
			firstHit = false;
		}
	}
}
