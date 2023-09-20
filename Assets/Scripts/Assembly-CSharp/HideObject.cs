using UnityEngine;

[PBSerialize("HideObject")]
public class HideObject : MonoBehaviour
{
	[PBSerializeField]
	public GameObjectId ObjectId;

	[PBSerializeField]
	public string ObjectName;

	private void Start()
	{
		GameObject gameObject = ((!(ObjectName != string.Empty)) ? GameObjectId.Find(ObjectId) : GameObject.Find(ObjectName));
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}
}
