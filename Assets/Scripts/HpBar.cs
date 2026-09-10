using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
	[SerializeField] float m_barLength;

	const float ChangeColorByHp01 = 0.3f;

	RectTransform m_frameBar;
	RectTransform m_mainBar;
	Image m_mainBarImage;

	// Start is called before the first frame update
	void Start()
    {
		m_mainBar = transform.Find("MainBar").GetComponent<RectTransform>();
		m_frameBar = GetComponent<RectTransform>();
        m_frameBar.sizeDelta = new(m_barLength, m_frameBar.sizeDelta.y);

		m_mainBarImage = transform.Find("MainBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
		// é‘Ì—Í‚É‰ž‚¶‚ÄƒQ[ƒW‚Ì’·‚³‚ð•Ï‚¦‚é
		float hp01 = Castle.Instance.GetHp01();
		m_mainBar.sizeDelta = new(m_barLength * hp01, m_mainBar.sizeDelta.y);

		// é‘Ì—Í‚ªˆê’è‚ÌŠ„‡ˆÈ‰º‚É‚È‚Á‚½‚Æ‚«‚ÉF‚ð•Ï‚¦‚é
		if (hp01 > ChangeColorByHp01)
		{
			m_mainBarImage.color = Color.cyan;
		}
		else
		{
			m_mainBarImage.color = Color.red;
		}
	}
}
