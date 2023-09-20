using UnityEngine;

public class StayTargetTriggerVolume : MonoBehaviour
{
	private StayTarget target;

	private void Start()
	{
		target = base.transform.parent.GetComponent<StayTarget>();
	}

	private void OnTriggerEnter(Collider hit)
	{
		target.OnTriggerEnter(hit);
	}

	private void OnTriggerExit(Collider hit)
	{
		target.OnTriggerExit(hit);
	}
}
