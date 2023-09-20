using Holoville.HOTween;
using UnityEngine;

public class GridLocatorItem : MonoBehaviour
{
	public UISprite ItemSprite;

	public Color ActiveColor;

	public Color InactiveColor;

	private bool m_active;

	private void Start()
	{
		if (!m_active)
		{
			ItemSprite.color = InactiveColor;
		}
		base.transform.localScale = new Vector2(20f, 20f);
	}

	public void SetState(bool active)
	{
		if (m_active != active)
		{
			m_active = active;
			if (m_active)
			{
				TweenColor(ActiveColor);
			}
			else
			{
				TweenColor(InactiveColor);
			}
		}
	}

	private void TweenColor(Color targetColor)
	{
		TweenParms p_parms = new TweenParms().Prop("color", targetColor).Ease(EaseType.EaseInCirc);
		HOTween.To(ItemSprite, 0.15f, p_parms);
	}
}
