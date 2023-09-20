using UnityEngine;

[PBSerialize("CoinsShapeSpline")]
public class CoinsShapeSpline : CoinsShape
{
	[PBSerializeField]
	public float Length;

	[PBSerializeField]
	public float Deviation;

	public Spline CoinSpline;

	protected override void Start()
	{
		base.Start();
		GameObject gameObject = CoinSpline.AddSplineNode();
		gameObject.transform.position = base.transform.position;
		gameObject = CoinSpline.AddSplineNode();
		gameObject.transform.position = base.transform.position + 0.33f * Length * base.transform.right + Deviation * base.transform.up;
		gameObject = CoinSpline.AddSplineNode();
		gameObject.transform.position = base.transform.position + 0.67f * Length * base.transform.right + Deviation * base.transform.up;
		gameObject = CoinSpline.AddSplineNode();
		gameObject.transform.position = base.transform.position + Length * base.transform.right;
		Invoke("SpawnShape", 0.1f);
	}

	private void SpawnShape()
	{
		float num = ((CoinAmount != 1) ? (1f / (float)(CoinAmount - 1)) : 1f);
		for (int i = 0; i < CoinAmount; i++)
		{
			float num2 = CoinSpline.ConvertNormalizedParameterToDistance(1f);
			float param = CoinSpline.ConvertDistanceToNormalizedParameter((float)i * num * num2);
			GameObject gameObject = Object.Instantiate(CoinPrefab, CoinSpline.GetPositionOnSpline(param), Quaternion.identity) as GameObject;
			gameObject.GetComponent<CoinCollider>().SetPhysics(PhysicsOn);
			m_coins.Add(gameObject);
		}
	}
}
