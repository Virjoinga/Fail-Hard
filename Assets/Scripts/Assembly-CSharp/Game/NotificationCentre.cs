using System;
using System.Collections.Generic;
using Game.Util;
using UnityEngine;

namespace Game
{
	[ExecuteInEditMode]
	internal class NotificationCentre : MonoBehaviour
	{
		private class NotificationData
		{
			public Notification.Types Type;

			public object Data;

			public NotificationData(Notification.Types type, object data)
			{
				Data = data;
				Type = type;
			}
		}

		private class NotificationRouter
		{
			private List<NotificationHandler> m_handlers;

			public NotificationRouter()
			{
				m_handlers = new List<NotificationHandler>();
			}

			public NotificationRouter(NotificationHandler firstHandler)
				: this()
			{
				m_handlers.Add(firstHandler);
			}

			public void AddHandler(NotificationHandler handler)
			{
				m_handlers.Add(handler);
			}

			public void SendNotification(object data)
			{
				foreach (NotificationHandler handler in m_handlers)
				{
					handler(data);
				}
			}

			public void RemoveHandler(NotificationHandler handler)
			{
				m_handlers.Remove(handler);
			}
		}

		public delegate void NotificationHandler(object data);

		private static NotificationCentre s_instance;

		private static bool s_created;

		private Dictionary<Notification.Types, NotificationRouter> m_subscriptions = new Dictionary<Notification.Types, NotificationRouter>();

		private Queue<NotificationData> m_notificationQueue = new Queue<NotificationData>();

		private static NotificationCentre Instance
		{
			get
			{
				if (!s_created && s_instance == null)
				{
					s_created = true;
					s_instance = new GameObject("NotificationCentre").AddComponent<NotificationCentre>();
					UnityEngine.Object.DontDestroyOnLoad(s_instance);
				}
				return s_instance;
			}
		}

		private void Update()
		{
			if (Application.isEditor && !Application.isPlaying)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (m_notificationQueue.Count <= 0)
			{
				return;
			}
			lock (m_notificationQueue)
			{
				while (m_notificationQueue.Count > 0)
				{
					NotificationData notificationData = m_notificationQueue.Dequeue();
					SendUnitySafeNotification(notificationData.Type, notificationData.Data);
				}
			}
		}

		private void OnApplicationQuit()
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}

		public static void Subscribe(Notification.Types type, NotificationHandler handler)
		{
			Instance.MakeSubscription(type, handler);
		}

		public static void Unsubscribe(Notification.Types type, NotificationHandler handler)
		{
			Instance.CancelSubscription(type, handler);
		}

		public static void Send(Notification.Types type, object data)
		{
			lock (Instance.m_notificationQueue)
			{
				Instance.m_notificationQueue.Enqueue(new NotificationData(type, data));
			}
		}

		private void MakeSubscription(Notification.Types type, NotificationHandler handler)
		{
			if (m_subscriptions.ContainsKey(type))
			{
				m_subscriptions[type].AddHandler(handler);
			}
			else
			{
				m_subscriptions.Add(type, new NotificationRouter(handler));
			}
		}

		private void CancelSubscription(Notification.Types type, NotificationHandler handler)
		{
			if (m_subscriptions.ContainsKey(type))
			{
				m_subscriptions[type].RemoveHandler(handler);
			}
		}

		private void SendUnitySafeNotification(Notification.Types type, object data)
		{
			if (m_subscriptions.ContainsKey(type))
			{
				m_subscriptions[type].SendNotification(data);
			}
			else
			{
				Logger.Log("(No one listening) Notification type not found: " + Enum.GetName(typeof(Notification.Types), type));
			}
		}
	}
}
