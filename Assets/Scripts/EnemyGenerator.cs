using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
	[SerializeField] GameObject m_enemy;
	[SerializeField] float m_span;

	float m_delta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		m_delta += Time.deltaTime;

		if (m_delta > m_span)
		{
			m_delta = 0;
			Instantiate(m_enemy);
		}
    }
}
