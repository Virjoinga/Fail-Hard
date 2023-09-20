using System.Collections.Generic;
using Game;
using UnityEngine;

public class LevelPageList : MonoBehaviour
{
	public GameObject LevelGridPageDelegatePrefab;

	public Transform LoadingScreenParent;

	private List<GameObject> m_delegates;

	public UIPanel ClippingPanel;

	public SpringPanel SpringPanel;

	private bool m_started;

	private void Start()
	{
		m_started = true;
		InitModel();
	}

	private void InitModel()
	{
		GameController instance = GameController.Instance;
		if (m_delegates == null)
		{
			m_delegates = new List<GameObject>();
		}
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1.3333334f;
		Vector4 clipRange = ClippingPanel.clipRange;
		clipRange.z = 1024f * num / num2;
		ClippingPanel.clipRange = clipRange;
		float num3 = 1024f * num / num2 - 840f;
		UIGrid component = GetComponent<UIGrid>();
		component.cellWidth = 1024f * num / num2;
		component.cellWidth -= 0.7f * num3;
		foreach (BundleStage stage in instance.CurrentBundle.Stages)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, LevelGridPageDelegatePrefab);
			bool isLocked = false;
			int unlockCriteria = 0;
			if (instance.CurrentBundle.CurrentStage < stage.Index)
			{
				isLocked = true;
				unlockCriteria = instance.CurrentBundle.StageCriteria(stage.Index - 1);
			}
			gameObject.GetComponentInChildren<LevelGridPage>().SetData(instance.CurrentBundle, stage, isLocked, LoadingScreenParent, unlockCriteria);
			gameObject.name = "Stage" + 10 + stage.Index;
			if (stage.Levels.Count > 12)
			{
			}
			BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
			Vector3 size = component2.size;
			size.x = component.cellWidth;
			component2.size = size;
			m_delegates.Add(gameObject);
		}
		component.Reposition();
		ClippingPanel.GetComponent<UIDraggablePanel>().ResetPosition();
		ClippingPanel.GetComponent<UIDraggablePanel>().MoveRelative(-ClippingPanel.GetComponent<UIDraggablePanel>().transform.localPosition);
		if (instance.CurrentBundle.NowPlayingStage != null)
		{
			ClippingPanel.GetComponent<UIDraggablePanel>().MoveRelative((float)(-instance.CurrentBundle.NowPlayingStage.Index) * GetComponent<UIGrid>().cellWidth * Vector3.right);
		}
		else
		{
			ClippingPanel.GetComponent<UIDraggablePanel>().MoveRelative((float)(-instance.CurrentBundle.CurrentStage) * component.cellWidth * Vector3.right);
		}
		if (instance.CurrentBundle.CurrentStage > instance.CurrentBundle.LastAnimatedStage && instance.CurrentBundle.CurrentStage < instance.CurrentBundle.Stages.Count)
		{
			AnimateNewStage();
		}
	}

	private void AnimateNewStage()
	{
		RemoveLock();
		SpringPanel.target = (float)(-GameController.Instance.CurrentBundle.CurrentStage) * GetComponent<UIGrid>().cellWidth * Vector3.right;
		SpringPanel.enabled = true;
	}

	private void RemoveLock()
	{
		m_delegates[GameController.Instance.CurrentBundle.CurrentStage].GetComponentInChildren<LevelGridPage>().AnimateUnlock();
	}

	public void OnEnable()
	{
		if (m_started)
		{
			InitModel();
		}
	}

	public void OnDisable()
	{
		if (m_delegates == null)
		{
			return;
		}
		foreach (GameObject @delegate in m_delegates)
		{
			Object.Destroy(@delegate);
		}
		m_delegates.Clear();
	}
}
