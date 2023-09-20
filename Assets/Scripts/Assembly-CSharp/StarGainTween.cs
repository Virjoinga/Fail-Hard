using Holoville.HOTween;
using UnityEngine;

public class StarGainTween : MonoBehaviour
{
	public GameObject Target;

	public float ScaleTime;

	public EaseType ScaleEase;

	public float StartScaleMultiplier;

	public float EndScaleMultiplier;

	public float AlphaFadeInTime;

	private Transform m_transform;

	private void Start()
	{
		Setup();
	}

	public void Begin()
	{
		if (m_transform == null)
		{
			Setup();
		}
		Target.SetActive(true);
		Vector3 vector = m_transform.localScale * EndScaleMultiplier;
		m_transform.localScale = vector * StartScaleMultiplier;
		TweenParms p_parms = new TweenParms().Prop("localScale", vector).Ease(ScaleEase);
		HOTween.To(Target.transform, ScaleTime, p_parms);
		UISprite component = Target.GetComponent<UISprite>();
		component.alpha = 0f;
		p_parms = new TweenParms().Prop("alpha", 1f);
		HOTween.To(component, AlphaFadeInTime, p_parms);
	}

	private void Setup()
	{
		if (Target == null)
		{
			Target = base.gameObject;
		}
		m_transform = Target.transform;
	}
}
