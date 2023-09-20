using Holoville.HOTween;
using UnityEngine;

[PBSerialize("CameraConfigurator")]
public class CameraConfigurator : MonoBehaviour
{
	[PBSerializeField]
	public float minY;

	[PBSerializeField]
	public float maxY;

	[PBSerializeField]
	public float smoothingFactor;

	[PBSerializeField]
	public float speedFactor;

	[PBSerializeField]
	public float zoomFactor;

	[PBSerializeField]
	public float minZoomLevel;

	[PBSerializeField]
	public float maxZoomLevel;

	[PBSerializeField]
	public Vector3 lookAtTargetOffset;

	[PBSerializeField]
	public float cameraOffset;

	[PBSerializeField]
	public float leftLimit;

	[PBSerializeField]
	public float rightLimit;

	[PBSerializeField]
	public float CameraHeightFromGround = 0.5f;

	[PBSerializeField]
	public float tweenSpeed = 2f;

	private CameraFlying currentCamera;

	private void Start()
	{
		GameObject gameObject = GameObject.FindWithTag("MainCamera");
		currentCamera = gameObject.GetComponent<CameraFlying>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.root.name == "ActionJackson(Clone)" || other.transform.root.name == "MopedSharp(Clone)")
		{
			HOTween.To(currentCamera, tweenSpeed, "minY", minY);
			HOTween.To(currentCamera, tweenSpeed, "maxY", maxY);
			HOTween.To(currentCamera, tweenSpeed, "smoothingFactor", smoothingFactor);
			HOTween.To(currentCamera, tweenSpeed, "speedFactor", speedFactor);
			HOTween.To(currentCamera, tweenSpeed, "zoomFactor", zoomFactor);
			HOTween.To(currentCamera, tweenSpeed, "minZoomLevel", minZoomLevel);
			HOTween.To(currentCamera, tweenSpeed, "maxZoomLevel", maxZoomLevel);
			HOTween.To(currentCamera, tweenSpeed, "lookAtTargetOffset", lookAtTargetOffset);
			HOTween.To(currentCamera, tweenSpeed, "cameraOffset", cameraOffset);
			HOTween.To(currentCamera, tweenSpeed, "leftLimit", leftLimit);
			HOTween.To(currentCamera, tweenSpeed, "rightLimit", rightLimit);
			HOTween.To(currentCamera, tweenSpeed, "CameraHeightFromGround", CameraHeightFromGround);
		}
	}
}
