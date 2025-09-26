using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    [Header("UI 연결 (한 번만 드래그)")]
    public Slider timerSlider;
    public TMP_Text timerText;
    public GameObject stopButton;
    public GameObject restartButton;
    public GameObject homeButton;
    public GameObject homeConfirmPanel;
    public GameObject rewardPanel;
    public GameObject failPanel;

    void Awake()
    {
        restartButton.SetActive(false);
        rewardPanel.SetActive(false);
        failPanel.SetActive(false);
        homeConfirmPanel.SetActive(false);
    }

    public void UpdateTimer(float remaining, float maxTime)
    {
        timerSlider.maxValue = maxTime;
        timerSlider.value = remaining;
        timerText.text = Mathf.CeilToInt(remaining).ToString();
    }

    public void ShowPaused()
    {
        stopButton.SetActive(false);
        restartButton.SetActive(true);
    }

    public void ShowRunning()
    {
        stopButton.SetActive(true);
        restartButton.SetActive(false);
    }

    public void ToggleHomeConfirm(bool show)
        => homeConfirmPanel.SetActive(show);

    public void ShowReward() => rewardPanel.SetActive(true);

    public void ShowFail() => failPanel.SetActive(true);

    /// <summary>
    /// 실패 패널을 숨기는 함수 (재시작 시 사용)
    /// </summary>
    public void HideFail() => failPanel.SetActive(false);
}