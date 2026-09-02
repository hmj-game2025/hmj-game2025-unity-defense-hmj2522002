using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
	[SerializeField] float m_barLength;

	RectTransform m_frameBar;
	RectTransform m_mainBar;

	private void Awake()
	{
		m_mainBar = transform.Find("MainBar").GetComponent<RectTransform>();
		m_frameBar = GetComponent<RectTransform>();
	}

	// Start is called before the first frame update
	void Start()
    {
        m_frameBar.sizeDelta = new(m_barLength, m_frameBar.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {

		float hp01 = Castle.Instance.GetHp01();
		m_mainBar.sizeDelta = new(m_barLength * hp01, m_mainBar.sizeDelta.y);
    }
}
