using Game;
using Game.Util;
using UnityEngine;

[PBSerialize("BreakingGlass")]
public class BreakingGlass : MonoBehaviour
{
	public GameObject WholeGlass;

	public GameObject BrokenGlass;

	public AudioSource AudioSource;

	public AudioClip GlassBreakingSound;

	[PBSerializeField]
	public bool triggerOnPlayer = true;

	[PBSerializeField]
	public bool triggerOnLevelObject = true;

	[PBSerializeField]
	public bool triggerOnce;

	private bool triggered;

	[PBSerializeField]
	public float DisappearTimeInS;

	public bool HideOnDisappear;

	[PBSerializeField]
	public float BreakingThreshold;

	private bool m_broken;

	private GigStatus m_gigController;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Level:root");
		if ((bool)gameObject)
		{
			m_gigController = gameObject.GetComponentInChildren<GigStatus>();
		}
		else
		{
			Logger.Error("No level root was found in scene!");
		}
	}

	private void OnCollisionEnter(Collision hit)
	{
		if (!m_broken && CheckColliderMatch(hit.gameObject) && hit.contacts.Length > 0 && hit.rigidbody.velocity.magnitude > BreakingThreshold)
		{
			Break();
		}
	}

	public void Break()
	{
		if (m_broken)
		{
			return;
		}
		m_broken = true;
		WholeGlass.SetActive(false);
		BrokenGlass.SetActive(true);
		base.collider.enabled = false;
		if (base.rigidbody != null)
		{
			Object.Destroy(base.rigidbody);
		}
		if (AudioSource != null)
		{
			AudioManager.Instance.Play(AudioSource, GlassBreakingSound, 0.5f, AudioTag.ItemCrashAudio2);
		}
		else if (base.audio != null)
		{
			AudioManager.Instance.Play(base.audio, GlassBreakingSound, 0.5f, AudioTag.ItemCrashAudio2);
		}
		if (m_gigController != null)
		{
			m_gigController.CollateralDamage(1f, base.transform.position);
		}
		if (DisappearTimeInS > 0f)
		{
			if (HideOnDisappear)
			{
				Object.Destroy(base.gameObject, DisappearTimeInS);
			}
			else
			{
				Invoke("DisablePhysics", DisappearTimeInS);
			}
		}
	}

	private void DisablePhysics()
	{
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.isKinematic = true;
			rigidbody.collider.enabled = false;
		}
	}

	private bool CheckColliderMatch(GameObject hit)
	{
		bool result = false;
		if (!triggered)
		{
			if (triggerOnPlayer && (hit.gameObject.tag == "Vehicle" || hit.gameObject.tag == "Player"))
			{
				triggered = true;
				result = true;
			}
			if (triggerOnLevelObject && hit.gameObject.tag == "DynamicLevelObject")
			{
				result = true;
				triggered = true;
			}
		}
		return result;
	}
}
