using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    Data m_data;

    static SaveData m_instance;
    public static SaveData Instance => m_instance;

    [Serializable]
    public class Data
    {
        public int[] highScores = new int[StageSelect.StageAmount];
    }

    private void Awake()
    {
        if (SceneChanger.Instance != null && GameManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;

        DontDestroyOnLoad(gameObject);

        m_data = new();
    }

    public void Write(int stageNum, int score)
    {
        m_data.highScores[stageNum] = score;
    }

    public int Read(int stageNum)
    {
        return m_data.highScores[stageNum];
    }

    public void SetData(Data data)
    {
        m_data = data;
    }

    public Data GetData()
    {
        return m_data;
    }
}
