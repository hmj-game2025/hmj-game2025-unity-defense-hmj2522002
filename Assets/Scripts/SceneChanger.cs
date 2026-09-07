using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	[SerializeField] GameObject m_fade;

	static SceneChanger m_instance;

	GameManager.SceneType m_sceneType;
	RectTransform m_fadeRect;
	Vector2 m_screenSize;
	float m_fadeTime;
	bool m_isStartFadeOut;
	bool m_isFade;
	string m_sceneName;

	public static SceneChanger Instance => m_instance;
	public bool IsFade => m_isFade;

	private void Awake()
	{
		if (m_instance != null && m_instance != this)
		{
			Destroy(gameObject);
			return;
		}

		m_instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		ResetContext();

		DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log(m_isFade);
		if (!m_isStartFadeOut && m_fadeTime <= 0)
		{
			m_isFade = false;
			return;
		}
		else
		{
			m_isFade = true;
		}

		if (m_isStartFadeOut)
		{
			m_fadeTime += Time.deltaTime;
			if (1 <= m_fadeTime)
			{
				// シーン名にUnity_Endが書かれていたら、アプリを終了する
				if (m_sceneName == "Unity_End")
				{
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
				}
				else
				{
					SceneManager.LoadScene(m_sceneName);
					GameManager.Instance.ThisSceneType = m_sceneType;
					ResetContext();
				}
			}
		}
		else
		{
			m_fadeTime -= Time.deltaTime;

			if (m_fadeTime <= 0)
			{
				m_fade.SetActive(false);
			}
		}

		float posY = Easing.EaseOutCubic(m_screenSize.y, 0, m_fadeTime);
		m_fadeRect.transform.localPosition = new(0, posY);
    }

	public void StartChangeScene(string sceneName, GameManager.SceneType sceneType)
	{
		if (!m_isStartFadeOut)
		{
			m_isStartFadeOut = true;
			m_sceneName = sceneName;
			m_fade.SetActive(true);
			m_sceneType = sceneType;
		}
	}

	public void ResetContext()
	{
		m_screenSize = new(Screen.width, Screen.height);

		m_fadeRect = m_fade.GetComponent<RectTransform>();
		m_fadeRect.sizeDelta = new(m_screenSize.x, m_screenSize.y);

		m_isStartFadeOut = false;
		m_fadeTime = 1;
	}
}
