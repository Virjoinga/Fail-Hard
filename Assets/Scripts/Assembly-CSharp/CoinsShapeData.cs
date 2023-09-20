using UnityEngine;

public class CoinsShapeData : CoinsShape
{
	public VectorData data;

	protected override void Start()
	{
		base.Start();
		if (!(data != null))
		{
			return;
		}
		CoinAmount = data.data.Count;
		foreach (Vector3 datum in data.data)
		{
			SpawnCoin(datum);
		}
	}

	private void SpawnCoin(Vector3 localPos)
	{
		GameObject gameObject = Object.Instantiate(CoinPrefab, base.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = localPos;
		gameObject.GetComponent<CoinCollider>().Value = CoinValue;
		gameObject.GetComponent<CoinCollider>().SetPhysics(PhysicsOn);
		m_coins.Add(gameObject);
	}
}
