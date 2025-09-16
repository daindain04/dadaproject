using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("Player Resources")]
    public int coins = 0;
    public int gems = 0;
    public int totalExperience = 0; // �� ����ġ
    public int level = 1;

    [Header("Save Settings")]
    public string saveFileName = "PlayerData";

    // ����ġ ���� �̺�Ʈ
    public static System.Action OnLevelUp;
    public static System.Action OnExperienceChanged;

    void Awake()
    {
        // �̱��� �������� �� ��ȯ �ÿ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData(); // ���� ���� �� ������ �ε�
            Debug.Log("MoneyManager ������ - ������ �ε� �Ϸ�");
        }
        else
        {
            Debug.Log("MoneyManager �ߺ� ���� ���� - ���� ������Ʈ ����");
            Destroy(gameObject);
        }
    }

    #region ���� �ý���
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

    #region ���� �ý���
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

    #region ����ġ �ý���
    /// <summary>
    /// ����ġ ȹ��
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
    /// Ư�� �������� ���� ������ ���µ� �ʿ��� ����ġ
    /// </summary>
    public int GetRequiredExpForLevel(int fromLevel)
    {
        return fromLevel * 10; // 1->2: 10, 2->3: 20, 3->4: 30...
    }

    /// <summary>
    /// Ư�� �������� �ʿ��� �� ����ġ (����)
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
    /// ���� �������� ���� �������� �ʿ��� ����ġ
    /// </summary>
    public int GetExpRequiredForNextLevel()
    {
        return GetRequiredExpForLevel(level);
    }

    /// <summary>
    /// ���� �������� �󸶳� ����ġ�� ä������ (0~�ʿ����ġ)
    /// </summary>
    public int GetCurrentLevelProgress()
    {
        int currentLevelStartExp = GetTotalExpForLevel(level);
        return totalExperience - currentLevelStartExp;
    }

    /// <summary>
    /// ���� ���� ���൵ �ۼ�Ʈ (0.0 ~ 1.0)
    /// </summary>
    public float GetCurrentLevelProgressPercent()
    {
        int currentProgress = GetCurrentLevelProgress();
        int requiredExp = GetExpRequiredForNextLevel();
        return (float)currentProgress / requiredExp;
    }

    /// <summary>
    /// ������ üũ
    /// </summary>
    private void CheckLevelUp()
    {
        while (totalExperience >= GetTotalExpForLevel(level + 1))
        {
            level++;
            OnLevelUp?.Invoke();
            Debug.Log($"������! ���� ����: {level}");
        }
    }
    #endregion

    #region ������ ����/�ε�
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

        // �ε� �� ���� ���� (������ ���Ἲ Ȯ��)
        RecalculateLevel();
        UpdateAllUI();
    }

    /// <summary>
    /// �� ����ġ�� ������� ���� ����
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

    #region UI ������Ʈ
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

    #region ����� ���
    [System.Serializable]
    public class DebugPanel
    {
        [Header("����� - ��ȭ �߰�")]
        public int debugCoinsToAdd = 100;
        public int debugGemsToAdd = 10;
        public int debugExpToAdd = 50;
    }

    public DebugPanel debug;

    [ContextMenu("�����: ���� �߰�")]
    public void DebugAddCoins()
    {
        AddCoins(debug.debugCoinsToAdd);
    }

    [ContextMenu("�����: ���� �߰�")]
    public void DebugAddGems()
    {
        AddGems(debug.debugGemsToAdd);
    }

    [ContextMenu("�����: ����ġ �߰�")]
    public void DebugAddExperience()
    {
        AddExperience(debug.debugExpToAdd);
    }

    [ContextMenu("�����: ������ ����")]
    public void DebugResetData()
    {
        ResetData();
    }
    #endregion
}