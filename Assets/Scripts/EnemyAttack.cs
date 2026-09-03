using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	SphereCollider m_hitBox;
	Enemy m_parentEnemy;
	Quaternion m_angle;
	int m_attackAmount = 1;
	int m_runningCoroutines;
	int m_attackStatusIndex;
	float m_attackSpan;
	float m_attackWaitTime;
	float m_attackHitStayTime;
	float m_knockBackHorizontalPower;
	float m_knockBackVerticalPower;
	float m_attackPower;
	float m_forwardOffset;
	float m_cullentToSwitchedTime;
	bool m_clonedFromBullet;

	public Quaternion Angle
	{
		set { m_angle = value; }
	}
	public int AttackAmount
	{
		set { m_attackAmount = value; }
	}
	public float AttackSpan
	{
		set { m_attackSpan = value; }
	}
	public float AttackWaitTime
	{
		set { m_attackWaitTime = value; }
	}
	public float AttackHitStayTime
	{
		set { m_attackHitStayTime = value; }
	}
	public float KnockBackHorizontalPower
	{
		get { return m_knockBackHorizontalPower; }
		set { m_knockBackHorizontalPower = value; }
	}
	public float KnockBackVerticalPower
	{
		get { return m_knockBackVerticalPower; }
		set { m_knockBackVerticalPower = value; }
	}
	public float AttackPower
	{
		get { return m_attackPower; }
		set { m_attackPower = value; }
	}

	public Enemy AttackFromEnemy
	{
		set { m_parentEnemy = value; }
	}

	public bool ClonedFromBullet
	{
		set { m_clonedFromBullet = value; }
	}

	private void Awake()
	{
		m_hitBox = GetComponent<SphereCollider>();
	}

	// Start is called before the first frame update
	void Start()
	{
		m_hitBox.enabled = false;

		Enemy.AttackStatus stats = m_parentEnemy.Status[m_attackStatusIndex];

		m_angle = Quaternion.Euler(m_parentEnemy.gameObject.transform.forward);
		m_attackAmount = stats.attackAmount;
		m_attackWaitTime = stats.attackWaitTime;
		m_attackHitStayTime = stats.attackHitStayTime;
		m_knockBackHorizontalPower = stats.knockBackHorizontalPower;
		m_knockBackVerticalPower = stats.knockBackVerticalPower;
		m_attackPower = stats.attackPower;
		m_forwardOffset = stats.startForwardOffset;

		m_hitBox.radius = stats.attackRadius;

		// 敵から直接生成されたダメージ判定なら、敵の位置に判定を生成する
		if (!m_clonedFromBullet)
		{
			transform.position = m_parentEnemy.transform.position;
			transform.position += transform.forward * m_forwardOffset;
		}

		m_cullentToSwitchedTime = -m_attackWaitTime;
	}

	private void Update()
	{
		m_cullentToSwitchedTime += Time.deltaTime;

		if (!m_hitBox.enabled)
		{
			if (m_cullentToSwitchedTime >= m_attackSpan)
			{
				m_hitBox.enabled = true;
				m_cullentToSwitchedTime = 0;
			}
		}
		else
		{
			if (m_cullentToSwitchedTime >= m_attackHitStayTime)
			{
				m_hitBox.enabled = false;
				m_cullentToSwitchedTime = 0;
				m_attackAmount--;

				if (m_attackAmount <= 0)
				{
					gameObject.SetActive(false);
					Destroy(gameObject);
				}
			}
		}
	}

	public void SetAttackInfo(GameObject hitBox, Quaternion front, float length = 1.0f, int amount = 1, float span = 0.5f, float delay = 0)
	{
		m_angle = front;
		m_attackAmount = amount;
		m_attackSpan = span;
		m_attackWaitTime = delay;

		SphereCollider sphere = GetComponent<SphereCollider>();

		if (sphere != null)
		{
			sphere.radius = length / 2.0f;
		}
	}

	IEnumerator AttackHitBox(float waitSpan = 0.0f)
	{
		yield return new WaitForSeconds(m_attackWaitTime + waitSpan);

		m_hitBox.enabled = true;

		yield return new WaitForSeconds(m_attackHitStayTime);

		m_hitBox.enabled = false;
		Debug.Log("false");

		m_runningCoroutines--;
	}
}
