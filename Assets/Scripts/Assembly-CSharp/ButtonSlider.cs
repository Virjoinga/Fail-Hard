using System;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class ButtonSlider : MonoBehaviour
{
	[Serializable]
	public enum Direction
	{
		Down = 0,
		Right = 1,
		Left = 2,
		Up = 3
	}

	public float ButtonOffset;

	public Direction OpenDirection;

	public List<Transform> Buttons;

	public float OpenDuration;

	public UISprite ButtonBackground;

	public string OpenedSprite;

	public string ClosedSprite;

	public bool Scale = true;

	private bool m_opened;

	private void OnDisable()
	{
		Reset();
	}

	private void OnClick()
	{
		if (m_opened)
		{
			CloseFan();
		}
		else
		{
			OpenFan();
		}
		m_opened = !m_opened;
	}

	public void RemoveObject(GameObject go)
	{
		if (Buttons.Contains(go.transform))
		{
			Buttons.Remove(go.transform);
		}
	}

	private void OpenFan()
	{
		ButtonBackground.spriteName = OpenedSprite;
		for (int i = 0; i < Buttons.Count; i++)
		{
			Transform tf = Buttons[i];
			float num = (float)i * ButtonOffset + ButtonOffset;
			Vector3 vector = TranslateVector(OpenDirection);
			TweenParms tweenParms = new TweenParms().Prop("localPosition", vector * num, true);
			if (Scale)
			{
				tf.localScale = Vector3.zero;
				tweenParms.Prop("localScale", Vector3.one);
			}
			tweenParms.OnStart(delegate
			{
				tf.gameObject.SetActive(true);
			});
			HOTween.To(tf, OpenDuration, tweenParms);
		}
	}

	private void CloseFan()
	{
		ButtonBackground.spriteName = ClosedSprite;
		for (int i = 0; i < Buttons.Count; i++)
		{
			Transform tf = Buttons[i];
			float num = 0f - ((float)i * ButtonOffset + ButtonOffset);
			Vector3 vector = TranslateVector(OpenDirection);
			TweenParms tweenParms = new TweenParms().Prop("localPosition", vector * num, true);
			if (Scale)
			{
				tweenParms.Prop("localScale", Vector3.zero);
			}
			TweenAlphaEnableDisable component = tf.GetComponent<TweenAlphaEnableDisable>();
			if (component != null)
			{
				component.Disable();
			}
			else
			{
				tweenParms.OnComplete((TweenDelegate.TweenCallback)delegate
				{
					tf.gameObject.SetActive(false);
				});
			}
			HOTween.To(tf, OpenDuration, tweenParms);
		}
	}

	private void Reset()
	{
		m_opened = false;
		ButtonBackground.spriteName = ClosedSprite;
		for (int i = 0; i < Buttons.Count; i++)
		{
			Transform transform = Buttons[i];
			transform.localPosition = Vector3.zero;
			transform.gameObject.SetActive(false);
		}
	}

	private Vector3 TranslateVector(Direction dir)
	{
		switch (dir)
		{
		case Direction.Up:
			return Vector3.up;
		case Direction.Left:
			return Vector3.left;
		case Direction.Right:
			return Vector3.right;
		case Direction.Down:
			return Vector3.down;
		default:
			return Vector3.zero;
		}
	}
}
