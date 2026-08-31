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

	[SerializeField] GameObject m_enemy;

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

	[SerializeField] List<Wave> m_waves;

	float m_delta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		m_delta += Time.deltaTime;
    }
}
