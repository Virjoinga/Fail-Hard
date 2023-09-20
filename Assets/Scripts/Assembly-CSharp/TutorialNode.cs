using UnityEngine;

public class TutorialNode : MonoBehaviour
{
	public GizmoAction guideAction;

	public GizmoAction doneAction;

	public TutorialEvent requiredResponse;

	public int delay;

	private bool triggered;

	private bool responded;

	private bool keyboardControls;

	private void Awake()
	{
		Player.TutorialHook += UserResponse;
	}

	private void Update()
	{
		if (Input.anyKey)
		{
			keyboardControls = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!triggered && !keyboardControls)
		{
			triggered = true;
			Invoke("StopTime", delay);
		}
	}

	private void StopTime()
	{
		MyTweenTime myTweenTime = MyTweenTime.Begin(base.gameObject, 0.1f, 0f);
		myTweenTime.eventReceiver = base.gameObject;
		myTweenTime.callWhenFinished = "Do";
	}

	public void Do()
	{
		GameObject gameObject = GameObject.Find("MopedSharp");
		if (gameObject == null)
		{
			Debug.LogWarning("Could not find AJ from scene!");
			return;
		}
		Vector3 localPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		localPosition.x -= Screen.width / 2;
		localPosition.y -= Screen.height / 2;
		ControlGizmo.instance.gameObject.transform.localPosition = localPosition;
		ControlGizmo.instance.StartAction(guideAction);
	}

	public void UserResponse(TutorialEvent userResponse)
	{
		if (triggered && !responded && userResponse == requiredResponse)
		{
			responded = true;
			ControlGizmo.instance.StartAction(doneAction);
			MyTweenTime.Begin(base.gameObject, 0.1f, 1f);
		}
	}
}
