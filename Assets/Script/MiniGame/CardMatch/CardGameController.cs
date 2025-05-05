using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameController : MonoBehaviour
{
    [SerializeField] public Sprite bgImage;
    public Sprite[] puzzles;

    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    public bool firstGuess, secondGuess;
    public int countGuesses;
    public int countCorrectGuesses;

    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessPuzzle, secondGuessPuzzle;

    private void Awake()
    {
        puzzles = Resources.LoadAll<Sprite>("MiniGame/CardMatch/FruitCards");
    }

    void Start()
    {
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null; // 1프레임 대기해서 CardSpawner가 버튼 생성할 시간 확보

        Debug.Log("CardGameController Start 실행됨");

        GetButtons();
        Debug.Log($"버튼 수: {btns.Count}");

        AddListeners();
        AddGamePuzzles();
        Debug.Log($"퍼즐 수: {gamePuzzles.Count}");

        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        foreach (GameObject obj in objects)
        {
            Button btn = obj.GetComponent<Button>();
            btn.image.sprite = bgImage;
            btns.Add(btn);
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => PickAPuzzle());
        }
    }

    public void AddGamePuzzles()
    {
        gamePuzzles.Clear();
        int looper = btns.Count;
        int index = 0;

        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2) index = 0;
            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    public void PickAPuzzle()
    {
        int selectedIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = selectedIndex;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = selectedIndex;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
            StartCoroutine(CheckIfThePuzzleMatch());
        }
    }

    IEnumerator CheckIfThePuzzleMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckIfTheGameIsFinished();
        }
        else
        {
            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }

        yield return new WaitForSeconds(0.5f);
        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses >= gameGuesses)
        {
            Debug.Log("모든 카드를 맞췄습니다! 게임 성공!");
            FindObjectOfType<CardGameUIManager>().StopTimer();
            FindObjectOfType<CardGameUIManager>().successPanel.SetActive(true);
            CoinManager.instance.AddCoins(100);
            ExpManager.instance.AddExp(10);
        }
    }

    public void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    public void Initialize()
    {
        Debug.Log("CardGameController 초기화 시작");

        btns.Clear();
        gamePuzzles.Clear();

        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;

        Debug.Log($"버튼 수: {btns.Count}");
        Debug.Log($"퍼즐 수: {gamePuzzles.Count}");
    }
}