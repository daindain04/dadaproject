using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    public static ExpManager instance;

    [Header("UI")]
    public Image expFillImage;             // FillAmount로 제어할 이미지
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI[] expTexts;

    private int playerExp = 0;
    private int playerLevel = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadExp();
        UpdateExpUI();
    }

    public void AddExp(int amount)
    {
        playerExp += amount;
        SaveExp();
        UpdateExpUI();
    }

    private void UpdateExpUI()
    {
        playerLevel = CalculateLevel(playerExp);

        int expForCurrentLevel = GetTotalExpForLevel(playerLevel - 1);
        int expForNextLevel = GetTotalExpForLevel(playerLevel);

        int currentLevelExp = playerExp - expForCurrentLevel;
        int neededExp = expForNextLevel - expForCurrentLevel;

        float fill = (float)currentLevelExp / neededExp;

        if (expFillImage != null)
            expFillImage.fillAmount = fill; //  핵심: Fill Amount 설정

        foreach (TextMeshProUGUI expText in expTexts)
        {
            if (expText != null)
                expText.text = $"EXP: {playerExp}";
        }

        if (levelText != null)
            levelText.text = $"Lv. {playerLevel}";
    }

    private int CalculateLevel(int totalExp)
    {
        int level = 1;
        while (totalExp >= GetTotalExpForLevel(level))
        {
            level++;
        }
        return level;
    }

    private int GetTotalExpForLevel(int level)
    {
        return (level * (level + 1) / 2) * 10;
    }

    private void SaveExp()
    {
        PlayerPrefs.SetInt("PlayerExp", playerExp);
        PlayerPrefs.Save();
    }

    private void LoadExp()
    {
        playerExp = PlayerPrefs.GetInt("PlayerExp", 0);
    }
}
