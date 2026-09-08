using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
	[SerializeField] Transform m_baseScoreUi;
	[SerializeField] Transform m_timeBonusUi;
	[SerializeField] Transform m_CastleDamageUi;
	[SerializeField] TextMeshProUGUI m_totalScore;
	GameManager m_gameManager;

	const int TimeBonus = 30000;
	const int CastleBonus = 10000;

	int m_baseScore;
	float m_time;
	float m_castleHp01;

    // Start is called before the first frame update
    void Start()
    {
		if (SceneChanger.Instance == null || GameManager.Instance == null)
		{
			SceneManager.LoadScene("Title");
			return;
		}

		m_gameManager = GameManager.Instance;

		m_baseScore = m_gameManager.MainScore;
		m_time = m_gameManager.PlayTime;
		m_castleHp01 = m_gameManager.CastleHp01;

		m_baseScoreUi.transform.Find("Text").GetComponent<TextMeshProUGUI>().text
			= "ベーススコア";
		m_baseScoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>().text
			= "+ " + m_baseScore;

		string time = FloatToTime.ChangeFloatToTime(m_time);
		int timeBonus = TimeBonus - (int)(m_time * 100);
		if (timeBonus < 0)
		{
			timeBonus = 0;
		}

		m_timeBonusUi.transform.Find("Text").GetComponent<TextMeshProUGUI>().text
			= "クリアタイム ( " + time + " )";
		m_timeBonusUi.transform.Find("Score").GetComponent<TextMeshProUGUI>().text
			= "+ " + timeBonus;

		int hpBonus = (int)(m_castleHp01 * CastleBonus);

		m_CastleDamageUi.transform.Find("Text").GetComponent<TextMeshProUGUI>().text
			= "城HP ( のこり " + (int)(m_castleHp01 * 100) + " % )";
		m_CastleDamageUi.transform.Find("Score").GetComponent<TextMeshProUGUI>().text
			= "+ " + hpBonus;

		m_totalScore.text = (m_baseScore + timeBonus + hpBonus).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnCollect(InputAction.CallbackContext callbackContext)
	{
		if (SceneChanger.Instance.IsFade)
		{
			return;
		}

		SceneChanger.Instance.StartChangeScene("StageSelect", GameManager.SceneType.StageSelect);
	}
}
