using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("�г�")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    [Header("UI")]
    [SerializeField] private TMP_Text timeText;             // ���� �ð� ǥ�ÿ�
    [SerializeField] private Slider progressSlider;         // �ð� ���� �����̴�

    [Header("���� ����")]
    [SerializeField] private float successTime = 30f;       // ���߾� �� �ð�

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

        // �����̴� �ʱ�ȭ
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

            // ���� �ð� �ؽ�Ʈ ������Ʈ
            if (timeText != null)
            {
                timeText.text = Mathf.CeilToInt(remainingTime).ToString();
            }

            // �����̴� �� ������Ʈ (�ð� �����)
            if (progressSlider != null)
            {
                float progress = 1f - (remainingTime / successTime);
                progressSlider.value = Mathf.Clamp01(progress);
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
