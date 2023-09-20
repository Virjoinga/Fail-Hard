using System;
using System.Collections.Generic;
using Game;
using Game.Progress;
using UnityEngine;

public class LevelDetailsListView : MonoBehaviour
{
	public GameObject loadingScreen;

	public GameObject itemDelegatePrefab;

	private List<GameObject> delegates;

	public UIGrid grid;

	public UIDraggablePanel draggablePanel;

	private SpringPanel centeringSpring;

	private ILevel[] model;

	public UITexture background;

	private int currentItemIndex;

	private void Start()
	{
		UIDraggablePanel uIDraggablePanel = draggablePanel;
		uIDraggablePanel.onDragFinished = (UIDraggablePanel.OnDragFinished)Delegate.Combine(uIDraggablePanel.onDragFinished, new UIDraggablePanel.OnDragFinished(OnDragFinished));
		centeringSpring = draggablePanel.gameObject.AddComponent<SpringPanel>();
		centeringSpring.strength = 20f;
		centeringSpring.enabled = false;
	}

	public void InitModel()
	{
		GameController instance = GameController.Instance;
		model = instance.LevelDatabase.Levels(instance.MyCurrentTheme);
		for (int i = 0; i < model.GetLength(0); i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(itemDelegatePrefab, base.transform.position, base.transform.rotation) as GameObject;
			gameObject.transform.parent = grid.transform;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<LevelParametersDialog>().SetLevelData(model[i]);
			delegates.Add(gameObject);
			gameObject.GetComponent<LevelParametersDialog>().ItemSelected += OnLevelSelected;
			if (model[i] == instance.CurrentLevel)
			{
				currentItemIndex = i;
			}
		}
		grid.Reposition();
		SetCurrentIndex(currentItemIndex, false);
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		if (delegates == null)
		{
			delegates = new List<GameObject>();
		}
		InitModel();
	}

	public void OnDisable()
	{
		if (delegates == null)
		{
			return;
		}
		foreach (GameObject @delegate in delegates)
		{
			UnityEngine.Object.Destroy(@delegate);
		}
		delegates.Clear();
	}

	public void OnLevelSelected(LevelParametersDialog data)
	{
		GameController.Instance.SetLevelToLoad(model[currentItemIndex]);
		NGUITools.AddChild(base.transform.parent.parent.gameObject, loadingScreen);
	}

	public void OnDragFinished()
	{
		int num = -Mathf.RoundToInt(draggablePanel.transform.localPosition.x / grid.cellWidth);
		if (num < 0)
		{
			num = 0;
		}
		SetCurrentIndex(num);
	}

	public void SetCurrentIndex(int index, bool smooth = true)
	{
		if (index < 0)
		{
			index = 0;
		}
		if (index >= model.GetLength(0) - 1)
		{
			index = model.GetLength(0) - 1;
		}
		currentItemIndex = index;
		if (centeringSpring != null)
		{
			centeringSpring.enabled = true;
			centeringSpring.target = (0f - grid.cellWidth) * (float)index * Vector3.right;
			if (!smooth)
			{
				draggablePanel.transform.localPosition = centeringSpring.target;
			}
		}
		else
		{
			draggablePanel.transform.localPosition = (0f - grid.cellWidth) * (float)index * Vector3.right;
		}
	}
}
