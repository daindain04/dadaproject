using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameManager : MonoBehaviour
{
    [Header("게임 설정")]
    public GameSettings easySettings;
    public GameSettings normalSettings;
    public GameSettings hardSettings;

    [Header("카드 관련")]
    public Transform cardParent;                // 카드가 배치될 부모 오브젝트
    public GameObject cardPrefab;               // 카드 프리팹
    public Sprite cardBackSprite;               // 공통 카드 뒷면
    public List<Sprite> frontSprites;           // 카드 앞면 스프라이트 목록 (쌍 기준)

    private GameSettings currentSettings;
    private List<Card> spawnedCards = new List<Card>();
    private Card firstCard, secondCard;

    private float timeRemaining;
    private bool gameRunning = false;

    public Slider timerSlider;

    [Header("UI 제어자")]
    public CardUIManager uiManager;

    // 초기화용 외부 진입점
    public void InitializeGame(GameSettings setting)
    {
        currentSettings = setting;
        InitializeGame(); // 내부 함수 호출
    }

    // 실제 게임 초기화
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
        Debug.Log("타이머 작동 중");

        timeRemaining -= Time.deltaTime;

        Debug.Log($"타이머 값: {timeRemaining}");

        timerSlider.value = timeRemaining;
        uiManager.UpdateTimer(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0;
            gameRunning = false;
            StartCoroutine(HandleFail());
        }
    }

    // 카드 클릭 시 호출됨
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
            if (!card.IsMatched()) return; // 하나라도 안 맞았으면 아직 아님
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
        InitializeGame(currentSettings); // 현재 설정으로 재시작
    }
}
