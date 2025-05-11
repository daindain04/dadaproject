using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    [Header("UI 패널")]
    public GameObject gameChoosePanel;
    public GameObject inGamePanel;
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("UI 요소")]
    public Image backgroundImage;
    public Image timerFillImage;
    public Slider timerSlider;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI rewardText;

    public void ApplyDifficultyStyle(GameSettings setting)
    {
        Debug.Log("ApplyDifficultyStyle called!"); // 이거 넣어보세요
        backgroundImage.sprite = setting.backgroundImage;
        timerFillImage.color = setting.uiAccentColor;

        timerSlider.maxValue = setting.timeLimit;
        timerSlider.value = setting.timeLimit;
    }

    public void ShowInGamePanel()
    {
        gameChoosePanel.SetActive(false);
        successPanel.SetActive(false);
        failPanel.SetActive(false);
        inGamePanel.SetActive(true);
    }

    public void UpdateTimer(float time)
    {
        timerSlider.value = time;
        timerText.text = Mathf.Ceil(time).ToString("F0");
    }

    public void ShowSuccessPanel(int coins)
    {
        inGamePanel.SetActive(false);
        successPanel.SetActive(true);
        rewardText.text = $"{coins} 코인을 획득했습니다!";
    }

    public void ShowFailPanel()
    {
        inGamePanel.SetActive(false);
        failPanel.SetActive(true);
    }
}
