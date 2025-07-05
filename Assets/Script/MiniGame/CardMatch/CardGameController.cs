using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty { Easy, Normal, Hard }

public class CardGameController : MonoBehaviour
{
    [Header("설정")]
    public Difficulty difficulty;
    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    public int pairsCount;
    public float maxTime;

    // 내부용
    CardUIManager ui;
    Transform grid;
    float remaining;
    bool isRunning;
    bool isInitialized = false;
    List<Card> flipped = new List<Card>();
    int matchedCount = 0;

    void Awake()
    {
        ui = GetComponent<CardUIManager>();
        grid = transform.parent.Find("CardGrid");
    }

    // 더 이상 Start() 에서 자동 초기화하지 않습니다.
    // 대신 UI 매니저에서 호출할 InitializeGame() 으로 대체

    /// <summary>
    /// UI에서 “Easy/Normal/Hard” 버튼 클릭 시 이 함수를 호출하세요.
    /// </summary>
    public void InitializeGame()
    {
        if (isInitialized) return;
        isInitialized = true;

        // 타이머 세팅
        remaining = maxTime;
        isRunning = true;
        ui.ShowRunning();
        ui.UpdateTimer(remaining, maxTime);

        // 카드 배치
        SpawnCards();

        // 타이머 코루틴 시작
        StartCoroutine(TimerLoop());
    }

    IEnumerator TimerLoop()
    {
        while (true)
        {
            if (isRunning)
            {
                remaining -= Time.deltaTime;
                ui.UpdateTimer(remaining, maxTime);
                if (remaining <= 0f)
                {
                    OnFail();
                    yield break;
                }
            }
            yield return null;
        }
    }

    void SpawnCards()
    {
        // 페이스 배열 복제 및 셔플
        var faces = new List<Sprite>();
        for (int i = 0; i < pairsCount; i++)
        {
            faces.Add(cardFaces[i]);
            faces.Add(cardFaces[i]);
        }
        Shuffle(faces);

        // 프리팹 인스턴스화
        foreach (var face in faces)
        {
            var go = Instantiate(cardPrefab, grid);
            var card = go.GetComponent<Card>();
            card.Init(face, OnCardClicked);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    // =================================
    /// <summary>일시정지 버튼</summary>
    public void OnStopPressed()
    {
        isRunning = false;
        ui.ShowPaused();      // CardUIManager 에서 Stop→Restart UI 전환
    }

    /// <summary>재시작(Continue) 버튼</summary>
    public void OnRestartPressed()
    {
        isRunning = true;
        ui.ShowRunning();     // CardUIManager 에서 Restart→Stop UI 전환
    }

    /// <summary>홈 버튼 클릭 (확인 패널 띄우기)</summary>
    public void OnHomePressed()
    {
        isRunning = false;
        ui.ToggleHomeConfirm(true);
    }

    /// <summary>홈 확인 패널 ‘Yes’ 버튼</summary>
    public void OnHomeConfirmYes()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    public void OnRewardConfirm()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>홈 확인 패널 ‘No’ 버튼 (게임 재개)</summary>
    public void OnHomeConfirmNo()
    {
        ui.ToggleHomeConfirm(false);
        isRunning = true;
    }
    // =================================

    void OnCardClicked(Card card)
    {
        if (!isRunning || flipped.Contains(card)) return;
        flipped.Add(card);
        if (flipped.Count == 2) StartCoroutine(CheckMatch());
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);
        if (flipped[0].Face == flipped[1].Face)
        {
            flipped[0].SetMatched();
            flipped[1].SetMatched();
            matchedCount++;
            if (matchedCount >= pairsCount)
                OnWin();
        }
        else
        {
            flipped[0].FlipBack();
            flipped[1].FlipBack();
        }
        flipped.Clear();
    }

    void OnWin()
    {
        isRunning = false;
        ui.ShowReward();
        // 보상 지급 로직…
    }

    void OnFail()
    {
        isRunning = false;
        ui.ShowFail();
    }

    void OnDisable()
    {
        // 패널 꺼질 때 재초기화 가능하도록 플래그 리셋
        isInitialized = false;
    }
}
