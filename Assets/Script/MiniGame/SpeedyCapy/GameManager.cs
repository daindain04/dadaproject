using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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

            if (gameWinPanel != null)
            {
                gameWinPanel.SetActive(true);
            }
        }
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(1f);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
