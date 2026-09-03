using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField] GameObject m_destroyedAttack;
	[SerializeField] GameObject m_hitBox;
	[SerializeField] float m_hight;

	Enemy m_parentEnemy;
	Rigidbody m_rigidbody;
	float m_power;
	float m_knockBackHorizontalPower;
	float m_knockBackVerticalPower;

	public Enemy BulletFromEnemy
	{
		set { m_parentEnemy = value; }
	}

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
		go.transform.rotation = Quaternion.Euler(transform.forward);
		
		GameObject hb = Instantiate(m_hitBox);
		hb.transform.position = transform.position;

		EnemyAttack attack = hb.GetComponent<EnemyAttack>();

		if (attack != null)
		{
			attack.AttackFromEnemy = m_parentEnemy;
			attack.ClonedFromBullet = true;
			//attack.AttackAmount = 1;
			//attack.Angle = Quaternion.Euler(transform.forward);
			//attack.AttackWaitTime = 0.5f;
			//attack.AttackHitStayTime = 1.0f;
			//attack.KnockBackHorizontalPower = m_knockBackHorizontalPower;
			//attack.KnockBackVerticalPower = m_knockBackVerticalPower;
			//attack.AttackPower = m_power;
		}


		Destroy(gameObject);
	}
}
