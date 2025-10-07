using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoneyUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public List<TextMeshProUGUI> coinTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> gemTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> expPercentTexts = new List<TextMeshProUGUI>(); // ⭐ 변경: 경험치 퍼센트 텍스트
    public List<TextMeshProUGUI> levelTexts = new List<TextMeshProUGUI>();
    [Header("Experience Bar")]
    public List<Image> expBars = new List<Image>(); // 경험치 바 (Fill 이미지들)
    void Start()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        // 코인 텍스트들 업데이트
        foreach (TextMeshProUGUI coinText in coinTexts)
        {
            if (coinText != null)
                coinText.text = MoneyManager.Instance.coins.ToString();
        }
        // 보석 텍스트들 업데이트
        foreach (TextMeshProUGUI gemText in gemTexts)
        {
            if (gemText != null)
                gemText.text = MoneyManager.Instance.gems.ToString();
        }

        // 레벨 텍스트들 업데이트
        foreach (TextMeshProUGUI levelText in levelTexts)
        {
            if (levelText != null)
                levelText.text = MoneyManager.Instance.level.ToString() + "Lv";
        }

        // 경험치 바들 업데이트
        float expPercent = MoneyManager.Instance.GetCurrentLevelProgressPercent();
        foreach (Image expBar in expBars)
        {
            if (expBar != null)
                expBar.fillAmount = expPercent;
        }

        // ⭐ 경험치 퍼센트 텍스트들 업데이트
        foreach (TextMeshProUGUI expPercentText in expPercentTexts)
        {
            if (expPercentText != null)
            {
                int percentValue = Mathf.RoundToInt(expPercent * 100);
                expPercentText.text = percentValue.ToString() + "%";
            }
        }
    }
}