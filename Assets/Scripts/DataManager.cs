using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    SaveData.Data m_data;
    string m_filePath;
    string m_fileName = "HighScore.json";

    static DataManager m_instance;
    public static DataManager Instance => m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }

    private void Start()
    {

        m_data = SaveData.Instance.GetData();

        // パス名を取得
        m_filePath = Application.dataPath + "/" + m_fileName;

        Debug.Log($"Awake m_filePath = [{m_filePath}]");

        // ファイルがない（初プレイ）なら、ファイルを作成する
        if (!File.Exists(m_filePath))
        {
            Save(m_data);
        }

        // ファイルを読み込む
        m_data = Load(m_filePath);
        
        SaveData.Instance.SetData(m_data);
    }

    void Save(SaveData.Data data)
    {
        string json = JsonUtility.ToJson(data);
        //StreamWriter writer = new StreamWriter(m_filePath, false);
        //writer.WriteLine(json);
        //writer.Close();

        Debug.Log("Save JSON : [" + json + "]");

        using (StreamWriter writer = new StreamWriter(m_filePath, false))
        {
            writer.WriteLine(json);
        }
    }

    SaveData.Data Load(string path)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<SaveData.Data>(json);
    }

    //public void Write(int stageNum, int score)
    //{
    //    m_data.highScores[stageNum] = score;
    //}

    //public int Read(int stageNum)
    //{
    //    return m_data.highScores[stageNum];
    //}


    public void SaveToFile()
    {
        Debug.Log($"m_filePath = [{m_filePath}]");
        Debug.Log($"m_filePath == null : {m_filePath == null}");
        Debug.Log($"m_filePath.Length : {(m_filePath == null ? -1 : m_filePath.Length)}");

        m_data = SaveData.Instance.GetData();

        Save(m_data);
    }
}
