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
    AutoGridSizer gridSizer; // 그리드 자동 크기 조정기 추가
    float remaining;
    bool isRunning;
    bool isInitialized = false;
    bool isCheckingMatch = false; // 매칭 판정 중인지 확인하는 플래그 추가
    List<Card> flipped = new List<Card>();
    int matchedCount = 0;

    void Awake()
    {
        Debug.Log($"Awake 호출됨 - 오브젝트: {gameObject.name}");

        ui = GetComponent<CardUIManager>();


        grid = transform.parent.Find("CardGrid");


        if (grid != null)
        {
            gridSizer = grid.GetComponent<AutoGridSizer>(); // AutoGridSizer 컴포넌트 가져오기

        }


    }

    // 더 이상 Start() 에서 자동 초기화하지 않습니다.
    // 대신 UI 매니저에서 호출할 InitializeGame() 으로 대체

    /// <summary>
    /// UI에서 "Easy/Normal/Hard" 버튼 클릭 시 이 함수를 호출하세요.
    /// </summary>
    public void InitializeGame()
    {


        if (isInitialized)
        {

            return;
        }
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
            card.Init(face, CanCardBeClicked, OnCardClicked);
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

    /// <summary>홈 확인 패널 'Yes' 버튼</summary>
    public void OnHomeConfirmYes()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    public void OnRewardConfirm()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>홈 확인 패널 'No' 버튼 (게임 재개)</summary>
    public void OnHomeConfirmNo()
    {
        ui.ToggleHomeConfirm(false);
        isRunning = true;
    }

    /// <summary>
    /// 실패 패널에서 재시작 버튼 클릭 시 호출
    /// </summary>
    public void OnFailRestartPressed()
    {
        // 게임 상태 초기화
        ResetGame();

        // 실패 패널 숨기기
        ui.HideFail();

        // 게임 다시 시작
        InitializeGame();
    }

    /// <summary>
    /// 실패 패널에서 홈으로 돌아가기 버튼 클릭 시 호출
    /// </summary>
    public void OnFailHomePressed()
    {
        // 메인 씬으로 돌아가기
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 게임 상태를 초기화하는 함수
    /// </summary>
    void ResetGame()
    {
        // 기존 카드들 삭제
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        // 게임 상태 변수들 초기화
        flipped.Clear();
        matchedCount = 0;
        isRunning = false;
        isInitialized = false;
        isCheckingMatch = false;

        // 타이머 관련 코루틴 정리 (새로 시작할 때 InitializeGame에서 다시 시작됨)
        StopAllCoroutines();
    }

    // =================================

    /// <summary>
    /// 카드가 클릭 가능한지 확인하는 메서드
    /// </summary>
    bool CanCardBeClicked(Card card)
    {
        // 게임이 실행 중이지 않거나, 이미 뒤집힌 카드거나, 매칭 판정 중이면 클릭 불가
        if (!isRunning || flipped.Contains(card) || isCheckingMatch) return false;

        // 이미 2개의 카드가 뒤집혀있다면 클릭 불가 (추가 안전장치)
        if (flipped.Count >= 2) return false;

        return true;
    }

    void OnCardClicked(Card card)
    {
        // CanCardBeClicked에서 이미 검증했지만 추가 안전장치
        if (!CanCardBeClicked(card)) return;

        flipped.Add(card);
        if (flipped.Count == 2)
        {
            isCheckingMatch = true; // 매칭 판정 시작
            StartCoroutine(CheckMatch());
        }
    }

    // =================================

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
        isCheckingMatch = false; // 매칭 판정 완료
    }

    void OnWin()
    {
        isRunning = false;
        ui.ShowReward();

        // 난이도별 보상 지급
        switch (difficulty)
        {
            case Difficulty.Easy:
                MoneyManager.Instance.AddCoins(100);
                MoneyManager.Instance.AddExperience(10);
                Debug.Log("Easy 클리어! 코인 100, 경험치 10 획득");
                break;

            case Difficulty.Normal:
                MoneyManager.Instance.AddCoins(150);
                MoneyManager.Instance.AddGems(1);
                MoneyManager.Instance.AddExperience(20);
                Debug.Log("Normal 클리어! 코인 150, 보석 1, 경험치 20 획득");
                break;

            case Difficulty.Hard:
                MoneyManager.Instance.AddCoins(200);
                MoneyManager.Instance.AddGems(2);
                MoneyManager.Instance.AddExperience(30);
                Debug.Log("Hard 클리어! 코인 200, 보석 1, 경험치 30 획득");
                break;
        }
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
        isCheckingMatch = false; // 매칭 판정 플래그도 리셋
    }
}