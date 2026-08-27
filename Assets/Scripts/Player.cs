using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	[SerializeField] GameObject m_myAttackHit;
	[SerializeField] GameObject m_sword;
	[SerializeField] GameObject m_playerObj;
	[SerializeField] float m_boostWalkSpeed;		// 速度収束量
	[SerializeField] float m_boostMagnificationAir;	// 空中での収束率
	[SerializeField] float m_maxWalkSpeed;			// サイコウソク
	[SerializeField] float m_swordEffectDelay;      // 斬撃のエフェクトが描画されるまでの時間
	[SerializeField] float m_comboAttackDelay;      // 連続攻撃ができる猶予時間
	[SerializeField] float m_cantAttackDuration;    // 攻撃入力を受け付けない時間
	[SerializeField] float m_attackStartDuration;   // 攻撃発生までの時間
	[SerializeField] float m_attackHitKeepDuration; // 攻撃判定の持続時間
	[SerializeField] float m_cantMoveAfterSpinAttackTime;	// 回転攻撃後の硬直
	[SerializeField] float m_moveHorizontalSpeed;   // 通常時の船の横移動量
	[SerializeField] float m_jumpPower;				// ジャンプ力
	[SerializeField] float m_gravityScale;          // 落下速度
	[SerializeField] int m_spinAttacksSpinAmount;   // 回転攻撃の回転数

	const float SpinSpeed = 800.0f;
	const int ComboAttacks = 4;
	const float StickActiveMin = 0.5f;

	Camera m_camera;
	Animator m_animator;
	CharacterController m_controller;
	Vector3 m_startSpinAttackRotation;
	Vector3 m_totalMove;
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
	int m_totalScore;
	int m_comboAmount;
	bool m_isComboAttackReady;
	bool m_isShield;
	bool m_prevShield;
	bool m_isPlayedGoalAnim;
	bool m_canControll;
	bool m_isPressedShield;
	bool m_canBasicMove;	// 移動、ジャンプの基本的な動作ができるか
	bool m_canTotalMove;	// 攻撃なども含めたすべての動作ができるか


	private void Awake()
	{
		m_controller = GetComponent<CharacterController>();
		m_animator = m_playerObj.GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
    {
		m_camera = Camera.Instance;
	}

    // Update is called once per frame
    void Update()
    {
		m_attackedDelay += Time.deltaTime;

		float boostMagnification = 
			m_controller.isGrounded ? 1.0f : m_boostMagnificationAir;

		m_canBasicMove = m_comboAttackDelay < m_attackedDelay;

		// 縦横移動 /////////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_moveXZ.magnitude <= m_leftStickControll.magnitude)
		{
			if (m_canBasicMove)
			{
				m_moveXZ += m_leftStickControll.normalized * m_boostWalkSpeed * boostMagnification;
			}

			Debug.Log("Plus");

			// 速度超過を防ぐため、最高速で止める
			if (m_moveXZ.magnitude > m_leftStickControll.magnitude)
			{
				m_moveXZ = m_moveXZ.normalized * m_leftStickControll.magnitude;
			}
		}
		if (m_moveXZ.magnitude > m_leftStickControll.magnitude ||
			m_leftStickControll.magnitude <= 0 ||
			!m_canBasicMove
			)
		{
			m_moveXZ -= m_moveXZ.normalized * m_boostWalkSpeed * boostMagnification;
			Debug.Log("Minus");

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
		}

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
		Debug.Log("Attacked");

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
			StartCoroutine(TryAttack("Attack01", m_swordEffectDelay, m_attackHitKeepDuration));
		}
		else
		{
			if (m_comboAmount == ComboAttacks)
			{
				m_comboAmount = 0;

				m_spinAttackTime = 360.0f * m_spinAttacksSpinAmount;
				m_startSpinAttackRotation = transform.eulerAngles;

				// コンボが途切れる長さにし、さらに連続で攻撃できないようにする
				m_attackedDelay = -m_cantMoveAfterSpinAttackTime;

				StartCoroutine(TryAttack("Attack02", m_swordEffectDelay, m_attackHitKeepDuration * 3.0f));
			}
			else
			{
				StartCoroutine(TryAttack("Attack02", m_swordEffectDelay, m_attackHitKeepDuration));
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

	IEnumerator TryAttack(string animName, float delay = 0.1f, float keep = 0.1f)
	{
		StartCoroutine(AttackHitBox(keep));

		m_animator.SetTrigger(animName);

		yield return new WaitForSeconds(delay);

		m_sword.transform.Find("SwordEffect").GetComponent<ParticleSystem>().Play();
	}

	IEnumerator AttackHitBox(float keep = 0.1f)
	{
		yield return new WaitForSeconds(m_attackStartDuration);

		m_myAttackHit.SetActive(true);

		yield return new WaitForSeconds(keep);

		m_myAttackHit.SetActive(false);
	}
}
