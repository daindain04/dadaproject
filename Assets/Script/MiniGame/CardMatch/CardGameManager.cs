using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    [Header("���� ����")]
    public GameSettings easySettings;
    public GameSettings normalSettings;
    public GameSettings hardSettings;

    [Header("ī�� ����")]
    public Transform cardParent;                // ī�尡 ��ġ�� �θ� ������Ʈ
    public GameObject cardPrefab;               // ī�� ������
    public Sprite cardBackSprite;               // ���� ī�� �޸�
    public List<Sprite> frontSprites;           // ī�� �ո� ��������Ʈ ��� (�� ����)

    private GameSettings currentSettings;
    private List<Card> spawnedCards = new List<Card>();
    private Card firstCard, secondCard;

    private float timeRemaining;
    private bool gameRunning = false;

    public Slider timerSlider;

    [Header("UI ������")]
    public CardUIManager uiManager;

    // �ʱ�ȭ�� �ܺ� ������
    public void InitializeGame(GameSettings setting)
    {
        currentSettings = setting;
        InitializeGame(); // ���� �Լ� ȣ��
    }

    // ���� ���� �ʱ�ȭ
    public void InitializeGame()
    {
        uiManager.ApplyDifficultyStyle(currentSettings);
        uiManager.ShowInGamePanel();

        ClearCards();

        List<Sprite> selected = new List<Sprite>();
        for (int i = 0; i < currentSettings.cardCount / 2; i++)
        {
            selected.Add(frontSprites[i]);
        }

        List<Sprite> allCards = new List<Sprite>();
        allCards.AddRange(selected);
        allCards.AddRange(selected);

        Shuffle(allCards);
        for (int i = 0; i < currentSettings.cardCount; i++)
        {
            GameObject go = Instantiate(cardPrefab, cardParent);
            Card card = go.GetComponent<Card>();
            card.Init(this, i, allCards[i]);

           
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() => card.OnClick());

            spawnedCards.Add(card);
        }


        timeRemaining = currentSettings.timeLimit;
        gameRunning = true;
    }

    void Update()
    {
        if (!gameRunning) return;
        Debug.Log("Ÿ�̸� �۵� ��");

        timeRemaining -= Time.deltaTime;

        Debug.Log($"Ÿ�̸� ��: {timeRemaining}");

        timerSlider.value = timeRemaining;
        uiManager.UpdateTimer(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0;
            gameRunning = false;
            StartCoroutine(HandleFail());
        }
    }

    // ī�� Ŭ�� �� ȣ���
    public bool CanReveal(Card card)
    {
        if (firstCard == null)
        {
            firstCard = card;
            return true;
        }

        if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
            return true;
        }

        return false;
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (firstCard.frontSprite == secondCard.frontSprite)
        {
            firstCard.SetMatched();
            secondCard.SetMatched();
            CheckWin();
        }
        else
        {
            firstCard.Hide();
            secondCard.Hide();
        }

        firstCard = secondCard = null;
    }

    void CheckWin()
    {
        foreach (Card card in spawnedCards)
        {
            if (!card.IsMatched()) return; // �ϳ��� �� �¾����� ���� �ƴ�
        }

        gameRunning = false;
        StartCoroutine(HandleSuccess());
    }

    IEnumerator HandleSuccess()
    {
        yield return new WaitForSeconds(0.5f);
        uiManager.ShowSuccessPanel(currentSettings.rewardCoins);
    }

    IEnumerator HandleFail()
    {
        yield return new WaitForSeconds(0.5f);
        uiManager.ShowFailPanel();
    }

    void ClearCards()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
        spawnedCards.Clear();
        firstCard = secondCard = null;
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    public void RetryGame()
    {
        InitializeGame(currentSettings); // ���� �������� �����
    }
}
