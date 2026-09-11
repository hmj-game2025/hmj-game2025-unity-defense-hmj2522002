using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Castle : MonoBehaviour
{
	[SerializeField] GameObject m_explosion;
	[SerializeField] BoxCollider m_hitBox;
	[SerializeField] CinemachineVirtualCamera m_camera;
	[SerializeField] AudioClip m_seDamage;
	[SerializeField] float m_maxHp;

	const float ExplosionDuration = 0.05f;
	const float ExplosionRandonDist = 3.0f;

	GameManager m_gameManager;
	AudioSource m_audioSource;
	float m_hp;
	float m_explodeElapsedTime;
	bool m_isSendStatus;

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
		m_gameManager = GameManager.Instance;
		m_hp = m_maxHp;
		m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		// ゲーム結果の送信 /////////////////////////////////////////////////////////////////////////////////////////////
		if (m_gameManager.IsGameOver)
		{
			if (!m_isSendStatus)
			{
				m_isSendStatus = true;

				m_gameManager.CastleHp01 = GetHp01();
			}

			m_explodeElapsedTime += Time.deltaTime;

			// 爆破エフェクト
			if (m_hp <= 0)
			{
				if (m_explodeElapsedTime > ExplosionDuration)
				{
					m_explodeElapsedTime = 0;

					Vector3 randomOffset = new(
						Random.Range(-ExplosionRandonDist, ExplosionRandonDist),
						Random.Range(0, ExplosionRandonDist * 2.0f)
						);

					Instantiate(m_explosion, transform.position + randomOffset, transform.rotation);

					if (SeCoolDown.Instance.CanPlay(m_seDamage))
					{
						m_audioSource.PlayOneShot(m_seDamage);
					}
				}
			}
		}
	}

	// 城にダメージ /////////////////////////////////////////////////////////////////////////////////////////////////////
	public void Damage(float damage)
	{
		if (m_gameManager.IsGameOver)
		{
			return;
		}

		m_hp -= damage;
		if (SeCoolDown.Instance.CanPlay(m_seDamage))
		{
			m_audioSource.PlayOneShot(m_seDamage);
		}

		if (m_hp <= 0)
		{
			m_hp = 0;

			m_camera.Follow = gameObject.transform;
			m_camera.LookAt = null;

			m_gameManager.IsGameOver = true;

			BGM.Instance.PlayBGM = false;
		}
	}

	public float GetHp01()
	{
		return m_hp / m_maxHp;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Damage"))
		{
			Damage(other.GetComponent<EnemyAttack>().AttackPower);
		}
	}
}
