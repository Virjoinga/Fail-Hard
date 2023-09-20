using UnityEngine;

public class CoinsShapeLine : CoinsShape
{
	public float Length;

	protected override void Start()
	{
		base.Start();
		SpawnShape();
	}

	private void SpawnShape()
	{
		float num = ((CoinAmount != 1) ? (Length / (float)(CoinAmount - 1)) : Length);
		for (int i = 0; i < CoinAmount; i++)
		{
			GameObject gameObject = Object.Instantiate(CoinPrefab, base.transform.position + num * (float)i * base.transform.right, Quaternion.identity) as GameObject;
			m_coins.Add(gameObject);
			gameObject.GetComponent<CoinCollider>().SetPhysics(PhysicsOn);
		}
	}

	public void Reset()
	{
		if (m_coins.Count > 0)
		{
			m_coins.Clear();
		}
	}
}
