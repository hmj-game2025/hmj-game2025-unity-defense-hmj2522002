using System;
using System.Collections;
using System.Collections.Generic;
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

	float m_delta;
	int m_nowWave;
	int m_enemyLeftInNowWave;

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
    }

    // Update is called once per frame
    void Update()
    {
		m_delta += Time.deltaTime;
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

		GameObject go = Instantiate(enemy);
		go.transform.position = position;
	}

	public void EnemyDeath()
	{
		if (m_nowWave >= m_waves.Count - 1)
		{
			return;
		}

		m_enemyLeftInNowWave--;

		if (m_enemyLeftInNowWave <= 0)
		{
			m_nowWave++;
			StartCoroutine(SpawnEnemyInNowWave(m_waitNextWaveTime));
		}
	}
}
