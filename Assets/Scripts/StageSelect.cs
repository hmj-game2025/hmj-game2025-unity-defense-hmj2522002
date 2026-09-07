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
	}

	[SerializeField] GameObject m_elementIcon;
	[SerializeField] GameObject m_playerObj;
	[SerializeField] Transform m_spawnElements;
	[SerializeField] TextMeshProUGUI m_stageName;
	[SerializeField] Vector3 m_iconOffset;
	[SerializeField] float m_iconDist;
	[SerializeField] List<StageNameInfo> m_stages;

	SceneChanger m_sceneChanger;
	Vector2 m_leftStick;
	Vector2 m_prevLeft;
	int m_nowCursor;
	bool m_isChangedCursor;

	static StageSelect m_instance;

	const float StickActivePower = 0.5f;

	public static StageSelect Instance => m_instance;


	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}

		m_sceneChanger = SceneChanger.Instance;
	}

	// Start is called before the first frame update
	void Start()
    {
		if (SceneChanger.Instance == null || GameManager.Instance == null)
		{
			SceneManager.LoadScene("Title");
		}

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
		}
		if (m_leftStick.x < -StickActivePower && m_prevLeft.x >= -StickActivePower)
		{
			m_nowCursor--;
			m_isChangedCursor = true;

			if (m_nowCursor < 0)
			{
				m_nowCursor = 0;
			}
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

		// 新しく選択されたステージのアイコンを貼り付ける
		for (int i = 0; i < elements.Count; i++)
		{
			GameObject icon = Instantiate(m_elementIcon, m_spawnElements);

			icon.GetComponent<Image>().sprite = elements[i];

			Vector3 pos = new(m_iconDist * (i - (elements.Count - 1) / 2.0f), 0);

			icon.transform.localPosition = pos + m_iconOffset;
		}
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

		m_sceneChanger.StartChangeScene("Level_01", GameManager.SceneType.Game);
	}
}
