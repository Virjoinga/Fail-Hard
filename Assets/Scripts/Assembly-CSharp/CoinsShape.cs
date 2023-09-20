using System.Collections.Generic;
using UnityEngine;

public class CoinsShape : MonoBehaviour
{
	public float CoinValue;

	public int CoinAmount;

	public GameObject CoinPrefab;

	public bool Visible = true;

	public bool PhysicsOn;

	protected List<GameObject> m_coins;

	protected virtual void Start()
	{
		m_coins = new List<GameObject>();
	}

	public void TogglePhysics(bool newState)
	{
		PhysicsOn = newState;
		foreach (GameObject coin in m_coins)
		{
			coin.GetComponent<CoinCollider>().SetPhysics(newState);
		}
		if (newState)
		{
			StopAnimations();
		}
	}

	public void ToggleVisible(bool newState)
	{
		Visible = newState;
		foreach (GameObject coin in m_coins)
		{
			coin.SetActive(newState);
		}
	}

	public void StopAnimations()
	{
		foreach (GameObject coin in m_coins)
		{
			DiamondAnimator component = coin.GetComponent<DiamondAnimator>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}
}
