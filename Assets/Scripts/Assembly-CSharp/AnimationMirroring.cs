using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimationMirroring : MonoBehaviour
{
	public enum MirroringState
	{
		Disabled = 0,
		BlendIn = 1,
		BlendOut = 2,
		Mirroring = 3
	}

	public delegate void OnMirroring();

	public Rigidbody[] targetBodies;

	public Rigidbody[] sourceBodies;

	private MirroringState mirroringState;

	private float blendingSlerp;

	private float blendingTime = 1f;

	private bool m_mirroringRequested;

	private List<ConfigurableJointMotion> m_cachedJointMotions;

	[method: MethodImpl(32)]
	public event OnMirroring MirroringEnabled;

	private void LateUpdate()
	{
		if (mirroringState == MirroringState.Disabled)
		{
			return;
		}
		if (mirroringState == MirroringState.Mirroring)
		{
			Mirror();
			if (m_mirroringRequested)
			{
				if (this.MirroringEnabled != null)
				{
					this.MirroringEnabled();
				}
				m_mirroringRequested = false;
			}
		}
		else if (mirroringState == MirroringState.BlendIn)
		{
			blendingSlerp += Time.deltaTime / blendingTime;
			BlendIn(blendingSlerp);
			if (blendingSlerp >= blendingTime)
			{
				mirroringState = MirroringState.Mirroring;
			}
		}
	}

	public void DisableMirroring()
	{
		mirroringState = MirroringState.Disabled;
		EnableJoints();
	}

	public void EnableMirroring()
	{
		mirroringState = MirroringState.Mirroring;
		DisableJoints();
		m_mirroringRequested = true;
	}

	public void BlendIntoAnimation()
	{
		if (mirroringState != MirroringState.BlendIn && mirroringState != MirroringState.Mirroring)
		{
			DisableJoints();
			mirroringState = MirroringState.BlendIn;
			blendingSlerp = 0f;
			m_mirroringRequested = true;
		}
	}

	public void SwitchToAnimation()
	{
		if (mirroringState != MirroringState.BlendIn && mirroringState != MirroringState.Mirroring)
		{
			DisableJoints();
			mirroringState = MirroringState.Mirroring;
			m_mirroringRequested = true;
		}
	}

	private void Mirror()
	{
		for (int i = 0; i < targetBodies.GetLength(0); i++)
		{
			targetBodies[i].transform.localRotation = sourceBodies[i].transform.localRotation;
			targetBodies[i].transform.localPosition = sourceBodies[i].transform.localPosition;
			targetBodies[i].velocity = base.rigidbody.GetRelativePointVelocity(targetBodies[i].transform.localPosition) + sourceBodies[i].velocity;
			targetBodies[i].angularVelocity = sourceBodies[i].angularVelocity;
		}
	}

	private void BlendIn(float u)
	{
		for (int i = 0; i < targetBodies.GetLength(0); i++)
		{
			targetBodies[i].transform.localRotation = Quaternion.Lerp(targetBodies[i].transform.localRotation, sourceBodies[i].transform.localRotation, u);
			targetBodies[i].transform.localPosition = Vector3.Lerp(targetBodies[i].transform.localPosition, sourceBodies[i].transform.localPosition, u);
			targetBodies[i].velocity = base.rigidbody.GetRelativePointVelocity(targetBodies[i].transform.localPosition) + sourceBodies[i].velocity;
			targetBodies[i].angularVelocity = sourceBodies[i].angularVelocity;
		}
	}

	private void DisableJoints()
	{
		bool flag = false;
		if (m_cachedJointMotions == null)
		{
			m_cachedJointMotions = new List<ConfigurableJointMotion>();
			flag = true;
		}
		for (int i = 0; i < targetBodies.GetLength(0); i++)
		{
			ConfigurableJoint component = targetBodies[i].GetComponent<ConfigurableJoint>();
			if (component != null)
			{
				component.xMotion = ConfigurableJointMotion.Free;
				component.yMotion = ConfigurableJointMotion.Free;
				component.yMotion = ConfigurableJointMotion.Free;
				if (flag)
				{
					m_cachedJointMotions.Add(component.angularXMotion);
					m_cachedJointMotions.Add(component.angularYMotion);
					m_cachedJointMotions.Add(component.angularZMotion);
				}
				component.angularXMotion = ConfigurableJointMotion.Free;
				component.angularYMotion = ConfigurableJointMotion.Free;
				component.angularZMotion = ConfigurableJointMotion.Free;
			}
		}
	}

	private void EnableJoints()
	{
		for (int i = 0; i < targetBodies.GetLength(0); i++)
		{
			ConfigurableJoint component = targetBodies[i].GetComponent<ConfigurableJoint>();
			if (component != null)
			{
				component.xMotion = ConfigurableJointMotion.Locked;
				component.yMotion = ConfigurableJointMotion.Locked;
				component.yMotion = ConfigurableJointMotion.Locked;
				if (m_cachedJointMotions.Count == 3 * targetBodies.GetLength(0))
				{
					component.angularXMotion = m_cachedJointMotions[3 * i];
					component.angularYMotion = m_cachedJointMotions[3 * i + 1];
					component.angularZMotion = m_cachedJointMotions[3 * i + 2];
				}
				else
				{
					component.angularXMotion = ConfigurableJointMotion.Limited;
					component.angularYMotion = ConfigurableJointMotion.Limited;
					component.angularZMotion = ConfigurableJointMotion.Limited;
				}
			}
		}
	}

	public void DrivingJoints(bool isDriving)
	{
		for (int i = 0; i < targetBodies.GetLength(0); i++)
		{
			ConfigurableJoint component = targetBodies[i].GetComponent<ConfigurableJoint>();
			if (component != null && targetBodies[i].gameObject.name.Contains("leg_2_"))
			{
				JointDrive angularXDrive = component.angularXDrive;
				if (isDriving)
				{
					angularXDrive.mode = JointDriveMode.None;
				}
				else
				{
					angularXDrive.mode = JointDriveMode.Velocity;
				}
				component.angularXDrive = angularXDrive;
			}
		}
	}
}
