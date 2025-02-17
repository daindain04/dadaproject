using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("타이머 관련")]
    public Slider timerSlider;  // 타이머 슬라이더 (UI)
    public float maxTime = 30f; // 제한 시간 (초)
    private float timeLeft;      // 남은 시간
    private bool isGameRunning = false; // 게임 진행 상태

    [Header(" Pause 버튼 관련")]
    public Button pauseButton;   // UI의 Pause 버튼
    public Sprite playIcon;      // 재생 아이콘 (Play)
    public Sprite pauseIcon;     // 일시 정지 아이콘 (Pause)

    private bool isPaused = false; // 현재 게임이 멈췄는지 여부
    private CardGameController cardController; // 카드 컨트롤러 참조

    [Header(" 홈 버튼 관련")]
    public Button homeButton;    // 홈 버튼 (UI)
    public GameObject noticePanel; // Notice Panel (팝업)
    public Button yesButton;     // "예" 버튼
    public Button noButton;      // "아니요" 버튼
    public GameObject inGame;    // InGame 오브젝트 (비활성화 대상)

    [Header(" 게임 시작 버튼")]
    public Button gameButton;   // 게임 시작 버튼 (GameButton)

    void Start()
    {
        // 초기 설정
        noticePanel.SetActive(false);  // Notice Panel 비활성화
        inGame.SetActive(false);       // InGame 비활성화
        isGameRunning = false;         // 게임 시작 전 상태

        // 카드 컨트롤러 찾기
        cardController = FindObjectOfType<CardGameController>();

        // 버튼 이벤트 추가
        pauseButton.onClick.AddListener(TogglePause);
        homeButton.onClick.AddListener(ShowNoticePanel);
        yesButton.onClick.AddListener(GoToMainMenu);
        noButton.onClick.AddListener(CloseNoticePanel);
        gameButton.onClick.AddListener(StartNewGame);  // 게임 시작 버튼 이벤트 추가
    }

    void Update()
    {
        if (isGameRunning && !isPaused)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft;

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
        isPaused = !isPaused;  // 상태 변경

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

        // 카드 클릭 비활성화
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }

        // 버튼 아이콘 변경 (Pause → Play)
        pauseButton.image.sprite = playIcon;
    }

    void ResumeGame()
    {
        isPaused = false;
        isGameRunning = true;

        // 카드 클릭 활성화
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
        }

        // 버튼 아이콘 변경 (Play → Pause)
        pauseButton.image.sprite = pauseIcon;
    }

    void GameOver()
    {
        Debug.Log("시간 초과! 게임 종료");
        // 추가 게임 오버 로직 가능 (예: 패널 띄우기, 재시작 버튼 활성화 등)
    }

    // 홈 버튼을 누르면 Notice Panel 표시 + 게임 정지
    void ShowNoticePanel()
    {
        isPaused = true;
        noticePanel.SetActive(true);
        isGameRunning = false;

        // 카드 클릭 비활성화
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }
    }

    // "예" 버튼 클릭 시 InGame 비활성화 → 메인 화면으로 이동
    void GoToMainMenu()
    {
        inGame.SetActive(false);
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

    // 게임 시작 버튼 (GameButton) 클릭 시 새로운 게임 시작
    void StartNewGame()
    {
        Debug.Log("새로운 게임 시작!");

        // InGame 오브젝트 활성화
        inGame.SetActive(true);

        // 타이머 초기화
        timeLeft = maxTime;
        timerSlider.value = maxTime;
        isGameRunning = true;
        isPaused = false;

        // 카드 상태 초기화
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
            btn.image.sprite = cardController.bgImage;  // 카드 뒷면으로 초기화
        }

        // 카드 섞기 (Shuffle 함수 호출)
        ShuffleCards();
    }

    // 카드 섞기 함수
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
}
