using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] NavMeshAgent m_agent;
	[SerializeField] ParticleSystem m_damageHit;
	[SerializeField] GameObject m_attackObj;
	[SerializeField] Vector3 m_attackOffset;
	[SerializeField] float m_attackSpan;
	[SerializeField] float m_attackReach;
	[SerializeField] float m_attackPower;
	[SerializeField] float m_moveSpeed;
	[SerializeField] float m_hp;
	[SerializeField] float m_playerChaseTime;

	const float InvincibleTime = 0.05f;
	const float StopDist = 0.5f;
	const float MinimumDist = 1.05f;

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

				m_castle.Damage(m_attackPower);
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
			yield return 0;
		}

		GameObject go = Instantiate(m_attackObj);
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;

		EnemyBullet bullet = go.GetComponent<EnemyBullet>();
		if (bullet != null)
		{
			bullet.SetVelocity(transform.forward, m_attackReach);
		}

		AttackPower power = go.GetComponent<AttackPower>();
		if (power != null)
		{

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
