using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
	[SerializeField] float m_maxHp;

	float m_hp;

	static Castle m_instance;
	public static Castle Instance => m_instance;

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
		m_hp = m_maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Damage(float damage)
	{
		m_hp -= damage;
	}

	public float GetHp01()
	{
		return m_hp / m_maxHp;
	}
}
