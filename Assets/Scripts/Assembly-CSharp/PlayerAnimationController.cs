using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
	public delegate void OnPoseReady();

	public Animation animationRoot;

	public AnimationMirroring physicsSync;

	[method: MethodImpl(32)]
	public event OnPoseReady PoseReady;

	private void Start()
	{
		physicsSync.MirroringEnabled += OnMirroring;
	}

	public void AnimatePhysics(string name, WrapMode wrapMode = WrapMode.Loop)
	{
		physicsSync.BlendIntoAnimation();
		animationRoot[name].wrapMode = wrapMode;
		animationRoot[name].speed = 1.5f;
		animationRoot.CrossFade(name);
	}

	public void PosePhysics(string name, WrapMode wrapMode = WrapMode.Loop, float animationPos = 0f)
	{
		physicsSync.SwitchToAnimation();
		animationRoot[name].wrapMode = wrapMode;
		animationRoot[name].speed = 1.5f;
		animationRoot[name].time = animationPos;
		animationRoot.Play(name);
	}

	private void OnMirroring()
	{
		if (this.PoseReady != null)
		{
			this.PoseReady();
		}
	}

	public void DrivingPhysics(bool isDriving)
	{
		physicsSync.DrivingJoints(isDriving);
	}

	public void DisableAnimatedPhysics()
	{
		physicsSync.DisableMirroring();
	}

	public void EnableAnimatedPhysics()
	{
		physicsSync.BlendIntoAnimation();
	}

	public void CollisionEnter(BodyPartType bodyPartType, float impactMagnitude)
	{
		physicsSync.DisableMirroring();
	}

	public void CollisionExit(BodyPartType bodyPartType, float impactMagnitude)
	{
	}
}
