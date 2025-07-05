using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty { Easy, Normal, Hard }

public class CardGameController : MonoBehaviour
{
    [Header("����")]
    public Difficulty difficulty;
    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    public int pairsCount;
    public float maxTime;

    // ���ο�
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

    // �� �̻� Start() ���� �ڵ� �ʱ�ȭ���� �ʽ��ϴ�.
    // ��� UI �Ŵ������� ȣ���� InitializeGame() ���� ��ü

    /// <summary>
    /// UI���� ��Easy/Normal/Hard�� ��ư Ŭ�� �� �� �Լ��� ȣ���ϼ���.
    /// </summary>
    public void InitializeGame()
    {
        if (isInitialized) return;
        isInitialized = true;

        // Ÿ�̸� ����
        remaining = maxTime;
        isRunning = true;
        ui.ShowRunning();
        ui.UpdateTimer(remaining, maxTime);

        // ī�� ��ġ
        SpawnCards();

        // Ÿ�̸� �ڷ�ƾ ����
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
        // ���̽� �迭 ���� �� ����
        var faces = new List<Sprite>();
        for (int i = 0; i < pairsCount; i++)
        {
            faces.Add(cardFaces[i]);
            faces.Add(cardFaces[i]);
        }
        Shuffle(faces);

        // ������ �ν��Ͻ�ȭ
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
    /// <summary>�Ͻ����� ��ư</summary>
    public void OnStopPressed()
    {
        isRunning = false;
        ui.ShowPaused();      // CardUIManager ���� Stop��Restart UI ��ȯ
    }

    /// <summary>�����(Continue) ��ư</summary>
    public void OnRestartPressed()
    {
        isRunning = true;
        ui.ShowRunning();     // CardUIManager ���� Restart��Stop UI ��ȯ
    }

    /// <summary>Ȩ ��ư Ŭ�� (Ȯ�� �г� ����)</summary>
    public void OnHomePressed()
    {
        isRunning = false;
        ui.ToggleHomeConfirm(true);
    }

    /// <summary>Ȩ Ȯ�� �г� ��Yes�� ��ư</summary>
    public void OnHomeConfirmYes()
    {
        // ���� ������ ���ư���
        SceneManager.LoadScene("Main");
    }

    public void OnRewardConfirm()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>Ȩ Ȯ�� �г� ��No�� ��ư (���� �簳)</summary>
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
        // ���� ���� ������
    }

    void OnFail()
    {
        isRunning = false;
        ui.ShowFail();
    }

    void OnDisable()
    {
        // �г� ���� �� ���ʱ�ȭ �����ϵ��� �÷��� ����
        isInitialized = false;
    }
}
