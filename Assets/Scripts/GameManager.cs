using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] SceneType m_sceneType;

	static GameManager m_instance;

	public SceneType ThisSceneType
	{
		get { return m_sceneType; }
		set { m_sceneType = value; }
	}
	public static GameManager Instance => m_instance;

	public enum SceneType
	{
		Title,
		StageSelect,
		Game,
		Result
	}
	private void Awake()
	{
		if (m_instance != null && m_instance != this)
		{
			Destroy(gameObject);
			return;
		}

		m_instance = this;

		DontDestroyOnLoad(gameObject);

		Application.targetFrameRate = 120;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
