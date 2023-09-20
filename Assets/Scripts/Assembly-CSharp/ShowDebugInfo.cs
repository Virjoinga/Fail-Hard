using UnityEngine;

public class ShowDebugInfo : MonoBehaviour
{
	private UILabel label;

	private CounterUpdater cu;

	private void Awake()
	{
		cu = GetComponent<CounterUpdater>();
		cu.id = Version.PROTOCOL;
	}

	private void Start()
	{
		label = GetComponent<UILabel>();
		label.text = "version " + Version.CLIENT;
	}

	private void Update()
	{
	}
}
