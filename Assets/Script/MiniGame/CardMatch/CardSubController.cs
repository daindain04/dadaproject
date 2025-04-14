using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameController : MonoBehaviour
{
    [Header("타이머 관련")]
    public Slider timerSlider;
    public float maxTime = 55f;
    private float timeLeft;
    private bool isGameRunning = false;

    [Header("타이머 텍스트")]
    public TextMeshProUGUI timerText;

    [Header(" Pause 버튼 관련")]
    public Button pauseButton;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private bool isPaused = false;
    private CardGameController cardController;

    [Header(" 홈 버튼 관련")]
    public Button homeButton;
    public GameObject noticePanel;
    public Button yesButton;
    public Button noButton;
    public GameObject inGame;

    [Header(" 게임 시작 버튼")]
    public Button gameButton;

    [Header("게임 오버 패널")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button exitButton;

    [Header("성공 패널 관련")]
    public GameObject successPanel; // 성공 패널
    public Button HomeButton; // 성공 패널 내 버튼
   



    void Start()
    {

        noticePanel.SetActive(false);
        inGame.SetActive(false);
        isGameRunning = false;
        gameOverPanel.SetActive(false);
        successPanel.SetActive(false);


        cardController = FindObjectOfType<CardGameController>();


        pauseButton.onClick.AddListener(TogglePause);
        homeButton.onClick.AddListener(ShowNoticePanel);
        yesButton.onClick.AddListener(GoToMainMenu);
        noButton.onClick.AddListener(CloseNoticePanel);
        gameButton.onClick.AddListener(StartNewGame);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(GoToMainMenu);

        timerText.text = maxTime.ToString("F0");

        HomeButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if (isGameRunning && !isPaused)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft;

            timerText.text = Mathf.Ceil(timeLeft).ToString("F0");


            if (timeLeft <= 10)
            {
                timerText.color = new Color32(227, 28, 21, 255); // #E31C15 
            }
            else
            {
                timerText.color = new Color32(223, 113, 29, 255); // #E45E2B 
            }

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

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        isGameRunning = false;


        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }


        pauseButton.image.sprite = playIcon;
    }

    void ResumeGame()
    {
        isPaused = false;
        isGameRunning = true;


        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
        }


        pauseButton.image.sprite = pauseIcon;
    }

    void GameOver()
    {
        Debug.Log("시간 초과! 게임 종료");


        isGameRunning = false;


        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }


        gameOverPanel.SetActive(true);
    }

    // 홈 버튼을 누르면 Notice Panel 표시 + 게임 정지
    void ShowNoticePanel()
    {
        isPaused = true;
        noticePanel.SetActive(true);
        isGameRunning = false;


        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }
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
        // 카드 리스트 초기화
        cardController.gamePuzzles.Clear();

        // 새로운 카드 리스트 생성
        cardController.AddGamePuzzles();
        cardController.Shuffle(cardController.gamePuzzles);

        // 모든 카드 뒷면으로 설정 & 클릭 가능하도록 설정
        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage; // 카드 뒷면 이미지로 설정
            btn.interactable = true; // 다시 클릭 가능하도록 변경

            // 카드의 투명도를 복구 (완전히 보이게)
            btn.image.color = new Color(1, 1, 1, 1);
        }
    }




    // "예" 버튼 클릭 시 InGame 비활성화 → 메인 화면으로 이동
    void GoToMainMenu()
    {
        inGame.SetActive(false);
        ResetGameState();
    }

    // "아니요" 버튼 클릭 시 Notice Panel 닫고 게임 재개
    void CloseNoticePanel()
    {
        noticePanel.SetActive(false);
        isPaused = false;
        isGameRunning = true;

        // 카드 클릭 다시 활성화
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
        }
    }

    void StartNewGame()
    {
        Debug.Log("새로운 게임 시작!");

        // InGame 오브젝트 활성화
        inGame.SetActive(true);

        //  패널 비활성화 
        noticePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // 게임 상태 완전히 초기화
        ResetGameState();

        // 타이머 초기화
        timeLeft = maxTime;
        timerSlider.value = maxTime;
        isGameRunning = true;
        isPaused = false;

        timerText.text = maxTime.ToString("F0");

        // 카드 상태 초기화
        ResetCards();

        // 카드 섞기
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
        isGameRunning = false; // 게임 진행 상태 중지
    }
    public void ResetGameState()
    {
        // 카드 선택 상태 초기화
        cardController.firstGuess = false;
        cardController.secondGuess = false;
        cardController.countGuesses = 0;
        cardController.countCorrectGuesses = 0;

        // 남아 있는 카드 상태 초기화
        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage; // 뒷면으로 설정
            btn.image.color = new Color(1, 1, 1, 1);  // 투명도 복원
            btn.interactable = true; // 다시 클릭 가능하도록 설정
        }

        // 카드 다시 섞기
        cardController.gamePuzzles.Clear();
        cardController.AddGamePuzzles();

        //  ShuffleCards()가 이제 제대로 호출됨!
        ShuffleCards();
    }

    void ReturnToMainMenu()
    {
        successPanel.SetActive(false); // 성공 패널 비활성화
        inGame.SetActive(false); // 게임 화면 비활성화 (메인 화면으로 전환)

        ResetGameState(); // 게임 상태 초기화
    }

}
