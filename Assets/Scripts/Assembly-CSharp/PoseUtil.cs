using UnityEngine;

public class PoseUtil : MonoBehaviour
{
	public Npc Npc;

	public GameObject obj;

	private void Start()
	{
		Invoke("Pose", 0.01f);
	}

	private void Pose()
	{
		Npc.AssignObject(obj);
	}
}
