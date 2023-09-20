using UnityEngine;

public class AnimationUtil : MonoBehaviour
{
	public PlayerAnimationController pac;

	public AnimationMirroring am;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			pac.AnimatePhysics("IdleHead_01");
		}
		if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			pac.AnimatePhysics("freefall");
		}
		if (Input.GetKeyUp(KeyCode.Alpha3))
		{
			pac.AnimatePhysics("PoseMoped_01");
		}
		if (Input.GetKeyUp(KeyCode.Alpha4))
		{
			pac.AnimatePhysics("Animation_crashonhead");
		}
		if (Input.GetKeyUp(KeyCode.Alpha0))
		{
			pac.DisableAnimatedPhysics();
		}
	}
}
