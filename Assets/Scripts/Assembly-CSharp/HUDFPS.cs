using UnityEngine;

[RequireComponent(typeof(UILabel))]
public class HUDFPS : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private UILabel label;

	private void Start()
	{
		label = GetComponent<UILabel>();
		if (!label)
		{
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F0}", num);
			label.text = text;
			if (num < 30f)
			{
				label.color = Color.yellow;
			}
			else if (num < 10f)
			{
				label.color = Color.red;
			}
			else
			{
				label.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
