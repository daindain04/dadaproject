using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameController : MonoBehaviour
{
    [Header("Ÿ�̸� ����")]
    public Slider timerSlider;
    public float maxTime = 55f;
    private float timeLeft;
    private bool isGameRunning = false;

    [Header("Ÿ�̸� �ؽ�Ʈ")]
    public TextMeshProUGUI timerText;

    [Header(" Pause ��ư ����")]
    public Button pauseButton;
    public Sprite playIcon;
    public Sprite pauseIcon;

    private bool isPaused = false;
    private CardGameController cardController;

    [Header(" Ȩ ��ư ����")]
    public Button homeButton;
    public GameObject noticePanel;
    public Button yesButton;
    public Button noButton;
    public GameObject inGame;

    [Header(" ���� ���� ��ư")]
    public Button gameButton;

    [Header("���� ���� �г�")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button exitButton;

    [Header("���� �г� ����")]
    public GameObject successPanel; // ���� �г�
    public Button HomeButton; // ���� �г� �� ��ư
   



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
        Debug.Log("�ð� �ʰ�! ���� ����");


        isGameRunning = false;


        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }


        gameOverPanel.SetActive(true);
    }

    // Ȩ ��ư�� ������ Notice Panel ǥ�� + ���� ����
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
        Debug.Log("���� �����!");

        gameOverPanel.SetActive(false);


        ResetCards();


        StartNewGame();
      
    }
    void ResetCards()
    {
        // ī�� ����Ʈ �ʱ�ȭ
        cardController.gamePuzzles.Clear();

        // ���ο� ī�� ����Ʈ ����
        cardController.AddGamePuzzles();
        cardController.Shuffle(cardController.gamePuzzles);

        // ��� ī�� �޸����� ���� & Ŭ�� �����ϵ��� ����
        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage; // ī�� �޸� �̹����� ����
            btn.interactable = true; // �ٽ� Ŭ�� �����ϵ��� ����

            // ī���� ������ ���� (������ ���̰�)
            btn.image.color = new Color(1, 1, 1, 1);
        }
    }




    // "��" ��ư Ŭ�� �� InGame ��Ȱ��ȭ �� ���� ȭ������ �̵�
    void GoToMainMenu()
    {
        inGame.SetActive(false);
        ResetGameState();
    }

    // "�ƴϿ�" ��ư Ŭ�� �� Notice Panel �ݰ� ���� �簳
    void CloseNoticePanel()
    {
        noticePanel.SetActive(false);
        isPaused = false;
        isGameRunning = true;

        // ī�� Ŭ�� �ٽ� Ȱ��ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
        }
    }

    void StartNewGame()
    {
        Debug.Log("���ο� ���� ����!");

        // InGame ������Ʈ Ȱ��ȭ
        inGame.SetActive(true);

        //  �г� ��Ȱ��ȭ 
        noticePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // ���� ���� ������ �ʱ�ȭ
        ResetGameState();

        // Ÿ�̸� �ʱ�ȭ
        timeLeft = maxTime;
        timerSlider.value = maxTime;
        isGameRunning = true;
        isPaused = false;

        timerText.text = maxTime.ToString("F0");

        // ī�� ���� �ʱ�ȭ
        ResetCards();

        // ī�� ����
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
        isGameRunning = false; // ���� ���� ���� ����
    }
    public void ResetGameState()
    {
        // ī�� ���� ���� �ʱ�ȭ
        cardController.firstGuess = false;
        cardController.secondGuess = false;
        cardController.countGuesses = 0;
        cardController.countCorrectGuesses = 0;

        // ���� �ִ� ī�� ���� �ʱ�ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.image.sprite = cardController.bgImage; // �޸����� ����
            btn.image.color = new Color(1, 1, 1, 1);  // ���� ����
            btn.interactable = true; // �ٽ� Ŭ�� �����ϵ��� ����
        }

        // ī�� �ٽ� ����
        cardController.gamePuzzles.Clear();
        cardController.AddGamePuzzles();

        //  ShuffleCards()�� ���� ����� ȣ���!
        ShuffleCards();
    }

    void ReturnToMainMenu()
    {
        successPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        inGame.SetActive(false); // ���� ȭ�� ��Ȱ��ȭ (���� ȭ������ ��ȯ)

        ResetGameState(); // ���� ���� �ʱ�ȭ
    }

}
