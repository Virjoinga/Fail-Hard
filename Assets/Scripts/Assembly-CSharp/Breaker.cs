using UnityEngine;

[PBSerialize("Breaker")]
public class Breaker : MonoBehaviour
{
	[PBSerializeField]
	public bool triggerOnPlayer = true;

	[PBSerializeField]
	public bool triggerOnLevelObject = true;

	[PBSerializeField]
	public bool triggerOnce = true;

	private bool triggered;

	private void OnTriggerEnter(Collider hit)
	{
		if (triggerOnce && triggered)
		{
			return;
		}
		BreakingGlass[] componentsInChildren = base.transform.parent.GetComponentsInChildren<BreakingGlass>();
		foreach (BreakingGlass breakingGlass in componentsInChildren)
		{
			if (triggerOnPlayer && (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player"))
			{
				breakingGlass.Break();
				triggered = true;
			}
			if (triggerOnLevelObject && hit.gameObject.tag == "DynamicLevelObject")
			{
				breakingGlass.Break();
				triggered = true;
			}
		}
		BreakingStuff[] componentsInChildren2 = base.transform.parent.GetComponentsInChildren<BreakingStuff>();
		foreach (BreakingStuff breakingStuff in componentsInChildren2)
		{
			if (triggerOnPlayer && (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player"))
			{
				breakingStuff.Break();
				triggered = true;
			}
			if (triggerOnLevelObject && hit.gameObject.tag == "DynamicLevelObject")
			{
				breakingStuff.Break();
				triggered = true;
			}
		}
	}
}
