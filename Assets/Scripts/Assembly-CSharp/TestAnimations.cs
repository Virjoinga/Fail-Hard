using UnityEngine;

public class TestAnimations : MonoBehaviour
{
	public AnimationClip Pose1;

	public AnimationClip Pose2;

	public float BlendSpeed;

	private void Start()
	{
		base.animation.AddClip(Pose1, "pose1");
		base.animation.AddClip(Pose2, "pose2");
		base.animation["pose1"].blendMode = AnimationBlendMode.Blend;
		base.animation["pose1"].wrapMode = WrapMode.Loop;
		base.animation["pose2"].blendMode = AnimationBlendMode.Blend;
		base.animation["pose2"].wrapMode = WrapMode.Loop;
		base.animation["pose2"].normalizedTime = 1f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if (base.animation.IsPlaying("pose1"))
			{
				base.animation.Stop("pose1");
			}
			else
			{
				base.animation.Play("pose1");
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			base.animation.Blend("pose2", 1f);
			base.animation["pose2"].speed = 1f;
		}
		float num = Time.deltaTime / BlendSpeed;
		if (Input.GetKey(KeyCode.D))
		{
			base.animation["pose1"].weight = Mathf.Clamp(base.animation["pose1"].weight - num, 0f, 1f);
			base.animation["pose2"].weight = Mathf.Clamp(base.animation["pose2"].weight + num, 0f, 1f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.animation["pose1"].weight = Mathf.Clamp(base.animation["pose1"].weight + num, 0f, 1f);
			base.animation["pose2"].weight = Mathf.Clamp(base.animation["pose2"].weight - num, 0f, 1f);
		}
	}
}
