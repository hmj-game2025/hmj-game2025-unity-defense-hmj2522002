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
	[SerializeField] Transform m_highScoreUi;
	[SerializeField] TextMeshProUGUI m_totalScore;
	[SerializeField] AudioClip m_seSelect;
	AudioSource m_audioSource;
	GameManager m_gameManager;
	SaveData m_saveData;

	const int TimeBonus = 30000;
	const int CastleBonus = 10000;

	int m_baseScore;
	float m_time;
	float m_castleHp01;

    // Start is called before the first frame update
    void Start()
    {
		// SceneChangerかGameManagerがシーン上になかったらタイトルに戻る
		// とはいってもあくまでデバッグ用。Titleシーン上に二つともあり、基本的に通過するのでこれは動作しないだろう
		if (SceneChanger.Instance == null || GameManager.Instance == null)
		{
			SceneManager.LoadScene("Title");
			return;
		}

		m_gameManager = GameManager.Instance;
		m_saveData = SaveData.Instance;
		m_audioSource = GetComponent<AudioSource>();

		// ベーススコア（敵を倒したときの合計スコア）
		m_baseScore = m_gameManager.MainScore;
		m_time = m_gameManager.PlayTime;
		m_castleHp01 = m_gameManager.CastleHp01;

		m_baseScoreUi.transform.Find("Text").GetComponent<TextMeshProUGUI>().text
			= "ベーススコア";
		m_baseScoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>().text
			= "+ " + m_baseScore;

		// タイムボーナス（クリアタイムごとのスコア）
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

		// 城体力ボーナス（体力の割合ごとのスコア）
		int hpBonus = (int)(m_castleHp01 * CastleBonus);

		m_CastleDamageUi.transform.Find("Text").GetComponent<TextMeshProUGUI>().text
			= "城HP ( のこり " + (((int)(m_castleHp01 * 10000)) / 100.0f).ToString() + " % )";
		m_CastleDamageUi.transform.Find("Score").GetComponent<TextMeshProUGUI>().text
			= "+ " + hpBonus;

		// ボーナススコア込みの合計スコア
		int totalScore = m_baseScore + timeBonus + hpBonus;

		m_totalScore.text = (totalScore).ToString();

		m_gameManager.TotalScore = totalScore;

		// ハイスコア表記
		int highScore = m_saveData.Read(m_gameManager.StageNum);

		TextMeshProUGUI highScoreText = m_highScoreUi.transform.Find("Text").GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI highScoreNum = m_highScoreUi.transform.Find("Score").GetComponent<TextMeshProUGUI>();

		if (highScore < totalScore)
		{
			highScoreText.text = "ハイスコアこうしん！！";
			highScoreText.color = Color.yellow;

			highScoreNum.text = totalScore.ToString();
			highScoreNum.color = Color.yellow;
		}
		else
		{
			highScoreText.text = "ハイスコア";			

			highScoreNum.text = highScore.ToString();
		}
	}

	public void OnCollect(InputAction.CallbackContext callbackContext)
	{
		if (SceneChanger.Instance.IsFade)
		{
			return;
		}

		SceneChanger.Instance.StartChangeScene("StageSelect", GameManager.SceneType.StageSelect);

		BGM.Instance.PlayBGM = false;

		m_audioSource.PlayOneShot(m_seSelect);
	}
}
