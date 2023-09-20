using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGizmo : MonoBehaviour
{
	private class MyTween
	{
		public GameObject target;

		public float duration;

		public float alpha;

		public MyTween(GameObject t, float d, float a)
		{
			target = t;
			duration = d;
			alpha = a;
		}
	}

	private class TweenSequencer
	{
		private ArrayList queue;

		private bool running;

		private int itemsRunning;

		private GameObject gameObject;

		public TweenSequencer(GameObject p)
		{
			queue = new ArrayList();
			running = false;
			itemsRunning = 0;
			gameObject = p;
		}

		public void Add(MyTween tw)
		{
			queue.Add(tw);
			if (!running)
			{
				Completed(null);
			}
		}

		public void Add(List<MyTween> tw)
		{
			queue.Add(tw);
			if (!running)
			{
				Completed(null);
			}
		}

		public void Completed(UITweener tweener)
		{
			running = true;
			if (queue.Count > 0)
			{
				if (!(queue[0] is MyTween))
				{
					List<MyTween> list = (List<MyTween>)queue[0];
					queue.RemoveAt(0);
					{
						foreach (MyTween item in list)
						{
							itemsRunning++;
							UITweener uITweener = TweenAlpha.Begin(item.target, item.duration, item.alpha);
							uITweener.eventReceiver = gameObject;
							uITweener.callWhenFinished = "TweenItemCompleted";
						}
						return;
					}
				}
				MyTween myTween = (MyTween)queue[0];
				queue.RemoveAt(0);
				UITweener uITweener2 = TweenAlpha.Begin(myTween.target, myTween.duration, myTween.alpha);
				uITweener2.eventReceiver = gameObject;
				uITweener2.callWhenFinished = "TweenCompleted";
			}
			else
			{
				running = false;
			}
		}

		public void ItemCompleted(UITweener tweener)
		{
			if (--itemsRunning == 0)
			{
				Completed(tweener);
			}
		}
	}

	private const float fadeDuration = 0.2f;

	private const float flashDuration = 0.9f;

	public static ControlGizmo instance;

	public GameObject DiagonalLeft;

	public GameObject DiagonalRight;

	public GameObject Left;

	public GameObject Right;

	public GameObject Up;

	public GameObject Middle;

	private TweenSequencer tweenQueue;

	private void Start()
	{
		tweenQueue = new TweenSequencer(base.gameObject);
		instance = this;
	}

	public void Reset()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item2 in base.transform)
		{
			foreach (Transform item3 in item2)
			{
				list.Add(item3);
			}
		}
		FadeOutItems(list, false);
	}

	public void StartAction(GizmoAction a)
	{
		base.gameObject.SendMessage(a.ToString());
	}

	public void Begin()
	{
		Reset();
		MiddleFlash();
	}

	public void ShowTap()
	{
		Middle.SetActive(true);
		Right.SetActive(false);
	}

	public void ShowRight()
	{
		Middle.SetActive(false);
		Right.SetActive(true);
	}

	public void HideAll()
	{
		Middle.SetActive(false);
		Right.SetActive(false);
	}

	public void MiddleFlash()
	{
		FadeInTree(Middle, "Normal");
		QueueFlashes(FindInTree(Middle, "Hint"), 6);
		FadeOut(Middle, "Hint");
	}

	public void MiddleOk()
	{
		FadeInTree(Middle, "Ok");
		FadeInTree(Middle, "Normal");
	}

	private void MiddleNormal()
	{
		FadeInTree(Middle, "Normal", true);
	}

	public void UpOk()
	{
		FadeInTree(Up, "Ok");
		Reset();
	}

	private void UpFadeOut()
	{
		FadeOutTree(Up, true);
	}

	public void UpFlash()
	{
		FadeInTree(Middle, "Normal");
		FadeIn(Up, "Normal");
		QueueFlashes(FindInTree(Up, "Hint"), 6);
		FadeOut(Up, "Hint");
	}

	public void RightOk()
	{
		FadeInTree(Middle, "Normal");
		FadeInTree(Right, "Ok");
		Reset();
	}

	private void RightFadeOut()
	{
		FadeOutTree(Right, true);
	}

	public void RightFlash()
	{
		FadeInTree(Middle, "Normal");
		FadeIn(Right, "Normal");
		QueueFlashes(FindInTree(Right, "Hint"), 6);
		FadeOut(Right, "Hint");
	}

	private void QueueFlashes(GameObject go, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			float a = ((i % 2 != 0) ? 1f : 0.3f);
			tweenQueue.Add(new MyTween(go, 0.05f, a));
		}
	}

	private void FadeInTree(GameObject go, string tag)
	{
		FadeInTree(go, tag, true);
	}

	private void FadeInTree(GameObject go, string tag, bool queue)
	{
		FadeInTree(new List<GameObject>(new GameObject[1] { go }), tag, queue);
	}

	private void FadeInTree(List<GameObject> plist, string tag)
	{
		FadeInTree(plist, tag, true);
	}

	private void FadeInTree(List<GameObject> plist, string tag, bool queue)
	{
		List<MyTween> list = new List<MyTween>();
		foreach (GameObject item in plist)
		{
			foreach (Transform item2 in item.transform)
			{
				if (item2.CompareTag(tag))
				{
					if (queue)
					{
						list.Add(new MyTween(item2.gameObject, 0.2f, 1f));
					}
					else
					{
						TweenAlpha.Begin(item2.gameObject, 0.2f, 1f);
					}
				}
				else if (queue)
				{
					list.Add(new MyTween(item2.gameObject, 0.2f, 0f));
				}
				else
				{
					TweenAlpha.Begin(item2.gameObject, 0.2f, 0f);
				}
			}
		}
		if (queue)
		{
			tweenQueue.Add(list);
		}
	}

	private void FadeOutTree(GameObject go)
	{
		FadeOutTree(go, true);
	}

	private void FadeOutTree(GameObject go, bool queue)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in go.transform)
		{
			list.Add(item);
		}
		FadeOutItems(list, queue);
	}

	private void FadeOutItems(List<Transform> childs)
	{
		FadeOutItems(childs, true);
	}

	private void FadeOutItems(List<Transform> childs, bool queue)
	{
		List<MyTween> list = null;
		if (queue)
		{
			list = new List<MyTween>();
		}
		foreach (Transform child in childs)
		{
			if (queue)
			{
				list.Add(new MyTween(child.gameObject, 0.2f, 0f));
			}
			else
			{
				TweenAlpha.Begin(child.gameObject, 0.2f, 0f);
			}
		}
		if (queue)
		{
			tweenQueue.Add(list);
		}
	}

	private void FadeOut(GameObject go, string tag, bool queue)
	{
		foreach (Transform item in go.transform)
		{
			if (item.CompareTag(tag))
			{
				if (queue)
				{
					tweenQueue.Add(new MyTween(item.gameObject, 0.2f, 0f));
				}
				else
				{
					TweenAlpha.Begin(item.gameObject, 0.2f, 0f);
				}
			}
		}
	}

	private void FadeOut(GameObject go, string tag)
	{
		foreach (Transform item in go.transform)
		{
			if (item.CompareTag(tag))
			{
				tweenQueue.Add(new MyTween(item.gameObject, 0.2f, 0f));
			}
		}
	}

	private void FadeIn(GameObject go, string tag)
	{
		foreach (Transform item in go.transform)
		{
			if (item.CompareTag(tag))
			{
				tweenQueue.Add(new MyTween(item.gameObject, 0.2f, 1f));
			}
		}
	}

	private GameObject FindInTree(GameObject go, string tag)
	{
		foreach (Transform item in go.transform)
		{
			if (item.CompareTag(tag))
			{
				return item.gameObject;
			}
		}
		return null;
	}

	private void RunTests()
	{
		MiddleFlash();
		MiddleOk();
		RightFlash();
		RightOk();
		UpFlash();
		UpOk();
	}

	public void TweenCompleted(UITweener tweener)
	{
		tweenQueue.Completed(tweener);
	}

	public void TweenItemCompleted(UITweener tweener)
	{
		tweenQueue.ItemCompleted(tweener);
	}
}
