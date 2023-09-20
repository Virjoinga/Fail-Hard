using UnityEngine;

public class TutorialInit : MonoBehaviour
{
	private int happened;

	private void Awake()
	{
		Player.TutorialHook += UserResponse;
	}

	private void OnDestroy()
	{
		Player.TutorialHook -= UserResponse;
	}

	private void SetGizmoPosition()
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
	}

	public void UserResponse(TutorialEvent tutorialEvent)
	{
		switch (tutorialEvent)
		{
		case TutorialEvent.StartMoving:
			if (((uint)happened & 8u) != 0 && (happened & 4) <= 0)
			{
				happened |= 4;
				MyTweenTime.Begin(base.gameObject, 0.1f, 1f);
				ControlGizmo.instance.RightOk();
			}
			break;
		case TutorialEvent.StartMovingHint:
			if (((uint)happened & 0x10u) != 0 && (happened & 8) <= 0)
			{
				happened |= 8;
				ControlGizmo.instance.MiddleOk();
			}
			break;
		case TutorialEvent.Begin:
			if ((happened & 0x10) <= 0)
			{
				happened |= 16;
				MyTweenTime myTweenTime = MyTweenTime.Begin(base.gameObject, 0.1f, 0f);
				myTweenTime.eventReceiver = base.gameObject;
				myTweenTime.callWhenFinished = "BeginTutorial";
			}
			break;
		}
	}

	private void BeginTutorial()
	{
		SetGizmoPosition();
		ControlGizmo.instance.MiddleFlash();
		ControlGizmo.instance.RightFlash();
		GestureRecognizer.enabled = true;
	}
}
