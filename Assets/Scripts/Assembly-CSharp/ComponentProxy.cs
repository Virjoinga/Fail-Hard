using UnityEngine;

[PBSerialize("ComponentProxy")]
public class ComponentProxy : MonoBehaviour
{
	[PBSerializeField]
	public Component ToProxy;
}
