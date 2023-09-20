using System.Collections.Generic;
using UnityEngine;

[PBSerialize("VectorData")]
public class VectorData : MonoBehaviour
{
	[PBSerializeField]
	public List<Vector3> data;
}
