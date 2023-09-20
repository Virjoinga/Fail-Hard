using Game.Util;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
public class UISpriteTuple : MonoBehaviour
{
	public enum SpriteState
	{
		StateOn = 0,
		StateOff = 1
	}

	public string[] sprites;

	private UISprite m_sprite;

	private SpriteState m_currentState;

	private void Awake()
	{
		m_sprite = GetComponent<UISprite>();
		if (m_sprite == null)
		{
			Logger.Error("No UISprite component found: " + base.gameObject.name);
		}
		else
		{
			m_sprite.spriteName = sprites[0];
		}
	}

	public void SetOn()
	{
		if ((bool)m_sprite)
		{
			m_sprite.spriteName = sprites[0];
		}
	}

	public void SetOff()
	{
		if ((bool)m_sprite)
		{
			m_sprite.spriteName = sprites[1];
		}
	}

	public SpriteState State()
	{
		return m_currentState;
	}
}
