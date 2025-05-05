using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardGameUIManager : MonoBehaviour
{
    [Header("Ÿ�̸� ����")]
    public Slider timerSlider;
    public float maxTime = 55f;
    private float timeLeft;
    private bool isGameRunning = false;

    [Header("Ÿ�̸� �ؽ�Ʈ")]
    public TextMeshProUGUI timerText;

    [Header("Pause ��ư ����")]
    public Button pauseButton;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private bool isPaused = false;
    public CardGameController cardController;

    [Header("Ȩ ��ư ����")]
    public Button homeButton;
    public GameObject noticePanel;
    public Button yesButton;
    public Button noButton;
    public GameObject inGame;

    [Header("���� ���� ��ư")]
    public Button gameButton;

    [Header("���� ���� �г�")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button exitButton;

    [Header("���� �г� ����")]
    public GameObject successPanel;
    public Button HomeButton;

    void Start()
    {

        Debug.Log("UI Manager Start ȣ���");

        noticePanel.SetActive(false);
        inGame.SetActive(true);
        isGameRunning = false;
        gameOverPanel.SetActive(false);
        successPanel.SetActive(false);

        pauseButton.onClick.AddListener(TogglePause);
        homeButton.onClick.AddListener(ShowNoticePanel);
        yesButton.onClick.AddListener(GoToMainMenu);
        noButton.onClick.AddListener(CloseNoticePanel);
        gameButton.onClick.AddListener(StartNewGame);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(GoToMainMenu);
        HomeButton.onClick.AddListener(ReturnToMainMenu);

        timerText.text = maxTime.ToString("F0");
    }

    void Update()
    {

        Debug.Log($"Update ���� ��: isGameRunning={isGameRunning}, isPaused={isPaused}");
        if (isGameRunning && !isPaused)
        { 
            if (isGameRunning && !isPaused)
            {
                timeLeft -= Time.deltaTime;
                timerSlider.value = timeLeft;
                timerText.text = Mathf.Ceil(timeLeft).ToString("F0");
            }

            if (timeLeft <= 10)
                timerText.color = new Color32(227, 28, 21, 255); // ������
            else
                timerText.color = new Color32(223, 113, 29, 255); // ��Ȳ��

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                isGameRunning = false;
                GameOver();
            }
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused) PauseGame();
        else ResumeGame();
    }

    void PauseGame()
    {
        isPaused = true;
        isGameRunning = false;
        foreach (Button btn in cardController.btns)
            btn.interactable = false;
        pauseButton.image.sprite = playIcon;
    }

    void ResumeGame()
    {
        isPaused = false;
        isGameRunning = true;
        foreach (Button btn in cardController.btns)
            btn.interactable = true;
        pauseButton.image.sprite = pauseIcon;
    }

    void GameOver()
    {
        Debug.Log("�ð� �ʰ�! ���� ����");
        isGameRunning = false;
        foreach (Button btn in cardController.btns)
            btn.interactable = false;
        gameOverPanel.SetActive(true);
    }

    void ShowNoticePanel()
    {
        isPaused = true;
        isGameRunning = false;
        noticePanel.SetActive(true);
        foreach (Button btn in cardController.btns)
            btn.interactable = false;
    }

    void RestartGame()
    {
        Debug.Log("���� �����!");
        gameOverPanel.SetActive(false);
        ResetCards();
        StartNewGame();
    }

    void ResetCards()
    {
        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage;
            btn.image.color = new Color(1, 1, 1, 1);
            btn.interactable = true;
        }
    }

    void GoToMainMenu()
    {
        inGame.SetActive(false);
        ResetGameState();
    }

    void CloseNoticePanel()
    {
        noticePanel.SetActive(false);
        isPaused = false;
        isGameRunning = true;
        foreach (Button btn in cardController.btns)
            btn.interactable = true;
    }

    public void StartNewGame()
    {
        Debug.Log("���ο� ���� ����!");

        inGame.SetActive(true);

        // ī�� ��ư�� �����Ǿ����� Ȯ���ϰ� �ʱ�ȭ ����
        cardController.Initialize(); //  ���⼭ ��������� �ʱ�ȭ

        noticePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        timeLeft = maxTime;
        timerSlider.value = maxTime;
        isGameRunning = true;
        isPaused = false;
        timerText.text = maxTime.ToString("F0");

        ResetCards();
        ShuffleCards();
    }

    void ShuffleCards()
    {
        List<Sprite> shuffledCards = new List<Sprite>(cardController.gamePuzzles);
        for (int i = 0; i < shuffledCards.Count; i++)
        {
            Sprite temp = shuffledCards[i];
            int randomIndex = Random.Range(i, shuffledCards.Count);
            shuffledCards[i] = shuffledCards[randomIndex];
            shuffledCards[randomIndex] = temp;
        }
        cardController.gamePuzzles = shuffledCards;
    }

    public void StopTimer()
    {
        isGameRunning = false;
    }

    public void ResetGameState()
    {
        cardController.firstGuess = false;
        cardController.secondGuess = false;
        cardController.countGuesses = 0;
        cardController.countCorrectGuesses = 0;

        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage;
            btn.image.color = new Color(1, 1, 1, 1);
            btn.interactable = true;
        }

        cardController.gamePuzzles.Clear();
        cardController.AddGamePuzzles();
        ShuffleCards();
    }

    void ReturnToMainMenu()
    {
        successPanel.SetActive(false);
        inGame.SetActive(false);
        ResetGameState();
    }
}
