using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject m_screen;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {
        m_isPause = true;
        m_screen.SetActive(true);

        // timeScale... 物理演算等のスピード。deltaTimeの値にも比例している。
        // フレーム単位のdeltaTimeを使わないカウントダウン処理などには反映されない
        Time.timeScale = 0.0f;
    }

    public void OnCloseMenu(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed)
        {
            return;
        }

        m_isPause = false;
        m_screen.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
