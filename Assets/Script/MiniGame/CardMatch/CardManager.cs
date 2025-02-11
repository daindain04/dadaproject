using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("게임 설정")]
    public float gameTime = 20f;
    private float currentTime;
    private bool isGameActive = true;

    [Header("UI 요소")]
    public GameObject cardPrefab;
    public Transform cardContainer;
    public Text resultText;
    public Slider timerBar;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    private int matchCount = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupGame();
        StartCoroutine(TimerCountdown());
    }

    void SetupGame()
    {
        List<int> values = new List<int>();

        for (int i = 1; i <= 6; i++)
        {
            values.Add(i);
            values.Add(i);
        }

        Shuffle(values); //  오류 해결: Shuffle 함수 추가

        for (int i = 0; i < 12; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardContainer);
            Card cardComponent = newCard.GetComponent<Card>();
            cardComponent.SetCard(values[i]);
            cards.Add(cardComponent);
        }

        currentTime = gameTime;
        timerBar.maxValue = gameTime;
        timerBar.value = gameTime;
    }

    //  Shuffle 함수 추가 (CS0103 오류 해결)
    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    IEnumerator TimerCountdown()
    {
        while (currentTime > 0 && isGameActive)
        {
            currentTime -= Time.deltaTime;
            timerBar.value = currentTime;
            yield return null;
        }

        if (currentTime <= 0)
        {
            GameOver();
        }
    }

    public void CardSelected(Card card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstCard.cardValue == secondCard.cardValue)
        {
            firstCard.isMatched = true;
            secondCard.isMatched = true;
            matchCount++;

            if (matchCount == 6)
            {
                GameClear();
            }
        }
        else
        {
            firstCard.HideCard();
            secondCard.HideCard();
        }

        firstCard = null;
        secondCard = null;
    }

    void GameOver()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);
        resultText.text = " 시간 초과! 실패!";
    }

    void GameClear()
    {
        isGameActive = false;
        resultText.text = " 게임 클리어!";
    }

    public void PauseGame()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isGameActive = true;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void QuitToMain()
    {
        Time.timeScale = 1f;
        GameOver();
        SceneManager.LoadScene("MainScene");
    }
}
