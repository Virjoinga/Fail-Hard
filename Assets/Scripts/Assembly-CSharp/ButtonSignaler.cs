using System.Runtime.CompilerServices;
using UnityEngine;

public class ButtonSignaler : MonoBehaviour
{
	public delegate void OnButtonClick();

	[method: MethodImpl(32)]
	public event OnButtonClick ButtonClick;

	private void OnClick()
	{
		if (this.ButtonClick != null)
		{
			this.ButtonClick();
		}
	}
}
