using Game;
using UnityEngine;

public class NavigateFromTo : MonoBehaviour
{
	public static UIPanel FromPanelForBackStep;

	public UIPanel fromPanel;

	public UIPanel toPanel;

	public bool TriggerAutomatically = true;

	public float navigationDelay;

	public void OnClick()
	{
		if (TriggerAutomatically)
		{
			ManualTrigger();
		}
	}

	public void ManualTrigger()
	{
		Invoke("NavigateWithCutscenes", 0.01f);
	}

	public void NavigateBack()
	{
		if (FromPanelForBackStep == null)
		{
			Debug.LogWarning("No back panel. Cannot back step.");
			return;
		}
		toPanel = FromPanelForBackStep;
		ManualTrigger();
	}

	private void NavigateWithCutscenes()
	{
		LocalStorage.Instance.SyncDirtyToDisk();
		FromPanelForBackStep = fromPanel;
		fromPanel.gameObject.SetActive(false);
		toPanel.gameObject.SetActive(true);
	}
}
