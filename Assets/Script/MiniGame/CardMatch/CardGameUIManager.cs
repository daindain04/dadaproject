using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardGameUIManager : MonoBehaviour
{
    [Header("타이머 관련")]
    public Slider timerSlider;
    public float maxTime = 55f;
    private float timeLeft;
    private bool isGameRunning = false;

    [Header("타이머 텍스트")]
    public TextMeshProUGUI timerText;

    [Header("Pause 버튼 관련")]
    public Button pauseButton;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private bool isPaused = false;
    public CardGameController cardController;

    [Header("홈 버튼 관련")]
    public Button homeButton;
    public GameObject noticePanel;
    public Button yesButton;
    public Button noButton;
    public GameObject inGame;

    [Header("게임 시작 버튼")]
    public Button gameButton;

    [Header("게임 오버 패널")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button exitButton;

    [Header("성공 패널 관련")]
    public GameObject successPanel;
    public Button HomeButton;

    void Start()
    {

        Debug.Log("UI Manager Start 호출됨");

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

        Debug.Log($"Update 실행 중: isGameRunning={isGameRunning}, isPaused={isPaused}");
        if (isGameRunning && !isPaused)
        { 
            if (isGameRunning && !isPaused)
            {
                timeLeft -= Time.deltaTime;
                timerSlider.value = timeLeft;
                timerText.text = Mathf.Ceil(timeLeft).ToString("F0");
            }

            if (timeLeft <= 10)
                timerText.color = new Color32(227, 28, 21, 255); // 빨간색
            else
                timerText.color = new Color32(223, 113, 29, 255); // 주황색

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
        Debug.Log("시간 초과! 게임 종료");
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
        Debug.Log("게임 재시작!");
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
        Debug.Log("새로운 게임 시작!");

        inGame.SetActive(true);

        // 카드 버튼이 생성되었는지 확인하고 초기화 실행
        cardController.Initialize(); //  여기서 명시적으로 초기화

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
