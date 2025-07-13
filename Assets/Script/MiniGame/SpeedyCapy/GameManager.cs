using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    [Header("게임 설정")]
    [SerializeField] private float successTime = 30f; //  Inspector에서 조절
    [SerializeField] private TMP_Text timeText;  //  남은 시간 표시용

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
    }

    private void Update()
    {
        if (isGameRunning && !isGameOver)
        {
            remainingTime -= Time.deltaTime;

            // 남은 시간 텍스트 업데이트
            if (timeText != null)
            {
                timeText.text =  Mathf.CeilToInt(remainingTime).ToString() ;
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
