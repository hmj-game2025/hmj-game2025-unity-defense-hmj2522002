using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField] SceneType m_sceneType;

	const float GameOverWaitTime = 3.0f;
	const float RavelShowTime = 5.0f;
	const float RavelHidingTime = 1.0f;
	const float RavelShowWaitTime = 1.0f;

	SceneChanger m_sceneChanger;
	TextMeshProUGUI m_ravel;
	SceneType m_prevSceneType;
	string m_stageName;
	float m_playTime;
	float m_castleHp01;
	float m_gameOverElapsed;
	float m_startElapsed;
	int m_mainScore;
	bool m_isGameOver;
	bool m_prevGameOver;

	static GameManager m_instance;

	public SceneType ThisSceneType
	{
		get { return m_sceneType; }
		set { m_sceneType = value; }
	}
	public float PlayTime
	{
		get { return m_playTime; }
		set { m_playTime = value; }
	}
	public float CastleHp01
	{
		get { return m_castleHp01; }
		set { m_castleHp01 = value; }
	}
	public int MainScore
	{
		get { return m_mainScore; }
		set { m_mainScore = value; }
	}
	public bool IsGameOver
	{
		get { return m_isGameOver; }
		set { m_isGameOver = value; }
	}
	public string StageName
	{
		get { return m_stageName; }
		set { m_stageName = value; }
	}
	public static GameManager Instance => m_instance;

	public enum SceneType
	{
		Title,
		StageSelect,
		Game,
		Result
	}
	private void Awake()
	{
		if (m_instance != null && m_instance != this)
		{
			Destroy(gameObject);
			return;
		}

		m_instance = this;

		DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = 120;
	}

	// Start is called before the first frame update
	void Start()
    {
        if (SceneChanger.Instance == null || GameManager.Instance == null)
        {
            SceneManager.LoadScene("Title");
			m_sceneType = SceneType.Title;
        }
    }

    // Update is called once per frame
    void Update()
    {
		// シーンタイプが切り替わった瞬間 ///////////////////////////////////////////////////////////////////////////////
		if (m_prevSceneType != m_sceneType)
		{
			switch (m_sceneType)
			{
				case SceneType.Game:

					m_ravel = GameObject.FindWithTag("Ravel").GetComponent<TextMeshProUGUI>();

					m_isGameOver = false;
					m_startElapsed = 0;
					m_gameOverElapsed = 0;

					break;
			}

			m_prevSceneType = m_sceneType;
		}

		// シーンタイプごとの処理 ///////////////////////////////////////////////////////////////////////////////////////
		switch (m_sceneType)
		{
			case SceneType.Game:
				
				// 開始数秒間ステージ名を表示、そのあとゆっくり消える
				if (!m_isGameOver)
				{
					m_startElapsed += Time.deltaTime;

					m_ravel.text = m_stageName + "\nスタート！！";

					float alpha = 1 - Mathf.Clamp01(m_startElapsed - RavelShowTime - RavelHidingTime);
					Color color = Color.cyan;
					color.a = alpha;

					m_ravel.color = color;
				}

				if (m_isGameOver)
				{
					m_gameOverElapsed += Time.deltaTime;

					// 少し待ってから結果を表示
					if (m_gameOverElapsed > RavelShowWaitTime)
					{
						if (Castle.Instance.GetHp01() > 0)
						{
							m_ravel.text = "ステージクリア！！";
							m_ravel.color = Color.yellow;
						}
						else
						{
							m_ravel.text = "ゲームオーバー";
							m_ravel.color = Color.magenta;
						}
					}

					if (m_gameOverElapsed > GameOverWaitTime)
					{
						SceneChanger sceneChanger = SceneChanger.Instance;

						if (sceneChanger.IsFade)
						{
							break;
						}

						sceneChanger.StartChangeScene("Result", SceneType.Result);
					}
				}
				
				break;
		}

		m_prevGameOver = m_isGameOver;
    }
}
