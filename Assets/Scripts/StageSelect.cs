using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
	[Serializable]
	class StageNameInfo
	{
		public string stageName;
		public Vector3 position;
		public string sceneName;
		public List<Sprite> element;
		public int timeBonus;
	}

	[SerializeField] GameObject m_elementIcon;
	[SerializeField] GameObject m_playerObj;
	[SerializeField] Transform m_spawnElements;
	[SerializeField] TextMeshProUGUI m_stageName;
	[SerializeField] TextMeshProUGUI m_highScore;
	[SerializeField] AudioClip m_seSelect;
	[SerializeField] AudioClip m_seMove;
	[SerializeField] Vector3 m_iconOffset;
	[SerializeField] float m_iconDist;
	[SerializeField] List<StageNameInfo> m_stages;

	GameManager m_gameManager;
	SceneChanger m_sceneChanger;
	AudioSource m_audioSource;
	Vector2 m_leftStick;
	Vector2 m_prevLeft;
	int m_nowCursor;
	bool m_isChangedCursor;

	static StageSelect m_instance;

	public const int StageAmount = 3;
	const float StickActivePower = 0.5f;

	public static StageSelect Instance => m_instance;


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
		if (SceneChanger.Instance == null || GameManager.Instance == null)
		{
			SceneManager.LoadScene("Title");
		}

		m_sceneChanger = SceneChanger.Instance;
		m_gameManager = GameManager.Instance;
		m_audioSource = GetComponent<AudioSource>();

		RefreshUis();
    }

    // Update is called once per frame
    void Update()
    {
		// カーソル位置 /////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_leftStick.x > StickActivePower && m_prevLeft.x <= StickActivePower)
		{
			m_nowCursor++;
			m_isChangedCursor = true;

			if (m_nowCursor > m_stages.Count - 1)
			{
				m_nowCursor = m_stages.Count - 1;
			}

			m_audioSource.PlayOneShot(m_seMove);
		}
		if (m_leftStick.x < -StickActivePower && m_prevLeft.x >= -StickActivePower)
		{
			m_nowCursor--;
			m_isChangedCursor = true;

			if (m_nowCursor < 0)
			{
				m_nowCursor = 0;
			}

			m_audioSource.PlayOneShot(m_seMove);
		}

		m_prevLeft = m_leftStick;

		if (!m_isChangedCursor)
		{
			return;
		}

		RefreshUis();
	}

	void RefreshUis()
	{
		m_isChangedCursor = false;

		// ステージ名 ///////////////////////////////////////////////////////////////////////////////////////////////////
		m_stageName.text = m_stages[m_nowCursor].stageName;

		// プレイヤーオブジェクトの位置 /////////////////////////////////////////////////////////////////////////////////
		m_playerObj.transform.position = m_stages[m_nowCursor].position;

		// でてくるエレメダマ ///////////////////////////////////////////////////////////////////////////////////////////
		// すでにあるアイコンを全消去する
		foreach (Transform child in m_spawnElements)
		{
			Destroy(child.gameObject);
		}

		List<Sprite> elements = m_stages[m_nowCursor].element;

		// 新しく選択されたステージに出てくるエレメダマアイコンを貼り付ける
		for (int i = 0; i < elements.Count; i++)
		{
			GameObject icon = Instantiate(m_elementIcon, m_spawnElements);

			icon.GetComponent<Image>().sprite = elements[i];

			Vector3 pos = new(m_iconDist * (i - (elements.Count - 1) / 2.0f), 0);

			icon.transform.localPosition = pos + m_iconOffset;
		}

		// 個のステージのハイスコア /////////////////////////////////////////////////////////////////////////////////////

		m_highScore.text = SaveData.Instance.Read(m_nowCursor).ToString();
	}

	public void OnMove(InputAction.CallbackContext callbackContext)
	{
		m_leftStick = callbackContext.ReadValue<Vector2>();
	}

	public void OnCollect(InputAction.CallbackContext callbackContext)
	{
		if (m_sceneChanger.IsFade)
		{
			return;
		}

		// ステージに飛ぶ ///////////////////////////////////////////////////////////////////////////////////////////////
		m_gameManager.StageName = m_stages[m_nowCursor].stageName;

		m_sceneChanger.StartChangeScene(m_stages[m_nowCursor].sceneName, GameManager.SceneType.Game);

		// 同時に、最後にプレイしたステージ番号を記憶
		m_gameManager.StageNum = m_nowCursor;

		// 個のステージのタイムボーナスも記憶
		m_gameManager.TimeBonus = m_stages[m_nowCursor].timeBonus;

		BGM.Instance.PlayBGM = false;

		m_audioSource.PlayOneShot(m_seSelect);
	}

	public void OnBack(InputAction.CallbackContext callbackContext)
	{
		if (m_sceneChanger.IsFade)
		{
			return;
		}

		m_sceneChanger.StartChangeScene("Title", GameManager.SceneType.Title);

		BGM.Instance.PlayBGM = false;
	}
}
