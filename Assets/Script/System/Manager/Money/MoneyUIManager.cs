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
    public List<TextMeshProUGUI> totalExpTexts = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> levelTexts = new List<TextMeshProUGUI>();

    [Header("Experience Bar")]
    public List<Image> expBars = new List<Image>(); // ����ġ �� (Fill �̹�����)

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // ���� �ؽ�Ʈ�� ������Ʈ
        foreach (TextMeshProUGUI coinText in coinTexts)
        {
            if (coinText != null)
                coinText.text = MoneyManager.Instance.coins.ToString();
        }

        // ���� �ؽ�Ʈ�� ������Ʈ
        foreach (TextMeshProUGUI gemText in gemTexts)
        {
            if (gemText != null)
                gemText.text = MoneyManager.Instance.gems.ToString();
        }

        // �� ����ġ �ؽ�Ʈ�� ������Ʈ
        foreach (TextMeshProUGUI totalExpText in totalExpTexts)
        {
            if (totalExpText != null)
                totalExpText.text = MoneyManager.Instance.totalExperience.ToString();
        }

        // ���� �ؽ�Ʈ�� ������Ʈ
        foreach (TextMeshProUGUI levelText in levelTexts)
        {
            if (levelText != null)
                levelText.text = "Level " + MoneyManager.Instance.level.ToString();
        }

        // ����ġ �ٵ� ������Ʈ
        float expPercent = MoneyManager.Instance.GetCurrentLevelProgressPercent();
        foreach (Image expBar in expBars)
        {
            if (expBar != null)
                expBar.fillAmount = expPercent;
        }
    }
}
