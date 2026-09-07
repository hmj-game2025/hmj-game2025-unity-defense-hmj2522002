using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] NavMeshAgent m_agent;
	[SerializeField] GameObject m_damageObj;
	[SerializeField] GameObject m_deathObj;
	[SerializeField] GameObject m_attackObj;
	[SerializeField] GameObject m_attackHitBox;
	[SerializeField] float m_attackSpan;
	[SerializeField] float m_attackReach;
	[SerializeField] float m_attackPower;
	[SerializeField] float m_moveSpeed;
	[SerializeField] float m_hp;
	[SerializeField] float m_playerChaseTime;

	const float InvincibleTime = 0.05f;
	const float StopDist = 0.5f;
	const float MinimumDist = 1.05f;
	const float StunTime = 0.1f;
	const float DeathStunTime = 0.5f;

	[Serializable]
	public class AttackStatus
	{
		public float startForwardOffset;
		public Quaternion angle;
		public int attackAmount;
		public float attackWaitTime;
		public float attackSpan;
		public float attackHitStayTime;
		public float knockBackHorizontalPower;
		public float knockBackVerticalPower;
		public float attackPower;
		public float attackRadius;
	}

	[SerializeField] List<AttackStatus> m_attackStatus;

	public List<AttackStatus> Status => m_attackStatus;

	Castle m_castle;
	Transform m_playerObj;
	Transform m_castleObj;
	Transform m_target;
	EnemyGenerator m_generator;
	Animator m_animator;
	float m_invincibleTimeLeft;
	float m_attackWaitTimeLeft;
	float m_playerChaseTimeLeft;
	float m_stunTimeLeft;
	float m_baseSpeed;
	bool m_isDeath;
	bool m_isTargetInReach;

	private void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();
		m_castleObj = GameObject.FindWithTag("Castle").transform;
		m_target = m_castleObj;
	}

	private void Start()
	{
		m_agent.speed = m_moveSpeed;
		m_agent.stoppingDistance = Mathf.Clamp(m_attackReach - StopDist, MinimumDist, m_attackReach);
		m_playerObj = Player.Instance.transform;
		m_generator = EnemyGenerator.Instance;
		m_castle = Castle.Instance;
		m_baseSpeed = m_moveSpeed;

		// 召喚されるときのパーティクル
		Instantiate(m_deathObj, transform.position, transform.rotation);
	}

	private void Update()
	{
		// 残り時間系 ///////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_invincibleTimeLeft > 0)
		{
			m_invincibleTimeLeft -= Time.deltaTime;
		}
		if (m_attackWaitTimeLeft > 0)
		{
			m_attackWaitTimeLeft -= Time.deltaTime;
		}
		if (m_stunTimeLeft > 0)
		{
			m_stunTimeLeft -= Time.deltaTime;
		}
		if (m_playerChaseTimeLeft > 0)
		{
			m_playerChaseTimeLeft -= Time.deltaTime;
		}

		// 追尾するターゲットを選択する /////////////////////////////////////////////////////////////////////////////////
		if (m_playerChaseTimeLeft > 0)
		{
			m_target = m_playerObj.transform;
		}
		else
		{
			m_target = m_castleObj.transform;
		}

		m_agent.SetDestination(m_target.position);

		// 攻撃 /////////////////////////////////////////////////////////////////////////////////////////////////////////
		m_isTargetInReach = (m_target.position - transform.position).magnitude < m_attackReach;
		
		if (m_isTargetInReach)
		{
			Vector3 angle = m_target.position - transform.position;

			transform.LookAt(m_target);

            if (m_attackWaitTimeLeft <= 0)
			{
				Debug.Log(gameObject.name.ToString() + "_actived");
				m_animator.SetTrigger("Attack");
				m_attackWaitTimeLeft = m_attackSpan;

				StartCoroutine(Attack());
			}
		}

		// スピード調整 /////////////////////////////////////////////////////////////////////////////////////////////////
		// 射程範囲内にターゲットがあるかスタンしている時にスピードをゼロにしている
		if (m_isTargetInReach || m_stunTimeLeft > 0)
		{
			m_agent.speed = 0.0f;
		}
		else
		{
			m_agent.speed = m_baseSpeed;
		}

		m_animator.SetBool("IsStun", m_stunTimeLeft > 0);
	}

	// 倒されたとき /////////////////////////////////////////////////////////////////////////////////////////////////////
	void OnDeath(float delay = 0.0f)
	{
		m_generator.EnemyDeath();

		Destroy(gameObject, delay);
	}

    private void OnDestroy()
    {
		if (m_isDeath)
		{
			Instantiate(m_deathObj, transform.position, transform.rotation);
		}
    }

    // ターゲットに攻撃 /////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator Attack(float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);

		if (!m_attackObj)
		{
			yield break;
		}

		GameObject obj = Instantiate(m_attackObj, transform.position, transform.rotation);

		// 弾を発射して攻撃するタイプ
		EnemyBullet bullet = obj.GetComponent<EnemyBullet>();

		if (bullet != null)
		{
			bullet.SetVelocity(transform.forward, m_attackReach);
			bullet.BulletFromEnemy = this;
            bullet.Angle = Quaternion.Euler(gameObject.transform.forward);
        }

        // 直接殴って攻撃するタイプ
        EnemyAttack attack = obj.GetComponent<EnemyAttack>();

		if (attack != null)
		{
			attack.AttackFromEnemyStatus = m_attackStatus;
		}

		if (!m_attackHitBox)
		{
			yield break;
		}

		GameObject hit = Instantiate(m_attackHitBox, transform.position, transform.rotation);

		attack = hit.GetComponent<EnemyAttack>();

		// 攻撃先の敵を自分に指定する（攻撃の情報を伝えるため）
		if (attack != null)
		{
			attack.AttackFromEnemyStatus = m_attackStatus;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("TriggerHit");

		// プレイヤーからの攻撃を受けたとき /////////////////////////////////////////////////////////////////////////////
		if (other.gameObject.CompareTag("Attack"))
		{
			if (m_invincibleTimeLeft > 0)
			{
				return;
			}

			m_hp -= other.GetComponent<AttackPower>().Power;
			m_invincibleTimeLeft = InvincibleTime;

			Instantiate(m_damageObj, transform.position, transform.rotation);

			// スタンする時間の指定
			if (m_hp <= 0)
			{
				if (!m_isDeath)
				{
					m_stunTimeLeft = DeathStunTime;
					OnDeath(DeathStunTime);
					m_isDeath = true;
				}
			}
			else
			{
				m_stunTimeLeft = StunTime;
				m_playerChaseTimeLeft = m_playerChaseTime;
			}
		}
	}

}
