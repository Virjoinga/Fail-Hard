using UnityEngine;

namespace Game
{
	public class Pose : MonoBehaviour
	{
		private ConfigurableJoint legR;

		private HingeJoint kneeR;

		private ConfigurableJoint legL;

		private HingeJoint kneeL;

		private ConfigurableJoint shoulderL;

		private ConfigurableJoint shoulderR;

		private HingeJoint elbowL;

		private HingeJoint elbowR;

		private GameObject footR;

		private GameObject footL;

		private GameObject handR;

		private GameObject handL;

		private GameObject head;

		public GameObject vehicle;

		private HingeJoint footRToVehicle;

		private HingeJoint footLToVehicle;

		private HingeJoint handRToVehicle;

		private HingeJoint handLToVehicle;

		public float legRAngle;

		public float legRAngle2;

		public float legLAngle;

		public float legLAngle2;

		public float kneeRAngle;

		public float kneeLAngle;

		public float shoulderRAngle;

		public float shoulderLAngle;

		public float elbowRAngle;

		public float elbowLAngle;

		public bool poseTesting;

		public BodyPartAnimator handRAnimator;

		private void AssignJoints()
		{
			legR = GameObject.Find("leg_2_R").GetComponent<ConfigurableJoint>();
			legL = GameObject.Find("leg_2_L").GetComponent<ConfigurableJoint>();
			kneeR = GameObject.Find("leg_3_R").GetComponent<HingeJoint>();
			kneeL = GameObject.Find("leg_3_L").GetComponent<HingeJoint>();
			shoulderR = GameObject.Find("hand_2_R").GetComponent<ConfigurableJoint>();
			shoulderL = GameObject.Find("hand_2_L").GetComponent<ConfigurableJoint>();
			elbowR = GameObject.Find("hand_3_R").GetComponent<HingeJoint>();
			elbowL = GameObject.Find("hand_3_L").GetComponent<HingeJoint>();
			footR = GameObject.Find("leg_4_R");
			footL = GameObject.Find("leg_4_L");
			handR = GameObject.Find("palm_2_R");
			handL = GameObject.Find("palm_2_L");
			head = GameObject.Find("head");
		}

		private void Update()
		{
		}

		private void PrepareForPose()
		{
			AssignJoints();
			EnableColliders(false);
			shoulderL.gameObject.rigidbody.velocity = Vector3.zero;
			shoulderL.gameObject.rigidbody.angularVelocity = Vector3.zero;
			shoulderR.gameObject.rigidbody.velocity = Vector3.zero;
			shoulderR.gameObject.rigidbody.angularVelocity = Vector3.zero;
			elbowL.gameObject.rigidbody.velocity = Vector3.zero;
			elbowL.gameObject.rigidbody.angularVelocity = Vector3.zero;
			elbowR.gameObject.rigidbody.velocity = Vector3.zero;
			elbowR.gameObject.rigidbody.angularVelocity = Vector3.zero;
		}

		private void EnableColliders(bool enabled)
		{
			base.collider.enabled = enabled;
			legR.gameObject.collider.enabled = enabled;
			legL.gameObject.collider.enabled = enabled;
			kneeR.gameObject.collider.enabled = enabled;
			kneeL.gameObject.collider.enabled = enabled;
			footR.collider.enabled = enabled;
			footL.collider.enabled = enabled;
			handR.collider.enabled = enabled;
			handL.collider.enabled = enabled;
		}

		public void PoseInVehicle(VehicleBase currentVehicle)
		{
			PrepareForPose();
			PositionPlayerInVehicle(currentVehicle);
			RelaxAll();
			currentVehicle.GetPoseIK().FootR.GetComponent<VehiclePoseIKTarget>().AttachToVehicle(footR.rigidbody, currentVehicle.gameObject);
			currentVehicle.GetPoseIK().FootL.GetComponent<VehiclePoseIKTarget>().AttachToVehicle(footL.rigidbody, currentVehicle.gameObject);
			currentVehicle.GetPoseIK().FistR.GetComponent<VehiclePoseIKTarget>().AttachToVehicle(handR.rigidbody, currentVehicle.gameObject);
			currentVehicle.GetPoseIK().FistL.GetComponent<VehiclePoseIKTarget>().AttachToVehicle(handL.rigidbody, currentVehicle.gameObject);
			Invoke("PoseReady", 1f);
		}

