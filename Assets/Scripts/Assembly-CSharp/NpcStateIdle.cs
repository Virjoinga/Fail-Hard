public class NpcStateIdle : NpcState
{
	public override void UpdateFunc()
	{
	}

	public override void FixedUpdateFunc()
	{
	}

	public override void Enter()
	{
		pac.DisableAnimatedPhysics();
		base.rigidbody.isKinematic = false;
		Npc.ConnectedObject.rigidbody.isKinematic = false;
	}
}
