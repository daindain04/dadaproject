using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("�г�")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    [Header("���� ����")]
    [SerializeField] private float successTime = 30f; //  Inspector���� ����
    [SerializeField] private TMP_Text timeText;  //  ���� �ð� ǥ�ÿ�

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

            // ���� �ð� �ؽ�Ʈ ������Ʈ
            if (timeText != null)
            {
                timeText.text =  Mathf.CeilToInt(remainingTime).ToString() ;
            }

            // ���� ���� �޼�
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
