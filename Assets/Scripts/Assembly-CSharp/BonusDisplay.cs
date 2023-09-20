using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusDisplay : MonoBehaviour
{
	private class BonusData
	{
		public string Type;

		public int Bonus;
	}

	public BonusScoreItem Item1;

	public BonusScoreItem Item2;

	private BonusScoreItem m_showing;

	private BonusScoreItem m_waiting;

	private List<BonusData> m_bonusQueue = new List<BonusData>();

	private void Start()
	{
		Item1.BonusHidden += ConsumeQueue;
		Item2.BonusHidden += ConsumeQueue;
	}

	public void CreateNewBonus(string type, int bonus)
	{
		StartCoroutine(ShowBonus(type, bonus));
	}

	private IEnumerator ShowBonus(string type, int bonus)
	{
		yield return null;
		bool update = false;
		if (m_showing != null && m_showing.Type == type)
		{
			m_showing.UpdateScore(bonus);
			update = true;
		}
		else if (m_waiting != null && m_waiting.Type == type)
		{
			m_waiting.UpdateScore(bonus);
			update = true;
		}
		if (update)
		{
			yield break;
		}
		if (Item1.IsHidden && Item2.IsHidden)
		{
			m_showing = Item1;
			m_showing.SetData(type, bonus);
			m_showing.SetShowing();
			yield break;
		}
		if (m_showing != null && m_waiting != null)
		{
			m_bonusQueue.Add(new BonusData
			{
				Type = type,
				Bonus = bonus
			});
			yield break;
		}
		if (m_showing == Item1)
		{
			m_waiting = Item2;
		}
		else
		{
			m_waiting = Item1;
		}
		m_waiting.SetData(type, bonus);
		m_waiting.SetReady();
	}

	private void ConsumeQueue(BonusScoreItem item)
	{
		if (m_bonusQueue.Count > 0)
		{
			BonusData bonusData = m_bonusQueue[0];
			m_bonusQueue.RemoveAt(0);
			item.SetData(bonusData.Type, bonusData.Bonus);
			m_waiting.SetShowing();
			m_showing = m_waiting;
			m_waiting = item;
			m_waiting.SetReady();
		}
		else if (m_waiting != null)
		{
			m_waiting.SetShowing();
			m_showing = m_waiting;
			m_waiting = null;
		}
		else
		{
			m_showing = null;
		}
	}
}