		public void PositionPlayerInVehicle(VehicleBase currentVehicle)
		{
			vehicle = currentVehicle.gameObject;
			base.transform.parent.parent.transform.LookAt(base.transform.parent.parent.transform.position + currentVehicle.GetPoseIK().GetRoot().right);
			Vector3 translation = currentVehicle.GetPoseIK().GetRoot().position - base.rigidbody.position;
			base.transform.parent.parent.transform.Translate(translation, Space.World);
			base.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}

		public HingeJoint AttachToVehicle(GameObject obj)
		{
			HingeJoint hingeJoint = obj.AddComponent("HingeJoint") as HingeJoint;
			hingeJoint.axis = Vector3.forward;
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.connectedBody = vehicle.rigidbody;
			return hingeJoint;
		}

		public void DetachFromVehicle()
		{
			Object.Destroy(footRToVehicle);
			Object.Destroy(footLToVehicle);
			Object.Destroy(handRToVehicle);
			Object.Destroy(handLToVehicle);
			if ((bool)handRAnimator)
			{
				handRAnimator.Enable();
			}
		}

		private void PoseReady()
		{
			if (!poseTesting)
			{
				footRToVehicle = AttachToVehicle(footR);
				footLToVehicle = AttachToVehicle(footL);
				Object.Destroy(vehicle.GetComponent<VehicleBase>().GetPoseIK().FootR.GetComponent<VehiclePoseIKTarget>().hj);
				Object.Destroy(vehicle.GetComponent<VehicleBase>().GetPoseIK().FootL.GetComponent<VehiclePoseIKTarget>().hj);
				handRToVehicle = AttachToVehicle(handR);
				handLToVehicle = AttachToVehicle(handL);
				Object.Destroy(vehicle.GetComponent<VehicleBase>().GetPoseIK().FistR.GetComponent<VehiclePoseIKTarget>().hj);
				Object.Destroy(vehicle.GetComponent<VehicleBase>().GetPoseIK().FistL.GetComponent<VehiclePoseIKTarget>().hj);
				vehicle.rigidbody.isKinematic = false;
				vehicle.rigidbody.constraints = (RigidbodyConstraints)56;
			}
			EnableColliders(true);
			RemoveConstraints();
			GetComponent<SkinnedMeshRenderer>().enabled = true;
		}

		private void RelaxAll()
		{
			JointDrive angularXDrive = legR.angularXDrive;
			angularXDrive.mode = JointDriveMode.None;
			angularXDrive.positionSpring = 1000f;
			angularXDrive.positionDamper = 10f;
			legR.angularXDrive = angularXDrive;
			angularXDrive = legR.angularYZDrive;
			angularXDrive.mode = JointDriveMode.None;
			angularXDrive.positionSpring = 1000f;
			angularXDrive.positionDamper = 10f;
			legR.angularYZDrive = angularXDrive;
			angularXDrive = legL.angularXDrive;
			angularXDrive.mode = JointDriveMode.None;
			angularXDrive.positionSpring = 1000f;
			angularXDrive.positionDamper = 10f;
			legL.angularXDrive = angularXDrive;
			angularXDrive = legL.angularYZDrive;
			angularXDrive.mode = JointDriveMode.None;
			angularXDrive.positionSpring = 1000f;
			angularXDrive.positionDamper = 10f;
			legL.angularYZDrive = angularXDrive;
			kneeR.useSpring = false;
			kneeL.useSpring = false;
			elbowL.useSpring = false;
			elbowR.useSpring = false;
		}

		private void RemoveConstraints()
		{
			head.rigidbody.constraints = RigidbodyConstraints.None;
			base.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		}
	}
}
