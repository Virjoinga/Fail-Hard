using Game;
using UnityEngine;

public class CounterUpdater : MonoBehaviour
{
	public bool read = true;

	public bool write = true;

	public string id = "invalid";

	private Counter cvalue;

	public int Value
	{
		get
		{
			if (cvalue == null)
			{
				cvalue = Storage.Instance.LoadCounter(id);
			}
			if (cvalue == null)
			{
				cvalue = new Counter();
			}
			return cvalue.Value;
		}
		set
		{
			if (cvalue == null)
			{
				cvalue = Storage.Instance.LoadCounter(id);
			}
			if (cvalue == null)
			{
				cvalue = new Counter();
			}
			cvalue.Value = value;
			if (write)
			{
				Storage.Instance.UpdateCounter(id);
			}
		}
	}

	public void FetchValue()
	{
		cvalue = Storage.Instance.LoadCounter(id);
	}

	private void Start()
	{
		if (read)
		{
			FetchValue();
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		Value++;
	}

	private void OnCollisionEnter(Collision c)
	{
		Value++;
	}
}
