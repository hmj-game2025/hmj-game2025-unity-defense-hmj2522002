using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUi : MonoBehaviour
{
	[SerializeField] float m_screenDist;
	Enemy m_enemy;
	Image m_image;
	UnityEngine.Camera m_camera;

	public Enemy FocusEnemy
	{
		get { return m_enemy; }
		set { m_enemy = value; }
	}

	private void Awake()
	{
        
	}

	// Start is called before the first frame update
	void Start()
    {
		m_camera = GameObject.FindWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
		m_image = GetComponent<Image>();
		Debug.Log(m_camera);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_enemy == null)
		{
			Debug.Log("Enemy is null");
			Destroy(gameObject);
			return;
		}

		Vector3 view = m_camera.WorldToViewportPoint(m_enemy.transform.position);

		// ‰æ–Ê“à‚É“G‚ª‰f‚Á‚½‚çÁ‚·
		bool isInside = 
			view.x >= 0.0f &&
			view.x <= 1.0f &&
			view.y >= 0.0f &&
			view.y <= 1.0f &&
			view.z > 0.0f;

		m_image.enabled = !isInside;

		Vector2 direction = new(view.x - 0.5f, view.y - 0.5f);

		view.x = view.x * Screen.width - Screen.width / 2.0f;
		view.y = view.y * Screen.height - Screen.height / 2.0f;
		view.x = Mathf.Clamp(view.x, -Screen.width / 2.0f + m_screenDist, Screen.width / 2.0f - m_screenDist);
		view.y = Mathf.Clamp(view.y, -Screen.height / 2.0f + m_screenDist, Screen.height / 2.0f - m_screenDist);

		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		transform.rotation = Quaternion.Euler(0, 0, angle);

		transform.localPosition = view;
    }
}
