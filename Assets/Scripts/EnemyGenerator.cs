using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
	enum Elemental	
	{
		None,
		Fire,
		Aqua,
		Ice,
		Elec,
		Wind,
		Glow,
		Dark,
	}

	[SerializeField] TextMeshProUGUI m_enemyAmount;
	[SerializeField] float m_waitNextWaveTime;

	[Serializable] 
	class SpawnInfo
	{
		public Vector3 position;
		public Elemental elemental;
		public float delay;
	}

	[Serializable]
	class Wave
	{
		public List<SpawnInfo> m_spawnInfos;
	}

	[SerializeField] List<GameObject> m_enemyTypes;
	[SerializeField] List<Wave> m_waves;

	GameManager m_gameManager;
	float m_delta;
	int m_nowWave;
	int m_enemyLeftInNowWave;
	int m_enemyLeftInThisGame;

	static EnemyGenerator m_instance;
	public static EnemyGenerator Instance => m_instance;

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
        m_nowWave = 0;
		StartCoroutine(SpawnEnemyInNowWave(m_waitNextWaveTime));

		m_gameManager = GameManager.Instance;

		// 個のステージで出てくる敵の数をカウントする
		m_enemyLeftInThisGame = 0;
		for (int wave = 0; wave < m_waves.Count; wave++)
		{
			for (int amount = 0; amount < m_waves[wave].m_spawnInfos.Count; amount++)
			{
				m_enemyLeftInThisGame++;
			}
		}

		UpdateEnemyAmountText();
    }

    // Update is called once per frame
    void Update()
    {
		m_delta += Time.deltaTime;
    }

	void UpdateEnemyAmountText()
	{
		m_enemyAmount.text = m_enemyLeftInThisGame.ToString();
	}

	IEnumerator SpawnEnemyInNowWave(float delay)
	{
		yield return new WaitForSeconds(delay);

		int enemyAmount = m_waves[m_nowWave].m_spawnInfos.Count;

		m_enemyLeftInNowWave = enemyAmount;

		for (int i = 0; i < enemyAmount; i++)
		{
			StartCoroutine(SpawnEnemy(m_nowWave, i));
		}
	}

	IEnumerator SpawnEnemy(int wave, int enemyNum)
	{
		SpawnInfo info = m_waves[wave].m_spawnInfos[enemyNum];
		GameObject enemy = m_enemyTypes[(int)info.elemental];
		Vector3 position = info.position;

		yield return new WaitForSeconds(info.delay);

		GameObject go = Instantiate(enemy, position, Quaternion.identity);
	}

	public void EnemyDeath()
	{
		// 残りの敵の数をカウントする
		m_enemyLeftInNowWave--;
		m_enemyLeftInThisGame--;
		UpdateEnemyAmountText();

		// ゲーム終了時に強制で止める
		if (!m_gameManager.IsGameOver)
		{
			if (m_enemyLeftInThisGame <= 0)
			{
				m_gameManager.IsGameOver = true;
			}
		}
		// 最終ウェーブの時、次のウェーブ（インデックス）を参照しないように止める
		if (m_nowWave >= m_waves.Count - 1)
		{
			return;
		}

		// 個のウェーブのすべての敵を倒したら次のウェーブの敵を出現させる
		if (m_enemyLeftInNowWave <= 0)
		{
			m_nowWave++;
			StartCoroutine(SpawnEnemyInNowWave(m_waitNextWaveTime));
		}
	}
}
