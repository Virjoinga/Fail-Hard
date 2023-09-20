using UnityEngine;

public class AggressiveCenterOnChild : MonoBehaviour
{
	public float springStrength = 8f;

	public float FlipThreshold;

	public SpringPanel.OnFinished onFinished;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;

	public GameObject centeredObject
	{
		get
		{
			return mCenteredObject;
		}
	}

	private void OnEnable()
	{
		if (!(mDrag == null))
		{
			return;
		}
		mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
		if (mDrag == null)
		{
			Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
			base.enabled = false;
			return;
		}
		mDrag.onDragFinished = OnDragFinished;
		if (mDrag.horizontalScrollBar != null)
		{
			mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;
		}
		if (mDrag.verticalScrollBar != null)
		{
			mDrag.verticalScrollBar.onDragFinished = OnDragFinished;
		}
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Vector2 totalDelta = UICamera.currentTouch.totalDelta;
			float num = Mathf.Abs(totalDelta.x) / (float)Screen.width;
			if (num > FlipThreshold)
			{
				Recenter(totalDelta.x);
			}
			else
			{
				Recenter();
			}
		}
	}

	public void Recenter(float xDirection)
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (mDrag == null)
			{
				Debug.LogWarning(string.Concat(GetType(), " requires ", typeof(UIDraggablePanel), " on a parent object in order to work"), this);
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
			if (mDrag.horizontalScrollBar != null)
			{
				mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;
			}
			if (mDrag.verticalScrollBar != null)
			{
				mDrag.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		localPosition = cachedTransform.parent.TransformPoint(localPosition);
		Vector3 vector = localPosition - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		Transform transform = null;
		Transform transform2 = null;
		Transform transform3 = base.transform;
		int i = 0;
		for (int childCount = transform3.childCount; i < childCount; i++)
		{
			Transform child = transform3.GetChild(i);
			float x = (child.position - vector).x;
			bool flag = false;
			flag = (x < 0f && xDirection > 0f) || (x > 0f && xDirection < 0f);
			float num3 = Mathf.Abs(x);
			if (flag && num3 < num)
			{
				num = num3;
				transform = child;
			}
			else if (num3 < num2)
			{
				num2 = num3;
				transform2 = child;
			}
		}
		if (transform2 != null && transform == null)
		{
			transform = transform2;
		}
		if (transform != null)
		{
			mCenteredObject = transform.gameObject;
			Vector3 vector2 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector3 = cachedTransform.InverseTransformPoint(localPosition);
			Vector3 vector4 = vector2 - vector3;
			if (mDrag.scale.x == 0f)
			{
				vector4.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector4.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector4.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector4, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}

	public Transform FindClosest()
	{
		Vector3 vector = FindWorldCenter();
		Vector3 vector2 = vector - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform result = null;
		Transform transform = base.transform;
		int i = 0;
		for (int childCount = transform.childCount; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - vector2);
			if (num2 < num)
			{
				num = num2;
				result = child;
			}
		}
		return result;
	}

	public void Recenter()
	{
		if (mDrag.panel == null)
		{
			return;
		}
		Transform transform = FindClosest();
		if (transform != null)
		{
			mCenteredObject = transform.gameObject;
			Transform cachedTransform = mDrag.panel.cachedTransform;
			Vector3 position = FindWorldCenter();
			Vector3 vector = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(position);
			Vector3 vector3 = vector - vector2;
			if (mDrag.scale.x == 0f)
			{
				vector3.x = 0f;
			}
			if (mDrag.scale.y == 0f)
			{
				vector3.y = 0f;
			}
			if (mDrag.scale.z == 0f)
			{
				vector3.z = 0f;
			}
			SpringPanel.Begin(mDrag.gameObject, cachedTransform.localPosition - vector3, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}

	private Vector3 FindWorldCenter()
	{
		if (!mDrag)
		{
			OnEnable();
		}
		Vector4 clipRange = mDrag.panel.clipRange;
		Transform cachedTransform = mDrag.panel.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x += clipRange.x;
		localPosition.y += clipRange.y;
		return cachedTransform.parent.TransformPoint(localPosition);
	}
}
