using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] NavMeshAgent m_agent;
	[SerializeField] ParticleSystem m_damageHit;
	[SerializeField] float m_attackSpan;
	[SerializeField] float m_attackPower;
	[SerializeField] float m_moveSpeed;
	[SerializeField] float m_hp;
	[SerializeField] float m_playerChaseTime;

	const float InvincibleTime = 0.05f;

	Transform m_player;
	Transform m_target;
	float m_invincibleTime;

	private void Awake()
	{
		m_target = GameObject.FindWithTag("Castle").transform;
	}

	private void Start()
	{
		m_agent.speed = m_moveSpeed;
		m_player = Player.Instance.transform;
	}

	private void Update()
	{
		if (m_invincibleTime > 0)
		{
			m_invincibleTime -= Time.deltaTime;
		}
		Debug.Log(m_target.position);

		m_agent.SetDestination(m_target.position);
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
				Destroy(gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("CollisionHit");

		if (collision.gameObject.CompareTag("Attack"))
		{
			m_hp -= collision.gameObject.GetComponent<AttackPower>().Power;

			if (m_hp <= 0)
			{
				Destroy(gameObject);
			}
		}

	}
}
