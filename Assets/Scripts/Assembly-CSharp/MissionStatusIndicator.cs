using UnityEngine;

public class MissionStatusIndicator : MonoBehaviour
{
	public UILabel MissionStatusLabel;

	public void SetState(bool done)
	{
		if (done)
		{
			MissionStatusLabel.text = "LEVEL COMPLETED!";
		}
		else
		{
			MissionStatusLabel.text = "LEVEL FAILED!";
		}
	}
}
