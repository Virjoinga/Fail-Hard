using Game;
using UnityEngine;

public class HighlightOnNotification : MonoBehaviour
{
	public GameObject ButtonToHighlight;

	public Color ButtonHighlightColor;

	public float ButtonHighlightScale = 1f;

	public UISprite Glow;

	public Color GlowHighlightColor;

	public float GlowHighlightScale = 1.05f;

	public int HighlightLoops = 10;

	public Notification.Types m_notificationType;

	private HighlightButton m_highlighter;

	private HighlightButton m_highlighterExtra;

	private void OnEnable()
	{
		if (ButtonToHighlight != null)
		{
			m_highlighter = base.gameObject.AddComponent<HighlightButton>();
			m_highlighter.TargetColor = ButtonHighlightColor;
			m_highlighter.TargetScale = ButtonHighlightScale;
		}
		if (Glow != null)
		{
			m_highlighterExtra = base.gameObject.AddComponent<HighlightButton>();
			m_highlighterExtra.TargetColor = GlowHighlightColor;
			m_highlighterExtra.TargetScale = GlowHighlightScale;
		}
		NotificationCentre.Subscribe(m_notificationType, ReactToNotification);
		NotificationCentre.Subscribe(Notification.Types.HideTutorials, DisableHighlight);
	}

	public void ReconfigureSubscription(Notification.Types notificationType)
	{
		NotificationCentre.Unsubscribe(m_notificationType, ReactToNotification);
		m_notificationType = notificationType;
		NotificationCentre.Subscribe(m_notificationType, ReactToNotification);
	}

	private void OnDestroy()
	{
		NotificationCentre.Unsubscribe(m_notificationType, ReactToNotification);
	}

	private void EnableHighlight()
	{
		if (m_highlighter != null)
		{
			m_highlighter.ButtonObject = ButtonToHighlight;
			m_highlighter.LoopCount = HighlightLoops;
			m_highlighter.ShowHighlight();
		}
		if (m_highlighterExtra != null)
		{
			m_highlighterExtra.ButtonObject = Glow.gameObject;
			m_highlighterExtra.LoopCount = HighlightLoops;
			m_highlighterExtra.ShowHighlight();
		}
	}

	private void DisableHighlight(object data)
	{
		if (m_highlighter != null)
		{
			m_highlighter.StopHighlight();
		}
		if (m_highlighterExtra != null)
		{
			m_highlighterExtra.StopHighlight();
		}
	}

	private void ReactToNotification(object data)
	{
		EnableHighlight();
	}

	public void OnClick()
	{
		DisableHighlight(null);
	}
}
