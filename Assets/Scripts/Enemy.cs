using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] NavMeshAgent m_agent;
	[SerializeField] ParticleSystem m_damageHit;
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
	float m_invincibleTime;
	float m_attackWaitTime;

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
	}

	private void Update()
	{
		if (m_invincibleTime > 0)
		{
			m_invincibleTime -= Time.deltaTime;
		}

		if (m_attackWaitTime > 0)
		{
			m_attackWaitTime -= Time.deltaTime;
		}

		m_agent.SetDestination(m_target.position);

		if ((m_target.position - transform.position).magnitude < m_attackReach)
		{

			if (m_attackWaitTime <= 0)
			{
				Debug.Log(gameObject.name.ToString() + "_actived");
				m_animator.SetTrigger("Attack");
				m_attackWaitTime = m_attackSpan;

				StartCoroutine(Attack());
			}
		}
	}

	void OnDeath()
	{
		m_generator.EnemyDeath();

		Destroy(gameObject);
	}

	IEnumerator Attack(float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);

		if (!m_attackObj)
		{
			yield break;
		}

		GameObject obj = Instantiate(m_attackObj, transform.position, transform.rotation);

		EnemyBullet bullet = obj.GetComponent<EnemyBullet>();
		if (bullet != null)
		{
			bullet.SetVelocity(transform.forward, m_attackReach);
			bullet.BulletFromEnemy = this;
		}

		EnemyAttack attack = obj.GetComponent<EnemyAttack>();

		if (attack != null)
		{
			attack.AttackFromEnemy = this;
		}

		if (!m_attackHitBox)
		{
			yield break;
		}

		GameObject hit = Instantiate(m_attackHitBox, transform.position, transform.rotation);

		attack = hit.GetComponent<EnemyAttack>();

		if (attack != null)
		{
			attack.AttackFromEnemy = this;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("TriggerHit");

		if (other.gameObject.CompareTag("Attack"))
		{
			if (m_invincibleTime > 0)
			{
				return;
			}

			m_hp -= other.GetComponent<AttackPower>().Power;
			m_invincibleTime = InvincibleTime;

			m_damageHit.Play();

			if (m_hp <= 0)
			{
				OnDeath();
			}
		}
	}

}
