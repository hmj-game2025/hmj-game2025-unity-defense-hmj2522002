using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Castle : MonoBehaviour
{
	[SerializeField] BoxCollider m_hitBox;
	[SerializeField] CinemachineVirtualCamera m_camera;
	[SerializeField] float m_maxHp;

	GameManager m_gameManager;
	float m_hp;
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
    }

    // Update is called once per frame
    void Update()
    {
		// ƒQ[ƒ€Œ‹‰Ê‚Ì‘—M /////////////////////////////////////////////////////////////////////////////////////////////
		if (m_gameManager.IsGameOver && !m_isSendStatus)
		{
			m_isSendStatus = true;

			m_gameManager.CastleHp01 = GetHp01();
		}
	}

	public void Damage(float damage)
	{
		if (m_gameManager.IsGameOver)
		{
			return;
		}

		m_hp -= damage;

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
