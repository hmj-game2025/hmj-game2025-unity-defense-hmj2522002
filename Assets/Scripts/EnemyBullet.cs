using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField] GameObject m_destroyedAttack;
	[SerializeField] float m_hight;

	Rigidbody m_rigidbody;
	float m_power;

	private void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetVelocity(Vector3 front, float distance)
	{
		m_rigidbody.velocity = (front * distance) + Vector3.up * m_hight;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_rigidbody.velocity.y > 0)
		{
			return;
		}

		GameObject go = Instantiate(m_destroyedAttack);
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;
	//	go.GetComponent<AttackPower>().Power = m_power;

		Destroy(gameObject);
	}
}
