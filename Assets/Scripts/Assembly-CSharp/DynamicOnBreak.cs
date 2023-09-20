using UnityEngine;

public class DynamicOnBreak : BreakingStuff
{
	public override void Break()
	{
		if (base.rigidbody != null)
		{
			base.rigidbody.isKinematic = false;
		}
		if (base.collider != null)
		{
			base.collider.enabled = true;
		}
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			rigidbody.isKinematic = false;
			rigidbody.collider.enabled = true;
			rigidbody.WakeUp();
		}
	}
}
