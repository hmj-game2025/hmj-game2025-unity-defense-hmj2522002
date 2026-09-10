using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject m_screen;
	[SerializeField] GameObject m_buttons;
	[SerializeField] GameObject m_arrow;
	[SerializeField] AudioClip m_seSelect;
	[SerializeField] AudioClip m_seMove;

	const float StickActivePower = 0.5f;
	const int PauseWaitControllFrame = 2;

	AudioSource m_audioSource;
	Vector2 m_leftStick;
	Vector2 m_prevLeft;
	float m_pausedElapsedTime;
	int m_waitControllFrame;
	int m_buttonAmount;
	int m_nowCursor;
    bool m_isPause;

    static PauseMenu m_instance;

    public bool IsPause => m_isPause;
    public static PauseMenu Instance => m_instance;

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
        m_buttonAmount = m_buttons.transform.childCount;
		m_nowCursor = 0;
		m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		if (m_isPause)
		{
			m_pausedElapsedTime += Time.deltaTime;
		}
		else
		{
			return;
		}

		if (m_waitControllFrame > 0)
		{
			m_waitControllFrame--;
		}

		// カーソル位置 /////////////////////////////////////////////////////////////////////////////////////////////////
		if (m_leftStick.y > StickActivePower && m_prevLeft.y <= StickActivePower)
		{
			m_nowCursor--;
			
			if (m_nowCursor < 0)
			{
				m_nowCursor = 0;
			}

			m_audioSource.PlayOneShot(m_seSelect);
		}
		if (m_leftStick.y < -StickActivePower && m_prevLeft.y >= -StickActivePower)
		{
			m_nowCursor++;

			if (m_nowCursor > m_buttonAmount - 1)
			{
				m_nowCursor = m_buttonAmount - 1;
			}
		}

		m_arrow.transform.position = m_buttons.transform.GetChild(m_nowCursor).transform.position;

		m_prevLeft = m_leftStick;
    }

	void Continue()
	{
		m_isPause = false;
		m_screen.SetActive(false);
		Time.timeScale = 1.0f;
	}

	public void OpenMenu()
    {
        m_isPause = true;
        m_screen.SetActive(true);
		m_waitControllFrame = PauseWaitControllFrame;

        // timeScale... 物理演算等のスピード。deltaTimeの値にも比例している。
        // フレーム単位のdeltaTimeを使わないカウントダウン処理などには反映されない
        Time.timeScale = 0.0f;

		m_audioSource.PlayOneShot(m_seSelect);
	}

	public void OnCloseMenu(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
        {
            return;
        }
		if (m_waitControllFrame > 0)
		{
			return;
		}

		Continue();

		m_audioSource.PlayOneShot(m_seSelect);
	}

	public void OnSelect(InputAction.CallbackContext callbackContext)
	{
		if (!callbackContext.performed)
		{
			return;
		}
		if (m_waitControllFrame > 0)
		{
			return;
		}

		Transform button = m_buttons.transform.GetChild(m_nowCursor);

		if (button.name == "Continue")
		{
			Continue();
		}
		else if (button.name == "Exit")
		{
			if (SceneChanger.Instance.IsFade)
			{
				return;
			}

			SceneChanger.Instance.StartChangeScene("StageSelect", GameManager.SceneType.StageSelect);
			Continue();
		}
		else
		{
			Continue();
		}

		m_audioSource.PlayOneShot(m_seSelect);
	}

	public void OnMove(InputAction.CallbackContext callbackContext)
	{
		m_leftStick = callbackContext.ReadValue<Vector2>();

		m_audioSource.PlayOneShot(m_seMove);
	}
}
