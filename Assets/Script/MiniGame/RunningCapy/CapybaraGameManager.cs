using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CapybaraGameManager : MonoBehaviour
{
    public static CapybaraGameManager instance = null;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private TextMeshProUGUI scoreText;

    public enum Difficulty { Easy, Normal, Hard }
    [Header("Difficulty")]
    public Difficulty currentDifficulty = Difficulty.Easy;
    private int targetScore = 200;

    private int score = 0;
    public bool isGameOver = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SetTargetScoreByDifficulty();
        UpdateScoreUI();
    }

    private void SetTargetScoreByDifficulty()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                targetScore = 200;
                break;
            case Difficulty.Normal:
                targetScore = 300;
                break;
            case Difficulty.Hard:
                targetScore = 400;
                break;
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;

        score += 1;
        UpdateScoreUI();

        if (score >= targetScore)
        {
            WinGame();
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    public void SetGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Capybara panda = FindObjectOfType<Capybara>();
        if (panda != null)
        {
            panda.SetGameOver();
        }

        CapybaraObjectSpawner spawner = FindObjectOfType<CapybaraObjectSpawner>();
        if (spawner != null)
        {
            spawner.StopObjectSpawning();
        }

        gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        Capybara panda = FindObjectOfType<Capybara>();
        if (panda != null)
        {
            panda.SetGameOver();
        }

        CapybaraObjectSpawner spawner = FindObjectOfType<CapybaraObjectSpawner>();
        if (spawner != null)
        {
            spawner.StopObjectSpawning();
        }

        // 보상 지급
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                CoinManager.instance.AddCoins(150);
                break;
            case Difficulty.Normal:
                CoinManager.instance.AddCoins(250);
                break;
            case Difficulty.Hard:
                CoinManager.instance.AddCoins(300);
                break;
        }

        gameWinPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MiniGame");
    }

    public void ToHome()
    {
        SceneManager.LoadScene("Main");
    }
}
