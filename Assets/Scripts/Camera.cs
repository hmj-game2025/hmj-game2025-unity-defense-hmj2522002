using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
	[SerializeField] float m_minRange;
	[SerializeField] float m_maxRange;
	[SerializeField] float m_changeRangeSpeed;
	[SerializeField] float m_zoomSmoothness;

	CinemachineVirtualCamera m_camera;
	float m_nowRange;
	float m_targetRange;

	static Camera m_instance;
	public static Camera Instance => m_instance;

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}

		m_camera = GetComponent<CinemachineVirtualCamera>();
	}

	void Start()
	{
		m_nowRange = m_minRange;
		m_targetRange = m_nowRange;
		m_camera.m_Lens.FieldOfView = m_nowRange;
	}

    // Update is called once per frame
    void Update()
    {
        m_nowRange += (m_targetRange - m_nowRange) / m_changeRangeSpeed;
		m_camera.m_Lens.FieldOfView = m_nowRange;
    }

	public void ZoomIn()
	{
		m_targetRange -= m_changeRangeSpeed * Time.deltaTime;

		if (m_targetRange < m_minRange)
		{
			m_targetRange = m_minRange;
		}
	}

	public void ZoomOut()
	{
		m_targetRange += m_changeRangeSpeed * Time.deltaTime;

		if (m_targetRange > m_maxRange)
		{
			m_targetRange = m_maxRange;
		}
	}
}
