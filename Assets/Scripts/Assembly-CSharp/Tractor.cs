using UnityEngine;

[PBSerialize("Tractor")]
public class Tractor : MonoBehaviour
{
	[PBSerializeField]
	public float Part2Angle;

	[PBSerializeField]
	public float Part3Angle;

	[PBSerializeField]
	public float Part4Angle;

	public Transform ShovelPart2;

	public Transform ShovelPart3;

	public Transform ShovelPart4;

	private void Start()
	{
		ShovelPart2.Rotate(0f, Part2Angle, 0f);
		ShovelPart3.Rotate(0f, Part3Angle, 0f);
		ShovelPart4.Rotate(0f, Part4Angle, 0f);
	}
}
