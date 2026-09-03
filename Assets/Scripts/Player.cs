using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] GameObject m_attackHit;
	[SerializeField] GameObject m_spinAttackHit;
	[SerializeField] GameObject m_sword;
	[SerializeField] GameObject m_playerObj;
	[SerializeField] float m_boostWalkSpeed;		// 速度収束量
	[SerializeField] float m_boostMagnificationAir;	// 空中での収束率
	[SerializeField] float m_maxWalkSpeed;          // サイコウソク
	[SerializeField] float m_attackDamage;			// 通常攻撃でのダメージ量
	[SerializeField] float m_spinAttackDamage;		// 回転攻撃での一発ごとのダメージ量
	[SerializeField] float m_swordEffectDelay;      // 斬撃のエフェクトが描画されるまでの時間
	[SerializeField] float m_comboAttackDelay;      // 連続攻撃ができる猶予時間
	[SerializeField] float m_cantAttackDuration;    // 攻撃入力を受け付けない時間
	[SerializeField] float m_attackStartDuration;   // 攻撃発生までの時間
	[SerializeField] float m_attackHitKeepDuration; // 攻撃判定の持続時間
	[SerializeField] float m_cantMoveAfterSpinAttackTime;	// 回転攻撃後の硬直
	[SerializeField] float m_moveHorizontalSpeed;   // 通常時の船の横移動量
	[SerializeField] float m_jumpPower;				// ジャンプ力
	[SerializeField] float m_gravityScale;          // 落下速度
	[SerializeField] float m_minStunTime;			// スタンの最小時間
	[SerializeField] int m_spinAttacksSpinAmount;   // 回転攻撃の回転数

	const float SpinSpeed = 800.0f;
	const int ComboAttacks = 4;
	const float StickActiveMin = 0.5f;

	Camera m_camera;
	Animator m_animator;
	CharacterController m_controller;
	Vector3 m_startSpinAttackRotation;
	Vector3 m_totalMove;
	Vector3 m_stunMove;
	Vector2 m_moveXZ;
	Vector2 m_prevMoveXZ;
	Vector2 m_leftStickControll;
	Vector2 m_rightStickControll;
	float m_walkSpeed;
	float m_attackedDelay;
	float m_baseHorizontalPos;
	float m_nowHorizontalPos;
	float m_rotateDirection;
	float m_boatBoost;
	float m_maxWidth;
	float m_stunTimeLeft;
	float m_speedY;
	float m_spinAttackTime;
	float m_prevSpinAttackTime;
	float m_stunElapsedTime;
	int m_totalScore;
	int m_comboAmount;
	bool m_isComboAttackReady;
	bool m_isShield;
	bool m_prevShield;
	bool m_isPlayedGoalAnim;
	bool m_canControll;
	bool m_isPressedShield;
	bool m_canBasicMove;	// 移動、ジャンプの基本的な動作ができるか
	bool m_canTotalMove;    // 攻撃なども含めたすべての動作ができるか
	bool m_isStun;

	enum AttackType
	{
		None,
		Attack,
		SpinAttack
	}

	static Player m_instance;

	public static Player Instance => m_instance;

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}

		m_controller = GetComponent<CharacterController>();
		m_animator = m_playerObj.GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
    {
		m_camera = Camera.Instance;

		for (int i = 0; i < m_attackHit.transform.childCount; i++)
		{
			m_attackHit.transform.GetChild(i).GetComponent<AttackPower>().Power = m_attackDamage;
		}

		for (int i = 0; i < m_spinAttackHit.transform.childCount; i++)
		{
			m_spinAttackHit.transform.GetChild(i).GetComponent<AttackPower>().Power = m_spinAttackDamage;
		}
	}

    // Update is called once per frame
    void Update()
    {
		m_attackedDelay += Time.deltaTime;

		float boostMagnification = 
			m_controller.isGrounded ? 1.0f : m_boostMagnificationAir;

		m_canBasicMove = m_comboAttackDelay < m_attackedDelay;

		m_canTotalMove = !m_isStun;

		// 縦横移動 /////////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_moveXZ.magnitude <= m_leftStickControll.magnitude)
		{
			if (m_canBasicMove && m_canTotalMove)
			{
				m_moveXZ += m_leftStickControll.normalized * m_boostWalkSpeed * boostMagnification;
			}

			// 速度超過を防ぐため、最高速で止める
			if (m_moveXZ.magnitude > m_leftStickControll.magnitude)
			{
				m_moveXZ = m_moveXZ.normalized * m_leftStickControll.magnitude;
			}
		}
		if (m_moveXZ.magnitude > m_leftStickControll.magnitude ||
			m_leftStickControll.magnitude <= 0 ||
			!m_canBasicMove ||
			!m_canTotalMove
			)
		{
			m_moveXZ -= m_moveXZ.normalized * m_boostWalkSpeed * boostMagnification;

			// ムーンウォークを防ぐために0で止める
			if (m_moveXZ.magnitude < 0.05f)
			{
				m_moveXZ = Vector2.zero;
			}
		}

		m_walkSpeed = m_moveXZ.magnitude;

		Vector3 moveX_Z = new(m_moveXZ.x, 0, m_moveXZ.y);
		moveX_Z *= m_maxWalkSpeed;

		m_speedY -= m_gravityScale;

		m_totalMove = (moveX_Z + Vector3.up * m_speedY) * Time.deltaTime;

		m_controller.Move(m_totalMove);

		// 移動中の向き、アニメーション速度の制御 ///////////////////////////////////////////////////////////////////////
		if (m_leftStickControll.magnitude > 0 &&
			m_canBasicMove)
		{
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(new Vector3(m_leftStickControll.x, 0, m_leftStickControll.y)),
				0.1f);

			m_animator.speed = m_leftStickControll.magnitude;
		}
		else
		{
			m_animator.speed = 1;
		}

		// 回転攻撃中の回転制御 /////////////////////////////////////////////////////////////////////////////////////////
		if (0 < m_spinAttackTime)
		{
			m_spinAttackTime -= SpinSpeed * m_spinAttacksSpinAmount * Time.deltaTime;

			if (m_spinAttackTime % 360 > m_prevSpinAttackTime % 360)
			{
				if (m_prevSpinAttackTime == 360.0f * m_spinAttacksSpinAmount)
				{
					StartCoroutine(TryAttack(AttackType.SpinAttack, "Attack02"));
				}
				else
				{
					StartCoroutine(TryAttack(AttackType.SpinAttack));
				}
			}
		}

		m_prevSpinAttackTime = m_spinAttackTime;

		if (0 < m_spinAttackTime)
		{
			Vector3 forward = new(Mathf.Sin(-m_spinAttackTime / 180.0f * Mathf.PI), 0, Mathf.Cos(-m_spinAttackTime / 180.0f * Mathf.PI));

			transform.rotation = Quaternion.LookRotation(forward);

			transform.eulerAngles += m_startSpinAttackRotation;
		}

		// 右スティック操作でのカメラワーク /////////////////////////////////////////////////////////////////////////////
		if (m_rightStickControll.x > StickActiveMin ||
			m_rightStickControll.y > StickActiveMin)
		{
			m_camera.ZoomIn();
		}

		if (m_rightStickControll.x < -StickActiveMin ||
			m_rightStickControll.y < -StickActiveMin)
		{
			m_camera.ZoomOut();
		}

		// スタン時 /////////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_isStun)
		{
			m_stunElapsedTime += Time.deltaTime;

			if (m_controller.isGrounded)
			{
				if (m_stunElapsedTime > m_minStunTime)
				{
					m_isStun = false;
					m_animator.SetBool("IsStun", false);
				}
			}
			else
			{
				Vector3 stunMove = m_stunMove * Time.deltaTime;

				m_controller.Move(stunMove);
			}
		}

		// 接地判定 /////////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_controller.isGrounded)
		{
			m_speedY = -0.2f;
		}
		else
		{
			m_animator.SetFloat("SpeedY", m_speedY);
		}

		m_animator.SetBool("IsGrounded", m_controller.isGrounded);
	}

	public void OnMove(InputAction.CallbackContext callbackContext)
	{
		m_leftStickControll = callbackContext.ReadValue<Vector2>();

		if (callbackContext.performed)
		{
			m_animator.SetBool("IsRun", true);
		}
		else
		{
			m_animator.SetBool("IsRun", false);
		}
	}

	public void OnAttack(InputAction.CallbackContext callbackContext)
	{
		if (m_attackedDelay < m_cantAttackDuration)
		{
			return;
		}
		if (!callbackContext.performed)
		{
			return;
		}
		if (!m_controller.isGrounded)
		{
			return;
		}
		if (!m_canTotalMove)
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
			m_comboAmount = 0;
		}

		m_comboAmount++;

		m_attackedDelay = 0;

		if (m_comboAmount % 2 == 1)
		{
			StartCoroutine(TryAttack(AttackType.Attack, "Attack01", m_swordEffectDelay, m_attackHitKeepDuration));
		}
		else
		{
			if (m_comboAmount == ComboAttacks)
			{
				m_comboAmount = 0;

				m_spinAttackTime = 360.0f * m_spinAttacksSpinAmount;
				m_prevSpinAttackTime = m_spinAttackTime;
				m_startSpinAttackRotation = transform.eulerAngles;

				// コンボが途切れる長さにし、さらに連続で攻撃できないようにする
				m_attackedDelay = -m_cantMoveAfterSpinAttackTime;
			}
			else
			{
				StartCoroutine(TryAttack(AttackType.Attack, "Attack02", m_swordEffectDelay, m_attackHitKeepDuration));
			}
		}
	}

	public void OnJump(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed)
		{
			return;
		}
		if (!m_canBasicMove)
		{
			return;
		}
		if (!m_canTotalMove)
		{
			return;
		}

		if (m_controller.isGrounded)
		{
			m_speedY = m_jumpPower;

			m_animator.SetTrigger("Jump");
		}
	}

	public void OnCamera(InputAction.CallbackContext callbackContext)
	{
		m_rightStickControll = callbackContext.ReadValue<Vector2>();
	}

	IEnumerator TryAttack(AttackType type, string animName = "", float delay = 0.1f, float keep = 0.1f)
	{

		if (type == AttackType.Attack)
		{
			StartCoroutine(AttackHitBox(m_attackHit, keep));
		}
		else if (type == AttackType.SpinAttack)
		{
			StartCoroutine(AttackHitBox(m_spinAttackHit, keep));
		}

		if (animName != "")
		{
			m_animator.SetTrigger(animName);

			yield return new WaitForSeconds(delay);

			m_sword.transform.Find("SwordEffect").GetComponent<ParticleSystem>().Play();
		}
	}

	IEnumerator AttackHitBox(GameObject hitBox, float keep = 0.1f)
	{
		yield return new WaitForSeconds(m_attackStartDuration);

		hitBox.SetActive(true);

		yield return new WaitForSeconds(keep);

		hitBox.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Damage"))
		{
			EnemyAttack enemyAttack = other.GetComponent<EnemyAttack>();

			if (enemyAttack)
			{
				Vector3 rota = (transform.position - other.transform.position).normalized;

				rota.x *= enemyAttack.KnockBackHorizontalPower;
				rota.z *= enemyAttack.KnockBackHorizontalPower;

				m_speedY = enemyAttack.KnockBackVerticalPower;

				m_stunMove = new(rota.x, 0, rota.z);

				m_animator.SetTrigger("Damaged");
				m_animator.SetBool("IsStun", true);

				m_isStun = true;
			}
		}
	}
}
