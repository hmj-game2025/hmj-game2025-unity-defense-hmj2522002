using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeCoolDown : MonoBehaviour
{
	[Serializable]
	class Se
	{
		public AudioClip clip;
		public float coolDown;
	}

	[SerializeField] float m_coolDown = 0.2f;

	static SeCoolDown m_instance;
	public static SeCoolDown Instance => m_instance;

	List<Se> m_ses = new();

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// クリップごとの効果音重複防止のクールダウン
		for (int i = 0; i < m_ses.Count; i++) 
		{
			m_ses[i].coolDown -= Time.deltaTime;

			if (m_ses[i].coolDown <= 0)
			{
				m_ses.Remove(m_ses[i]);
			}
		}
    }

	public bool CanPlay(AudioClip clip)
	{
		foreach (Se se in m_ses)
		{
			if (se.clip == clip)
			{
				return false;
			}
		}

		Se newSe = new();

		newSe.clip = clip;
		newSe.coolDown = m_coolDown;

		m_ses.Add(newSe);

		return true;
	}
}
