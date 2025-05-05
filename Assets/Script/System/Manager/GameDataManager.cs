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
    public float hunger;    // 배고픔 (0~100)
    public float boredom;   // 지루함 (0~100)
    public float happiness; // 행복도 (0~100)
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
        ApplyOfflineEffects();  // 접속 시 상태 감소 반영
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
        Debug.Log("게임 데이터 저장 완료!");
    }

    public void LoadGameData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("게임 데이터 로드 완료!");
        }
        else
        {
            Debug.Log("저장된 데이터가 없어 새로 생성합니다.");
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

            // 배고픔 감소
            data.hunger -= offlineHours * 5;
            if (data.hunger < 20) data.hunger = 20;

            // 지루함 감소
            data.boredom -= offlineHours * 3;
            if (data.boredom < 30) data.boredom = 30;

            // 행복도 감소 (배고픔 + 지루함 감소량 / 2)
            float happinessReduction = ((offlineHours * 5) + (offlineHours * 3)) / 2;
            data.happiness -= happinessReduction;
            if (data.happiness < 25) data.happiness = 25;

            Debug.Log($"오프라인 보상 적용: {offlineHours}시간 동안 배고픔 {offlineHours * 5} 감소, 지루함 {offlineHours * 3} 감소, 행복도 {happinessReduction} 감소");
        }
    }
}
