using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
	GameObject m_hitBox;
	Vector3 m_angle;
	int m_attackAmount;
	float m_attackSpan;
	float m_attackWaitTime;

	const float HitBoxActiveTime = 0.1f;

	// Start is called before the first frame update
	void Start()
    {
		m_hitBox.SetActive(false);

		StartCoroutine(AttackHitBox());
    }

	public void SetAttackInfo(GameObject hitBox, Vector3 front, float length = 1.0f, int amount = 1, float span = 0.5f, float delay = 0)
	{
		m_hitBox = hitBox;
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

	IEnumerator AttackHitBox()
	{
		yield return m_attackWaitTime;

		m_hitBox.SetActive(true);

		yield return HitBoxActiveTime;

		m_hitBox.SetActive(false);

		Destroy(gameObject);
	}
}
