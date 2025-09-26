using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("Player Resources")]
    public int coins = 0;
    public int gems = 0;
    public int totalExperience = 0; // 총 경험치
    public int level = 1;

    [Header("Save Settings")]
    public string saveFileName = "PlayerData";

    // 경험치 관련 이벤트
    public static System.Action OnLevelUp;
    public static System.Action OnExperienceChanged;

    void Awake()
    {
        // 싱글톤 패턴으로 씬 전환 시에도 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData(); // 게임 시작 시 데이터 로드
            Debug.Log("MoneyManager 생성됨 - 데이터 로드 완료");
        }
        else
        {
            Debug.Log("MoneyManager 중복 생성 방지 - 기존 오브젝트 유지");
            Destroy(gameObject);
        }
    }

    #region 코인 시스템
    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            coins += amount;
            UpdateAllUI();
            SaveData();
        }
    }

    public bool SpendCoins(int amount)
    {
        if (amount > 0 && coins >= amount)
        {
            coins -= amount;
            UpdateAllUI();
            SaveData();
            return true;
        }
        return false;
    }

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }
    #endregion

    #region 보석 시스템
    public void AddGems(int amount)
    {
        if (amount > 0)
        {
            gems += amount;
            UpdateAllUI();
            SaveData();
        }
    }

    public bool SpendGems(int amount)
    {
        if (amount > 0 && gems >= amount)
        {
            gems -= amount;
            UpdateAllUI();
            SaveData();
            return true;
        }
        return false;
    }

    public bool HasEnoughGems(int amount)
    {
        return gems >= amount;
    }
    #endregion

    #region 경험치 시스템
    /// <summary>
    /// 경험치 획득
    /// </summary>
    public void AddExperience(int amount)
    {
        if (amount > 0)
        {
            totalExperience += amount;
            CheckLevelUp();
            OnExperienceChanged?.Invoke();
            UpdateAllUI();
            SaveData();
        }
    }

    /// <summary>
    /// 특정 레벨에서 다음 레벨로 가는데 필요한 경험치
    /// </summary>
    public int GetRequiredExpForLevel(int fromLevel)
    {
        return fromLevel * 10; // 1->2: 10, 2->3: 20, 3->4: 30...
    }

    /// <summary>
    /// 특정 레벨까지 필요한 총 경험치 (누적)
    /// </summary>
    public int GetTotalExpForLevel(int targetLevel)
    {
        int total = 0;
        for (int i = 1; i < targetLevel; i++)
        {
            total += GetRequiredExpForLevel(i);
        }
        return total;
    }

    /// <summary>
    /// 현재 레벨에서 다음 레벨까지 필요한 경험치
    /// </summary>
    public int GetExpRequiredForNextLevel()
    {
        return GetRequiredExpForLevel(level);
    }

    /// <summary>
    /// 현재 레벨에서 얼마나 경험치를 채웠는지 (0~필요경험치)
    /// </summary>
    public int GetCurrentLevelProgress()
    {
        int currentLevelStartExp = GetTotalExpForLevel(level);
        return totalExperience - currentLevelStartExp;
    }

    /// <summary>
    /// 현재 레벨 진행도 퍼센트 (0.0 ~ 1.0)
    /// </summary>
    public float GetCurrentLevelProgressPercent()
    {
        int currentProgress = GetCurrentLevelProgress();
        int requiredExp = GetExpRequiredForNextLevel();
        return (float)currentProgress / requiredExp;
    }

    /// <summary>
    /// 레벨업 체크
    /// </summary>
    private void CheckLevelUp()
    {
        while (totalExperience >= GetTotalExpForLevel(level + 1))
        {
            level++;
            OnLevelUp?.Invoke();
            Debug.Log($"레벨업! 현재 레벨: {level}");
        }
    }
    #endregion

    #region 데이터 저장/로드
    public void SaveData()
    {
        PlayerPrefs.SetInt(saveFileName + "_Coins", coins);
        PlayerPrefs.SetInt(saveFileName + "_Gems", gems);
        PlayerPrefs.SetInt(saveFileName + "_TotalExperience", totalExperience);
        PlayerPrefs.SetInt(saveFileName + "_Level", level);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        coins = PlayerPrefs.GetInt(saveFileName + "_Coins", 0);
        gems = PlayerPrefs.GetInt(saveFileName + "_Gems", 0);
        totalExperience = PlayerPrefs.GetInt(saveFileName + "_TotalExperience", 0);
        level = PlayerPrefs.GetInt(saveFileName + "_Level", 1);

        // 로드 후 레벨 재계산 (데이터 무결성 확보)
        RecalculateLevel();
        UpdateAllUI();
    }

    /// <summary>
    /// 총 경험치를 기반으로 레벨 재계산
    /// </summary>
    private void RecalculateLevel()
    {
        level = 1;
        while (totalExperience >= GetTotalExpForLevel(level + 1))
        {
            level++;
        }
    }

    public void ResetData()
    {
        coins = 0;
        gems = 0;
        totalExperience = 0;
        level = 1;
        SaveData();
        UpdateAllUI();
    }
    #endregion

    #region UI 업데이트
    private void UpdateAllUI()
    {
        MoneyUIManager[] uiManagers = FindObjectsOfType<MoneyUIManager>();
        foreach (MoneyUIManager uiManager in uiManagers)
        {
            if (uiManager != null)
                uiManager.UpdateUI();
        }
    }
    #endregion

    #region 디버그 기능
    [System.Serializable]
    public class DebugPanel
    {
        [Header("디버그 - 재화 추가")]
        public int debugCoinsToAdd = 100;
        public int debugGemsToAdd = 10;
        public int debugExpToAdd = 50;
    }

    public DebugPanel debug;

    [ContextMenu("디버그: 코인 추가")]
    public void DebugAddCoins()
    {
        AddCoins(debug.debugCoinsToAdd);
    }

    [ContextMenu("디버그: 보석 추가")]
    public void DebugAddGems()
    {
        AddGems(debug.debugGemsToAdd);
    }

    [ContextMenu("디버그: 경험치 추가")]
    public void DebugAddExperience()
    {
        AddExperience(debug.debugExpToAdd);
    }

    [ContextMenu("디버그: 데이터 리셋")]
    public void DebugResetData()
    {
        ResetData();
    }
    #endregion
}