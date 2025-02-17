using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Ÿ�̸� ����")]
    public Slider timerSlider;  // Ÿ�̸� �����̴� (UI)
    public float maxTime = 30f; // ���� �ð� (��)
    private float timeLeft;      // ���� �ð�
    private bool isGameRunning = false; // ���� ���� ����

    [Header(" Pause ��ư ����")]
    public Button pauseButton;   // UI�� Pause ��ư
    public Sprite playIcon;      // ��� ������ (Play)
    public Sprite pauseIcon;     // �Ͻ� ���� ������ (Pause)

    private bool isPaused = false; // ���� ������ ������� ����
    private CardGameController cardController; // ī�� ��Ʈ�ѷ� ����

    [Header(" Ȩ ��ư ����")]
    public Button homeButton;    // Ȩ ��ư (UI)
    public GameObject noticePanel; // Notice Panel (�˾�)
    public Button yesButton;     // "��" ��ư
    public Button noButton;      // "�ƴϿ�" ��ư
    public GameObject inGame;    // InGame ������Ʈ (��Ȱ��ȭ ���)

    [Header(" ���� ���� ��ư")]
    public Button gameButton;   // ���� ���� ��ư (GameButton)

    void Start()
    {
        // �ʱ� ����
        noticePanel.SetActive(false);  // Notice Panel ��Ȱ��ȭ
        inGame.SetActive(false);       // InGame ��Ȱ��ȭ
        isGameRunning = false;         // ���� ���� �� ����

        // ī�� ��Ʈ�ѷ� ã��
        cardController = FindObjectOfType<CardGameController>();

        // ��ư �̺�Ʈ �߰�
        pauseButton.onClick.AddListener(TogglePause);
        homeButton.onClick.AddListener(ShowNoticePanel);
        yesButton.onClick.AddListener(GoToMainMenu);
        noButton.onClick.AddListener(CloseNoticePanel);
        gameButton.onClick.AddListener(StartNewGame);  // ���� ���� ��ư �̺�Ʈ �߰�
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
        isPaused = !isPaused;  // ���� ����

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

        // ī�� Ŭ�� ��Ȱ��ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }

        // ��ư ������ ���� (Pause �� Play)
        pauseButton.image.sprite = playIcon;
    }

    void ResumeGame()
    {
        isPaused = false;
        isGameRunning = true;

        // ī�� Ŭ�� Ȱ��ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
        }

        // ��ư ������ ���� (Play �� Pause)
        pauseButton.image.sprite = pauseIcon;
    }

    void GameOver()
    {
        Debug.Log("�ð� �ʰ�! ���� ����");
        // �߰� ���� ���� ���� ���� (��: �г� ����, ����� ��ư Ȱ��ȭ ��)
    }

    // Ȩ ��ư�� ������ Notice Panel ǥ�� + ���� ����
    void ShowNoticePanel()
    {
        isPaused = true;
        noticePanel.SetActive(true);
        isGameRunning = false;

        // ī�� Ŭ�� ��Ȱ��ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = false;
        }
    }

    // "��" ��ư Ŭ�� �� InGame ��Ȱ��ȭ �� ���� ȭ������ �̵�
    void GoToMainMenu()
    {
        inGame.SetActive(false);
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

    // ���� ���� ��ư (GameButton) Ŭ�� �� ���ο� ���� ����
    void StartNewGame()
    {
        Debug.Log("���ο� ���� ����!");

        // InGame ������Ʈ Ȱ��ȭ
        inGame.SetActive(true);

        // Ÿ�̸� �ʱ�ȭ
        timeLeft = maxTime;
        timerSlider.value = maxTime;
        isGameRunning = true;
        isPaused = false;

        // ī�� ���� �ʱ�ȭ
        foreach (Button btn in cardController.btns)
        {
            btn.interactable = true;
            btn.image.sprite = cardController.bgImage;  // ī�� �޸����� �ʱ�ȭ
        }

        // ī�� ���� (Shuffle �Լ� ȣ��)
        ShuffleCards();
    }

    // ī�� ���� �Լ�
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
