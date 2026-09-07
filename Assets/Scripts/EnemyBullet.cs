using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class EnemyBullet : MonoBehaviour
{
	[SerializeField] GameObject m_destroyedAttack;
	[SerializeField] GameObject m_hitBox;
	[SerializeField] float m_hight;

	List<Enemy.AttackStatus> m_attackStatus;
    Enemy m_parentEnemy;
	Rigidbody m_rigidbody;
	Quaternion m_angle;
	float m_power;
	float m_knockBackHorizontalPower;
	float m_knockBackVerticalPower;

	public Enemy BulletFromEnemy
	{
		set { m_parentEnemy = value; }
	}

	public Quaternion Angle
	{
        set { m_angle = value; }
    }

    private void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
	}

	// Start is called before the first frame update
	void Start()
    {
		m_attackStatus = m_parentEnemy.Status;
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
		if (m_rigidbody.velocity.y > 0.01f)
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
			attack.AttackFromEnemyStatus = m_attackStatus;
			attack.ClonedFromBullet = true;
			attack.Position = transform.position;
			attack.Angle = m_angle;
        }


		Destroy(gameObject);
	}
}
