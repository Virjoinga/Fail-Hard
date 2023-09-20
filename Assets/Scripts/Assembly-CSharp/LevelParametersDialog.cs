using System.Runtime.CompilerServices;
using Game;
using Game.Progress;
using UnityEngine;

public class LevelParametersDialog : MonoBehaviour
{
	public delegate void OnItemSelected(LevelParametersDialog listItem);

	private ILevel m_level;

	private Texture2D m_texture;

	public UITexture levelIcon;

	public UILabel dialogTitle;

	public LevelTargetDisplay targetGrid;

	public GameObject loadingScreen;

	[method: MethodImpl(32)]
	public event OnItemSelected ItemSelected;

	public void Start()
	{
		levelIcon.mainTexture = m_texture;
	}

	public void SetLevelData(ILevel level, Texture2D levelTexture, bool isLocked)
	{
		m_level = level;
		m_texture = levelTexture;
		dialogTitle.text = m_level.Parameters.Name;
		targetGrid.SetLevel(level as Level, 0, 55);
	}

	public void SetLevelData(ILevel level)
	{
		m_level = level;
		dialogTitle.text = m_level.Parameters.Name;
		targetGrid.SetLevel(level as Level, 0, 55);
		string path = "Levels/" + m_level.Parameters.Name + "_snapshot";
		m_texture = (Texture2D)Resources.Load(path);
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (GameController.Instance.CurrentLevel != null)
		{
			SetLevelData(GameController.Instance.CurrentLevel);
		}
	}

	public void OnCancel()
	{
		Object.Destroy(base.gameObject);
	}

	public void OnClick()
	{
		if (this.ItemSelected != null)
		{
			this.ItemSelected(this);
		}
	}

	public void PlayLevel()
	{
		GameController.Instance.SetLevelToLoad(m_level);
		NGUITools.AddChild(base.transform.parent.parent.gameObject, loadingScreen);
	}
}
