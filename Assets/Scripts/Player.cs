using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] GameObject m_myAttackHit;
	[SerializeField] GameObject m_sword;
	[SerializeField] GameObject m_playerObj;
	[SerializeField] float m_boostWalkSpeed;
	[SerializeField] float m_maxWalkSpeed;
	[SerializeField] float m_swordEffectDelay;      // 斬撃のエフェクトが描画されるまでの時間
	[SerializeField] float m_comboAttackDelay;      // 連続攻撃ができる猶予時間
	[SerializeField] float m_cantAttackDuration;    // 攻撃入力を受け付けない時間
	[SerializeField] float m_attackStartDuration;   // 攻撃発生までの時間
	[SerializeField] float m_attackHitKeepDuration; // 攻撃判定の持続時間
	[SerializeField] float m_moveHorizontalSpeed;   // 通常時の船の横移動量
	[SerializeField] float m_tiltPower;             // 船が傾く量
	[SerializeField] float m_rotateSpeed;           // フィーバー時等にプレイヤーの向きを変える速度
	[SerializeField] float m_canShieldDelay;        // 前動作からシールドできるまでの猶予

	Animator m_animator;
	CharacterController m_controller;
	Vector3 m_totalMove;
	Vector2 m_moveXZ;
	Vector2 m_prevMoveXZ;
	Vector2 m_stickControll;
	float m_walkSpeed;
	float m_attackedDelay;
	float m_baseHorizontalPos;
	float m_nowHorizontalPos;
	float m_rotateDirection;
	float m_boatBoost;
	float m_maxWidth;
	float m_stunTimeLeft;
	int m_totalScore;
	bool m_isComboAttackReady;
	bool m_isShield;
	bool m_prevShield;
	bool m_isPlayedGoalAnim;
	bool m_canControll;
	bool m_isPressedShield;


	private void Awake()
	{
		m_controller = GetComponent<CharacterController>();
		m_animator = m_playerObj.GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
		m_attackedDelay += Time.deltaTime;

		if (m_moveXZ.magnitude <= m_stickControll.magnitude)
		{
			m_moveXZ += m_stickControll.normalized * m_boostWalkSpeed;
			m_walkSpeed = m_moveXZ.magnitude;
			Debug.Log("Plus");

			if (m_moveXZ.magnitude > m_stickControll.magnitude)
			{
				m_moveXZ = m_moveXZ.normalized;
			}
		}
		if (m_moveXZ.magnitude > m_stickControll.magnitude ||
			m_stickControll.magnitude <= 0)
		{
			m_moveXZ -= m_moveXZ.normalized * m_boostWalkSpeed;
			m_walkSpeed = m_moveXZ.magnitude;
			Debug.Log("Minus");

			if (m_moveXZ.magnitude < 0.05f)
			{
				m_moveXZ = Vector2.zero;
			}
		}

		m_walkSpeed = m_moveXZ.magnitude;

		Debug.Log(m_walkSpeed);

		Vector3 moveX_Z = new(m_moveXZ.x, 0, m_moveXZ.y);
		moveX_Z *= m_maxWalkSpeed;

		m_totalMove = moveX_Z * Time.deltaTime;

		m_controller.Move(m_totalMove);


		//Vector3 moveXZ = new(m_stickControll.x, 0, m_stickControll.y);
		//m_moveXZ += moveXZ * m_maxWalkSpeed * m_boostWalkSpeed;

		//if (m_moveXZ.magnitude > m_maxWalkSpeed)
		//{
		//	m_moveXZ = m_moveXZ.normalized * m_maxWalkSpeed;
		//}

		//Debug.Log(m_moveXZ.magnitude);

		//m_moveXZ = m_moveXZ * Time.deltaTime;

		//m_totalMove = m_moveXZ;

		//m_controller.Move(m_totalMove);

		//m_prevMoveXZ = moveXZ;
    }

	public void OnMove(InputAction.CallbackContext callbackContext)
	{
		m_stickControll = callbackContext.ReadValue<Vector2>();
	}

	public void OnAttack(InputAction.CallbackContext callbackContext)
	{
		Debug.Log("Attacked");

		if (m_attackedDelay < m_cantAttackDuration)
		{
			return;
		}
		if (!callbackContext.performed)
		{
			return;
		}
		//if (m_isShield)
		//{
		//	return;
		//}
		//if (m_stageInfo.IsGoaled)
		//{
		//	return;
		//}
		//if (!m_canControll)
		//{
		//	return;
		//}
		//if (IsStun)
		//{
		//	return;
		//}

		if (m_comboAttackDelay < m_attackedDelay)
		{
			m_isComboAttackReady = false;
		}

		if (!m_isComboAttackReady)
		{
			StartCoroutine(TryAttack("Attack01", m_swordEffectDelay));
			m_isComboAttackReady = true;
		}
		else
		{
			StartCoroutine(TryAttack("Attack02", m_swordEffectDelay));
			m_isComboAttackReady = false;
		}

		m_attackedDelay = 0;
	}


	IEnumerator TryAttack(string animName, float delay = 0.1f)
	{
		StartCoroutine(AttackHitBox());

		m_animator.SetTrigger(animName);

		yield return new WaitForSeconds(delay);

		m_sword.transform.Find("SwordEffect").GetComponent<ParticleSystem>().Play();
	}

	IEnumerator AttackHitBox()
	{
		yield return new WaitForSeconds(m_attackStartDuration);

		m_myAttackHit.SetActive(true);

		yield return new WaitForSeconds(m_attackHitKeepDuration);

		m_myAttackHit.SetActive(false);
	}

}
