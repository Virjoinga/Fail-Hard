using UnityEngine;

public class ConditionProgressbar : MonoBehaviour
{
	public int MaxCondition;

	private int m_currentCondition;

	public UILabel Label;

	private UIFilledSprite filledSprite;

	public int CurrentCondition
	{
		private get
		{
			return m_currentCondition;
		}
		set
		{
			m_currentCondition = value;
			if (filledSprite != null)
			{
				filledSprite.fillAmount = (float)value / (float)MaxCondition;
			}
			if (Label != null)
			{
				Label.text = CurrentCondition + "/" + MaxCondition;
			}
		}
	}

	private void Start()
	{
		filledSprite = GetComponentInChildren<UIFilledSprite>();
		Label = GetComponentInChildren<UILabel>();
		filledSprite.fillAmount = (float)CurrentCondition / (float)MaxCondition;
		Label.text = CurrentCondition + "/" + MaxCondition;
	}
}
