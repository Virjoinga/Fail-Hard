using System.Runtime.CompilerServices;
using Game;
using UnityEngine;

public class AdColonyWrapper : MonoBehaviour
{
	public delegate void VideoEvent();

	public string AndroidApplicationKey = "appaf32ae500f4e42cf81";

	public string AndroidZoneId = "vzf310364d0114420898";

	public string iOSApplicationKey = "app4cd1928f2df5461695";

	public string iOSZoneId = "vzc1e2816072af4104aa";

	private string activeZoneId;

	[method: MethodImpl(32)]
	public event VideoEvent VideoClosed;

	public void Initialize()
	{
		AdColony.OnVideoFinished = OnVideoFinished;
		AdColony.OnV4VCResult = OnV4VCResult;
		AdColony.Configure("version:" + Version.CLIENT + ",store:google", AndroidApplicationKey, AndroidZoneId);
		activeZoneId = AndroidZoneId;
	}

	public void PlayV4VCAd()
	{
		if (AdColony.IsV4VCAvailable(activeZoneId))
		{
			AdColony.ShowV4VC(false, activeZoneId);
		}
	}

	public int VideoCount()
	{
		if (AdColony.IsV4VCAvailable(activeZoneId))
		{
			return 1;
		}
		return 0;
	}

	private void OnVideoFinished(bool ad_was_shown)
	{
		if (this.VideoClosed != null)
		{
			this.VideoClosed();
		}
	}

	private void OnV4VCResult(bool success, string name, int amount)
	{
		if (success)
		{
			GameController.Instance.Character.Coins += amount;
			Tracking.BusinessEvent(Tracking.Events.ApplifierAdWatched, "applifierAdCoins", amount);
			NotificationCentre.Send(Notification.Types.SupersonicAdWatched, amount);
		}
	}
}
