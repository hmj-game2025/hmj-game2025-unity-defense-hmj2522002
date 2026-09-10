using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Title : MonoBehaviour
{
	[SerializeField] AudioClip m_seSelect;
	SceneChanger m_sceneChanger;
	AudioSource m_audioSource;
	static Title m_instance;

	public static Title Instance => m_instance;

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
        m_sceneChanger = SceneChanger.Instance;
		m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnSelect(InputAction.CallbackContext callbackContext)
	{
		if (m_sceneChanger.IsFade)
		{
			return;
		}

		m_sceneChanger.StartChangeScene("StageSelect", GameManager.SceneType.StageSelect);

		BGM.Instance.PlayBGM = false;

		m_audioSource.PlayOneShot(m_seSelect);
	}
}
