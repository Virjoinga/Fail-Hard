using System.Collections;
using System.Runtime.CompilerServices;
using Holoville.HOTween;
using UnityEngine;

public class Photo : MonoBehaviour
{
	public delegate void OnItemClicked();

	public UITexture Texture;

	public UIWidget Frame;

	private Texture m_photoTexture;

	private Quaternion m_initialRotation;

	private Vector3 m_initialPosition;

	private bool m_colliderScaled;

	[method: MethodImpl(32)]
	public event OnItemClicked ItemClicked;

	private void Start()
	{
		Texture.mainTexture = m_photoTexture;
		m_colliderScaled = false;
		if (m_initialPosition != Vector3.zero && m_initialRotation != Quaternion.identity)
		{
			TweenParms tweenParms = new TweenParms().Prop("localScale", Vector3.one);
			tweenParms.Prop("localPosition", Vector3.zero);
			tweenParms.Prop("localRotation", Vector3.zero);
			tweenParms.Ease(EaseType.EaseOutBack);
			base.transform.localScale = new Vector3(0.1f, 0.1f);
			base.transform.localRotation = m_initialRotation;
			base.transform.localPosition = m_initialPosition;
			HOTween.To(base.transform, 0.4f, tweenParms);
		}
	}

	private void LateUpdate()
	{
		if (!m_colliderScaled)
		{
			m_colliderScaled = true;
			StartCoroutine(ScaleCollider());
		}
	}

	public void SetData(Texture photo, Quaternion rotation, Vector3 position)
	{
		m_photoTexture = photo;
		m_initialRotation = rotation;
		m_initialPosition = position;
	}

	private IEnumerator ScaleCollider()
	{
		yield return null;
		Transform child = base.transform.GetChild(0);
		BoxCollider collider = GetComponent<BoxCollider>();
		collider.size = child.localScale;
	}

	public void OnClick()
	{
		if (this.ItemClicked != null)
		{
			this.ItemClicked();
		}
	}
}
