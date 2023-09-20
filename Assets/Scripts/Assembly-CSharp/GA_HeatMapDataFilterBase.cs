using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GA_HeatMapDataFilterBase : MonoBehaviour
{
	public delegate void DataDownloadDelegate(GA_HeatMapDataFilterBase sender);

	[method: MethodImpl(32)]
	public event DataDownloadDelegate OnDataUpdate;

	public abstract List<GA_DataPoint> GetData();

	public void OnDataUpdated()
	{
		if (this.OnDataUpdate != null)
		{
			this.OnDataUpdate(this);
		}
	}
}
