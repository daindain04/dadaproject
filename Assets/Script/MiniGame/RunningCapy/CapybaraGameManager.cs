using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CapybaraGameManager : MonoBehaviour
{
    public static CapybaraGameManager instance = null;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI slidingText;

    public enum Difficulty { Easy, Normal, Hard }

    [Header("Difficulty")]
    public Difficulty currentDifficulty = Difficulty.Easy;
    private int targetScore = 200;

    [Header("슬라이딩 횟수")]
    public int easySlideCount = 3;
    public int normalSlideCount = 5;
    public int hardSlideCount = 7;
    private int remainingSlideCount;

    private int score = 0;
    public bool isGameOver = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        SetTargetScoreByDifficulty();
        SetSlideCountByDifficulty();
        UpdateScoreUI();
        UpdateSlidingText();
    }

    private void SetTargetScoreByDifficulty()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: targetScore = 200; break;
            case Difficulty.Normal: targetScore = 300; break;
            case Difficulty.Hard: targetScore = 400; break;
        }
    }

    private void SetSlideCountByDifficulty()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: remainingSlideCount = easySlideCount; break;
            case Difficulty.Normal: remainingSlideCount = normalSlideCount; break;
            case Difficulty.Hard: remainingSlideCount = hardSlideCount; break;
        }
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateSlidingText()
    {
        if (slidingText != null)
        {
            slidingText.text = remainingSlideCount.ToString();
        }
    }

    public void HandleSlideAction()
    {
        if (isGameOver || remainingSlideCount <= 0) return;

        remainingSlideCount--;
        UpdateSlidingText();

        // 슬라이딩이 마지막이면서 점수 조건도 충족 → 승리로 전환
        if (score >= targetScore && remainingSlideCount == 0)
        {
            WinGame();
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;

        score += 1;
        UpdateScoreUI();

        if (score >= targetScore)
        {
            // 점수는 도달했지만 슬라이딩 남았으면 실패 처리
            if (remainingSlideCount > 0)
            {
                SetGameOver(); // 실패로 간주
            }
            else
            {
                WinGame();
            }
        }
    }

    public void SetGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        FindObjectOfType<Capybara>()?.SetGameOver();
        FindObjectOfType<CapybaraObjectSpawner>()?.StopObjectSpawning();

        gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        FindObjectOfType<Capybara>()?.SetGameOver();
        FindObjectOfType<CapybaraObjectSpawner>()?.StopObjectSpawning();

        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                MoneyManager.Instance.AddCoins(150);
                MoneyManager.Instance.AddGems(1);
                MoneyManager.Instance.AddExperience(10);
                break;
            case Difficulty.Normal:
                MoneyManager.Instance.AddCoins(250);
                MoneyManager.Instance.AddGems(2);
                MoneyManager.Instance.AddExperience(20);
                break;
            case Difficulty.Hard:
                MoneyManager.Instance.AddCoins(300);
                MoneyManager.Instance.AddGems(4);
                MoneyManager.Instance.AddExperience(30);
                break;
        }

        gameWinPanel.SetActive(true);
    }

    public void RestartGame() => SceneManager.LoadScene("MiniGame");
    public void ToHome() => SceneManager.LoadScene("Main");
}
