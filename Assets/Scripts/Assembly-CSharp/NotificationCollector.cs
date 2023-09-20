using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class NotificationCollector : MonoBehaviour
{
	public enum Side
	{
		Left = 0,
		Right = 1
	}

	private class Handler
	{
		public delegate void NewNotification(Handler handler);

		public Notification.Types Type;

		public Action<Side, object> OnNotify;

		public Side ShowOnSide;

		private object m_data;

		[method: MethodImpl(32)]
		public event NewNotification OnNotification = delegate
		{
		};

		public void Subscribe()
		{
			NotificationCentre.Subscribe(Type, GotEvent);
		}

		public void Unsubscribe()
		{
			NotificationCentre.Unsubscribe(Type, GotEvent);
		}

		public void Invoke()
		{
			OnNotify(ShowOnSide, m_data);
		}

		private void GotEvent(object data)
		{
			m_data = data;
			this.OnNotification(this);
		}
	}

	public GameObject NotificationPrefabRight;

	public GameObject NotificationPrefabLeft;

	public GameObject NotificationPrefabRightBlocking;

	public GameObject NotificationPrefabLeftBlocking;

	public List<UIPanel> PanelsToShowIn;

	private Handler[] m_listenNotifications;

	private Queue<Handler> m_leftQueue;

	private Queue<Handler> m_rightQueue;

	private Handler m_leftShowing;

	private Handler m_rightShowing;

	private void Awake()
	{
		m_leftQueue = new Queue<Handler>();
		m_rightQueue = new Queue<Handler>();
		m_listenNotifications = new Handler[22]
		{
			new Handler
			{
				ShowOnSide = Side.Right,
				OnNotify = MechanicFinished
			},
			new Handler
			{
				Type = Notification.Types.MechanicFixingVehicle,
				ShowOnSide = Side.Right,
				OnNotify = MechanicFixingVehicle
			},
			new Handler
			{
				Type = Notification.Types.MechanicVehicleFixed,
				ShowOnSide = Side.Right,
				OnNotify = MechanicVehicleFixed
			},
			new Handler
			{
				Type = Notification.Types.MechanicNotEnoughMoney,
				ShowOnSide = Side.Right,
				OnNotify = MechanicNotEnoughMoney
			},
			new Handler
			{
				Type = Notification.Types.MechanicBusy,
				ShowOnSide = Side.Right,
				OnNotify = MechanicBusy
			},
			new Handler
			{
				Type = Notification.Types.MechanicItemLocked,
				ShowOnSide = Side.Right,
				OnNotify = MechanicItemLocked
			},
			new Handler
			{
				Type = Notification.Types.MechanicItemUnlocked,
				ShowOnSide = Side.Right,
				OnNotify = MechanicItemUnlocked
			},
			new Handler
			{
				Type = Notification.Types.TutorialThrottle,
				OnNotify = TutorialThrottle
			},
			new Handler
			{
				Type = Notification.Types.TutorialJump,
				OnNotify = TutorialJump
			},
			new Handler
			{
				Type = Notification.Types.TutorialJumpOff,
				OnNotify = TutorialJumpOff
			},
			new Handler
			{
				Type = Notification.Types.TutorialSpinForward,
				OnNotify = TutorialSpinForward
			},
			new Handler
			{
				Type = Notification.Types.TutorialUpgradeAcceleration,
				ShowOnSide = Side.Right,
				OnNotify = TutorialUpgradeAcceleration
			},
			new Handler
			{
				Type = Notification.Types.TutorialUpgradeSpin,
				ShowOnSide = Side.Right,
				OnNotify = TutorialUpgradeSpin
			},
			new Handler
			{
				Type = Notification.Types.TutorialUpgradeTopSpeed,
				ShowOnSide = Side.Right,
				OnNotify = TutorialUpgradeTopSpeed
			},
			new Handler
			{
				Type = Notification.Types.TutorialPurchaseFirstVehicle,
				OnNotify = TutorialPurchaseFirstVehicle
			},
			new Handler
			{
				Type = Notification.Types.LevelUnlocked,
				OnNotify = LevelUnlocked
			},
			new Handler
			{
				Type = Notification.Types.OldClient,
				OnNotify = OldClient
			},
			new Handler
			{
				Type = Notification.Types.StageCompleted,
				OnNotify = StageCompleted
			},
			new Handler
			{
				Type = Notification.Types.LevelUp,
				OnNotify = LevelUp
			},
			new Handler
			{
				Type = Notification.Types.GetJarCoinsGet,
				OnNotify = GetJarCoinsGet
			},
			new Handler
			{
				Type = Notification.Types.SupersonicAdWatched,
				OnNotify = SupersonicAdCoinsGet
			},
			new Handler
			{
				Type = Notification.Types.BundleCompletedAward,
				OnNotify = BundleCompletedAward
			}
		};
	}

	private void Start()
	{
		Handler[] listenNotifications = m_listenNotifications;
		foreach (Handler handler in listenNotifications)
		{
			handler.Subscribe();
			handler.OnNotification += CollectHandlers;
		}
	}

	private void OnDestroy()
	{
		Handler[] listenNotifications = m_listenNotifications;
		foreach (Handler handler in listenNotifications)
		{
			handler.Unsubscribe();
		}
	}

	private void ShowNotification(string text, string iconName, string audioName, Side side, bool isBlocking = false)
	{
		UIPanel uIPanel = PanelsToShowIn.Find((UIPanel x) => x.gameObject.activeInHierarchy);
		if (!(uIPanel == null))
		{
			Action action = null;
			GameObject gameObject;
			if (side == Side.Left)
			{
				gameObject = ((!isBlocking) ? GOTools.SpawnAsChild(NotificationPrefabLeft, uIPanel.cachedTransform) : GOTools.SpawnAsChild(NotificationPrefabLeftBlocking, uIPanel.cachedTransform));
				action = ShowNextOnLeft;
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = -2.5f;
				gameObject.transform.localPosition = localPosition;
			}
			else
			{
				gameObject = ((!isBlocking) ? GOTools.SpawnAsChild(NotificationPrefabRight, uIPanel.cachedTransform) : GOTools.SpawnAsChild(NotificationPrefabRightBlocking, uIPanel.cachedTransform));
				action = ShowNextOnRight;
				Vector3 localPosition2 = gameObject.transform.localPosition;
				localPosition2.z = -2.7f;
				gameObject.transform.localPosition = localPosition2;
			}
			NotificationTray component = gameObject.GetComponent<NotificationTray>();
			if ((bool)component)
			{
				component.SetData(text, isBlocking, iconName, action);
			}
			if (!string.IsNullOrEmpty(audioName))
			{
				AudioManager.Instance.NGPlay((AudioClip)Resources.Load(audioName));
			}
		}
	}

	private void CollectHandlers(Handler handler)
	{
		if (handler.ShowOnSide == Side.Left)
		{
			if (m_leftShowing != handler)
			{
				m_leftQueue.Enqueue(handler);
				if (m_leftShowing == null)
				{
					ShowNextOnLeft();
				}
			}
		}
		else if (m_rightShowing != handler)
		{
			m_rightQueue.Enqueue(handler);
			if (m_rightShowing == null)
			{
				ShowNextOnRight();
			}
		}
	}

	private void ShowNextOnRight()
	{
		m_rightShowing = null;
		if (m_rightQueue.Count != 0)
		{
			Handler handler = m_rightQueue.Dequeue();
			if (handler != null)
			{
				m_rightShowing = handler;
				handler.Invoke();
			}
		}
	}

	private void ShowNextOnLeft()
	{
		m_leftShowing = null;
		if (m_leftQueue.Count != 0)
		{
			Handler handler = m_leftQueue.Dequeue();
			if (handler != null)
			{
				m_leftShowing = handler;
				handler.Invoke();
			}
		}
	}

	private void MechanicFinished(Side side, object data = null)
	{
		ShowNotification(Language.Get("UPGRADE_READY"), "character_mechanic", "Audio/upgrade_ready", side);
	}

	private void MechanicFixingVehicle(Side side, object data = null)
	{
		ShowNotification(Language.Get("LETS_FIX_YOUR_MOPED"), "character_mechanic", "Audio/upgrade_ready", side);
	}

	private void MechanicVehicleFixed(Side side, object data = null)
	{
		ShowNotification(Language.Get("YOUR_MOPED_IS_FIXED"), "character_mechanic", "Audio/upgrade_ready", side);
	}

	private void MechanicBusy(Side side, object data = null)
	{
		ShowNotification(Language.Get("SORRY_IM_BUSY"), "character_mechanic", "Audio/aj_denied", side);
	}

	private void MechanicNotEnoughMoney(Side side, object data = null)
	{
		ShowNotification(Language.Get("NOT_ENOUGH_MONEY"), "character_mechanic", "Audio/aj_denied", side);
	}

	private void MechanicItemLocked(Side side, object data = null)
	{
		ShowNotification(Language.Get("SORRY_ITS_LOCKED"), "character_mechanic", "Audio/aj_denied", side);
	}

	private void MechanicItemUnlocked(Side side, object data = null)
	{
		ShowNotification(Language.Get("COOL_NEW_STUFF"), "character_mechanic", "Audio/upgrade_ready", side);
	}

	private void TutorialThrottle(Side side, object data = null)
	{
		ShowNotification("Throttle takes you to places.", "character_agent", null, side);
	}

	private void TutorialJump(Side side, object data = null)
	{
		ShowNotification("Jump on the ramp.", "character_agent", null, side);
	}

	private void TutorialJumpOff(Side side, object data = null)
	{
		ShowNotification("Jump again while in air to jump off.", "character_agent", null, side);
	}

	private void TutorialSpinForward(Side side, object data = null)
	{
		ShowNotification("Spin in air to aim your jump off.", "character_agent", null, side);
	}

	private void TutorialUpgradeAcceleration(Side side, object data = null)
	{
		ShowNotification("Better acceleration helps in this gig.", "character_mechanic", null, side, true);
	}

	private void TutorialUpgradeSpin(Side side, object data = null)
	{
		ShowNotification("Better spin helps in this gig.", "character_mechanic", null, side, true);
	}

	private void TutorialUpgradeTopSpeed(Side side, object data = null)
	{
		ShowNotification("Better top speed helps in this gig.", "character_mechanic", null, side, true);
	}

	private void TutorialPurchaseFirstVehicle(Side side, object data = null)
	{
		ShowNotification("Buy yourself a moped to get things rolling.", "character_agent", null, side);
	}

	private void LevelUnlocked(Side side, object data = null)
	{
		ShowNotification(Language.Get("NEW_LEVEL_UNLOCKED"), "character_agent", "Audio/fhg_radio_success", side);
	}

	private void OldClient(Side side, object data = null)
	{
		ShowNotification("New version available!", "character_agent", "Audio/fhg_radio_success", side);
	}

	private void StageCompleted(Side side, object data = null)
	{
		ShowNotification(Language.Get("STAGE_COMPLETED"), "character_agent", "Audio/fhg_radio_success", side);
	}

	private void BundleCompletedAward(Side side, object coinsAmount)
	{
		int num = 0;
		if (coinsAmount is int)
		{
			num = (int)coinsAmount;
		}
		if (num > 0)
		{
			string text = string.Format("Bundle completed! Reward: {0}@@", num);
			ShowNotification(text, "character_agent", null, side);
		}
	}

	private void LevelUp(Side side, object data = null)
	{
		if (data != null)
		{
			Notification.LevelUpNotification levelUpNotification = (Notification.LevelUpNotification)data;
			ShowNotification("Level up bonus " + levelUpNotification.Reward + "@@" + Environment.NewLine + "Next level in " + levelUpNotification.Stars + "**", "character_agent", "Audio/fhg_radio_success", side);
		}
	}

	private void GetJarCoinsGet(Side side, object coinsAmount)
	{
		int num = 0;
		if (coinsAmount is int)
		{
			num = (int)coinsAmount;
		}
		if (num > 0)
		{
			string text = string.Format("Congrats! You got {0}@@ via GetJar!", num);
			ShowNotification(text, "character_agent", null, side);
		}
	}

	private void SupersonicAdCoinsGet(Side side, object coinsAmount)
	{
		int num = 0;
		if (coinsAmount is int)
		{
			num = (int)coinsAmount;
		}
		if (num > 0)
		{
			string text = string.Format("Yeah! You were awarded {0}@@", num);
			ShowNotification(text, "character_agent", null, side);
		}
	}
}
