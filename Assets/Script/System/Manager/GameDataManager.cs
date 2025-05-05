using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class GameData
{
    public string capybaraName;
    public int gold;
    public int gems;
    public float hunger;    // ����� (0~100)
    public float boredom;   // ������ (0~100)
    public float happiness; // �ູ�� (0~100)
    public string lastPlayTime;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    private string savePath;

    public GameData data = new GameData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        savePath = Application.persistentDataPath + "/saveData.json";
    }

    private void Start()
    {
        LoadGameData();
        ApplyOfflineEffects();  // ���� �� ���� ���� �ݿ�
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }

    public void SaveGameData()
    {
        data.lastPlayTime = DateTime.Now.ToString();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("���� ������ ���� �Ϸ�!");
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("���� ������ �ε� �Ϸ�!");
        }
        else
        {
            Debug.Log("����� �����Ͱ� ���� ���� �����մϴ�.");
            data.capybaraName = "NoName";
            data.gold = 0;
            data.gems = 0;
            data.hunger = 100;
            data.boredom = 100;
            data.happiness = 100;
        }
    }

    private void ApplyOfflineEffects()
    {
        if (!string.IsNullOrEmpty(data.lastPlayTime))
        {
            DateTime lastPlay = DateTime.Parse(data.lastPlayTime);
            TimeSpan offlineTime = DateTime.Now - lastPlay;
            float offlineHours = (float)offlineTime.TotalHours;

            // ����� ����
            data.hunger -= offlineHours * 5;
            if (data.hunger < 20) data.hunger = 20;

            // ������ ����
            data.boredom -= offlineHours * 3;
            if (data.boredom < 30) data.boredom = 30;

            // �ູ�� ���� (����� + ������ ���ҷ� / 2)
            float happinessReduction = ((offlineHours * 5) + (offlineHours * 3)) / 2;
            data.happiness -= happinessReduction;
            if (data.happiness < 25) data.happiness = 25;

            Debug.Log($"�������� ���� ����: {offlineHours}�ð� ���� ����� {offlineHours * 5} ����, ������ {offlineHours * 3} ����, �ູ�� {happinessReduction} ����");
        }
    }
}
