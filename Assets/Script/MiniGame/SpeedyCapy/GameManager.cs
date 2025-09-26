using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum GameDifficulty { Easy, Normal, Hard }

public class GameManager : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    [Header("UI")]
    [SerializeField] private TMP_Text timeText;             // 남은 시간 표시용
    [SerializeField] private Slider progressSlider;         // 시간 진행 슬라이더

    [Header("게임 설정")]
    [SerializeField] private float successTime = 30f;       // 버텨야 할 시간
    [SerializeField] private GameDifficulty difficulty = GameDifficulty.Easy; // 난이도 설정

    public static GameManager instance = null;
    public float moveSpeed = 5f;
    private int obstacleCount = 0;
    public bool isGameOver = false;
    private float remainingTime;
    private bool isGameRunning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        remainingTime = successTime;
        isGameRunning = true;

        // 슬라이더 초기화
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }
    }

    private void Update()
    {
        if (isGameRunning && !isGameOver)
        {
            remainingTime -= Time.deltaTime;

            // 남은 시간 텍스트 업데이트
            if (timeText != null)
            {
                timeText.text = Mathf.CeilToInt(remainingTime).ToString();
            }

            // 슬라이더 값 업데이트 (시간 진행률)
            if (progressSlider != null)
            {
                float progress = 1f - (remainingTime / successTime);
                progressSlider.value = Mathf.Clamp01(progress);
            }

            // 성공 조건 달성
            if (remainingTime <= 0f)
            {
                SetGameWin();
            }
        }
    }

    public void IncreaseSpeed()
    {
        moveSpeed += 2f;
    }

    public void AddObstacleCount()
    {
        obstacleCount += 1;
        if (obstacleCount % 10 == 0)
        {
            IncreaseSpeed();
            ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
            if (spawner != null)
            {
                spawner.DecreaseObstacleInterval();
            }
        }
    }

    public void SetGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            isGameRunning = false;

            ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }

            StartCoroutine(ShowGameOverPanel());
        }
    }

    public void SetGameWin()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            isGameRunning = false;

            ObstacleSpawner spawner = FindObjectOfType<ObstacleSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }

            // 난이도별 보상 지급
            GiveReward();

            if (gameWinPanel != null)
            {
                gameWinPanel.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 난이도별 보상 지급
    /// </summary>
    private void GiveReward()
    {
        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("MoneyManager 인스턴스를 찾을 수 없습니다!");
            return;
        }

        switch (difficulty)
        {
            case GameDifficulty.Easy:
                MoneyManager.Instance.AddCoins(100);
                MoneyManager.Instance.AddGems(1);
                MoneyManager.Instance.AddExperience(10);
                Debug.Log("Easy 난이도 클리어! 코인 100, 보석 1, 경험치 10 획득");
                break;

            case GameDifficulty.Normal:
                MoneyManager.Instance.AddCoins(150);
                MoneyManager.Instance.AddGems(2);
                MoneyManager.Instance.AddExperience(20);
                Debug.Log("Normal 난이도 클리어! 코인 150, 보석 2, 경험치 20 획득");
                break;

            case GameDifficulty.Hard:
                MoneyManager.Instance.AddCoins(300);
                MoneyManager.Instance.AddGems(3);
                MoneyManager.Instance.AddExperience(30);
                Debug.Log("Hard 난이도 클리어! 코인 300, 보석 3, 경험치 30 획득");
                break;
        }
    }

    /// <summary>
    /// 게임오버 패널에서 재시작 버튼 클릭 시 호출
    /// </summary>
    public void OnGameOverRestartPressed()
    {
        // 현재 씬을 다시 로드하여 게임 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 게임오버 패널에서 나가기 버튼 클릭 시 호출
    /// </summary>
    public void OnGameOverExitPressed()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 게임승리 패널에서 확인 버튼 클릭 시 호출
    /// </summary>
    public void OnGameWinConfirmPressed()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(1f);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 게임 상태 리셋 (필요시 사용)
    /// </summary>
    public void ResetGame()
    {
        remainingTime = successTime;
        isGameRunning = true;
        isGameOver = false;
        obstacleCount = 0;
        moveSpeed = 5f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (gameWinPanel != null)
            gameWinPanel.SetActive(false);
    }
}