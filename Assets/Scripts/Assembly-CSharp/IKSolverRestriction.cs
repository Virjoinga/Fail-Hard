using System;
using UnityEngine;

public class IKSolverRestriction : MonoBehaviour
{
	[Serializable]
	public class BoneEntity
	{
		public Transform bone;

		public Transform bodyPart;

		public bool restrictionEnabled;

		public Restriction restrictionRange;
	}

	[Serializable]
	public class Restriction
	{
		public float xMin;

		public float xMax = 360f;

		public float yMin;

		public float yMax = 360f;

		public float zMin;

		public float zMax = 360f;
	}

	public Transform target;

	public BoneEntity[] boneEntity;

	public bool damping;

	public float dampingMax = 0.5f;

	private float IK_POS_THRESH = 0.00125f;

	private int MAX_IK_TRIES = 20;

	private bool ikEnabled = true;

	private void Start()
	{
		if (target == null)
		{
			target = base.transform;
		}
		BoneEntity[] array = this.boneEntity;
		foreach (BoneEntity boneEntity in array)
		{
			boneEntity.bone.position = boneEntity.bodyPart.position;
			boneEntity.bone.rotation = boneEntity.bodyPart.rotation;
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.R))
		{
			TogglePauseIK();
		}
	}

	private void LateUpdate()
	{
		if (ikEnabled)
		{
			Solve();
		}
	}

	public void Solve()
	{
		Transform bone = boneEntity[boneEntity.Length - 1].bone;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		Vector3 zero4 = Vector3.zero;
		Vector3 zero5 = Vector3.zero;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = boneEntity.Length - 2;
		int num5 = 0;
		zero2 = bone.position;
		while (num5 < MAX_IK_TRIES && (zero2 - target.position).sqrMagnitude > IK_POS_THRESH)
		{
			if (num4 < 0)
			{
				num4 = boneEntity.Length - 2;
			}
			zero = boneEntity[num4].bone.position;
			zero2 = bone.position;
			zero4 = zero2 - zero;
			zero3 = target.position - zero;
			zero4.Normalize();
			zero3.Normalize();
			num = Vector3.Dot(zero4, zero3);
			if (num < 0.99999f)
			{
				zero5 = Vector3.Cross(zero4, zero3);
				zero5.Normalize();
				num2 = Mathf.Acos(num);
				num3 = num2 * 57.29578f;
				if (damping)
				{
					if (num2 > dampingMax)
					{
						num2 = dampingMax;
					}
					num3 = num2 * 57.29578f;
				}
				boneEntity[num4].bone.rotation = Quaternion.AngleAxis(num3, zero5) * boneEntity[num4].bone.rotation;
				if (boneEntity[num4].restrictionEnabled)
				{
					CheckRestrictions(boneEntity[num4]);
				}
			}
			num5++;
			num4--;
		}
	}

	private void CheckRestrictions(BoneEntity boneEntity)
	{
		Vector3 eulerAngles = boneEntity.bone.localRotation.eulerAngles;
		if (eulerAngles.x > 180f)
		{
			eulerAngles.x -= 360f;
		}
		if (eulerAngles.y > 180f)
		{
			eulerAngles.y -= 360f;
		}
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		if (eulerAngles.x > boneEntity.restrictionRange.xMax)
		{
			eulerAngles.x = boneEntity.restrictionRange.xMax;
		}
		if (eulerAngles.x < boneEntity.restrictionRange.xMin)
		{
			eulerAngles.x = boneEntity.restrictionRange.xMin;
		}
		if (eulerAngles.y > boneEntity.restrictionRange.yMax)
		{
			eulerAngles.y = boneEntity.restrictionRange.yMax;
		}
		if (eulerAngles.y < boneEntity.restrictionRange.yMin)
		{
			eulerAngles.y = boneEntity.restrictionRange.yMin;
		}
		if (eulerAngles.z > boneEntity.restrictionRange.zMax)
		{
			eulerAngles.z = boneEntity.restrictionRange.zMax;
		}
		if (eulerAngles.z < boneEntity.restrictionRange.zMin)
		{
			eulerAngles.z = boneEntity.restrictionRange.zMin;
		}
		boneEntity.bone.localRotation = Quaternion.Euler(eulerAngles);
	}

	private void TogglePauseIK()
	{
		if (ikEnabled)
		{
			ikEnabled = false;
			return;
		}
		ikEnabled = true;
		BoneEntity[] array = this.boneEntity;
		foreach (BoneEntity boneEntity in array)
		{
			boneEntity.bone.position = boneEntity.bodyPart.position;
		}
	}
}
