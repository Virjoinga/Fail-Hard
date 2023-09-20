using UnityEngine;

public class TriggerVolume : MonoBehaviour
{
	private TargetZone tz;

	private void Start()
	{
		tz = base.transform.parent.GetComponent<TargetZone>();
	}

	private void OnTriggerEnter(Collider hit)
	{
		tz.OnTriggerEnter(hit);
	}
}
