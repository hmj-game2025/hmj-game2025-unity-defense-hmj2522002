using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
	[SerializeField] float m_volume = 1.0f;

	AudioSource m_audioSource;
	bool m_isPlaying;
	static BGM m_instance;
	public static BGM Instance => m_instance;
	public bool PlayBGM
	{
		set { m_isPlaying = value; }
	}

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
        m_audioSource = GetComponent<AudioSource>();
		m_isPlaying = true;
		m_audioSource.volume = m_volume;
    }

    // Update is called once per frame
    void Update()
    {
		if (m_isPlaying || m_audioSource.volume <= 0)
		{
			return;
		}

		m_audioSource.volume -= 0.05f;
		if (m_audioSource.volume < 0)
		{
			m_audioSource.volume = 0;
		}
    }
}
