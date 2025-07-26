using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class MiniGameUIManager : MonoBehaviour
{


    [Header("카드 게임 관련")]
    public GameObject cardEasyPanel;
    public GameObject cardNormalPanel;
    public GameObject cardHardPanel;
    public CardGameController cardEasyManager;
    public CardGameController cardNormalManager;
    public CardGameController cardHardManager;

    [Header("3매치 게임 관련")]
    public GameObject candyCrushEasy;
    public GameObject candyCrushNormal;
    public GameObject candyCrushHard;
    public CandyCrushManager candyCrushEasyManager;
    public CandyCrushManager candyCrushNormalManager;
    public CandyCrushManager candyCrushHardManager;

    [Header("달리기 게임 관련")]
    public GameObject capyRunEasy;
    public GameObject capyRunNormal;
    public GameObject capyRunHard;
    public CapybaraGameManager capyRunEasyManager;
    public CapybaraGameManager capyRunNormalManager;
    public CapybaraGameManager capyRunHardManager;

    [Header("스피디카피 관련")]
    public GameObject SpeedyCapyEasy;
    public GameObject SpeedyCapyNormal;
    public GameObject SpeedyCapyHard;






    //--------------------------------------------------


    [Header("전체 UI 캔버스")]
    public Canvas mainUICanvas;


    [Header("게임 선택 패널")]
    public GameObject gameChoosePanel;

    [Header("게임 버튼들")]
    public Button[] gameButtons;
    public GameObject[] selectedLines;

    [Header("난이도 선택 UI")]
    public RectTransform[] difficultyPanels; // 각 게임별 난이도 패널 (위치 이동용)
    public float slideDuration = 0.4f;

    [Header("난이도 선택 버튼별 보상 이미지")]
    public GameObject[] easyRewardImages;
    public GameObject[] normalRewardImages;
    public GameObject[] hardRewardImages;



    private RectTransform currentActivePanel = null;
    private int currentGameIndex = -1;

    private void Start()
    {
        ResetAll();
    }

    private void ResetAll()
    {
        foreach (GameObject line in selectedLines)
            line.SetActive(false);

        foreach (Button btn in gameButtons)
            btn.image.color = Color.white;

        foreach (RectTransform panel in difficultyPanels)
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -645f); // 아래로 숨김
    }

    public void OnGameButtonClicked(int index)
    {
        if (currentGameIndex == index)
            return;

        // 이전 패널 숨기기
        if (currentActivePanel != null)
            StartCoroutine(SlidePanel(currentActivePanel, false));

        currentGameIndex = index;

        // 버튼 색상 초기화 및 선택 표시
        for (int i = 0; i < gameButtons.Length; i++)
        {
            gameButtons[i].image.color = (i == index) ? Color.white : new Color32(119, 119, 119, 255);
            selectedLines[i].SetActive(i == index);
        }

        // 새 패널 보이기
        currentActivePanel = difficultyPanels[index];
        StartCoroutine(SlidePanel(currentActivePanel, true));
    }

    private IEnumerator SlidePanel(RectTransform panel, bool slideUp)
    {
        float elapsed = 0f;
        Vector2 start = panel.anchoredPosition;
        Vector2 end = slideUp
    ? new Vector2(start.x, -441f)  // 올라오는 위치
    : new Vector2(start.x, -645f); // 내려가는 위치


        while (elapsed < slideDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panel.anchoredPosition = end;
    }

    // 마우스 오버 시 보상 이미지 표시
    public void OnHoverDifficulty(string difficulty)
    {
        HideAllRewardImages();

        switch (difficulty)
        {
            case "Easy":
                if (currentGameIndex >= 0 && currentGameIndex < easyRewardImages.Length)
                    easyRewardImages[currentGameIndex].SetActive(true);
                break;
            case "Normal":
                if (currentGameIndex >= 0 && currentGameIndex < normalRewardImages.Length)
                    normalRewardImages[currentGameIndex].SetActive(true);
                break;
            case "Hard":
                if (currentGameIndex >= 0 && currentGameIndex < hardRewardImages.Length)
                    hardRewardImages[currentGameIndex].SetActive(true);
                break;
        }
    }

    public void OnExitHover()
    {
        HideAllRewardImages();
    }

    private void HideAllRewardImages()
    {
        foreach (GameObject img in easyRewardImages) img.SetActive(false);
        foreach (GameObject img in normalRewardImages) img.SetActive(false);
        foreach (GameObject img in hardRewardImages) img.SetActive(false);
    }

    // 난이도 버튼 클릭 시 각 게임 시작
    public void OnSelectDifficulty(string difficulty)
    {
        // 전체 UI 캔버스 비활성화
        if (mainUICanvas != null)
            mainUICanvas.gameObject.SetActive(false);



        switch (currentGameIndex)
        {
            case 0: // CardGame
                switch (difficulty)
                {
                    case "Easy":
                        cardEasyPanel.SetActive(true);
                        cardEasyManager.InitializeGame();
                        break;
                    case "Normal":
                        cardNormalPanel.SetActive(true);
                        cardNormalManager.InitializeGame();
                        break;
                    case "Hard":
                        cardHardPanel.SetActive(true);
                        cardHardManager.InitializeGame();
                        break;
                }
                break;

            case 1: // 3MatchGame
                switch (difficulty)
                {
                    case "Easy":
                        candyCrushEasy.SetActive(true);
                        candyCrushEasyManager.currentLevel = 1;
                        break;
                    case "Normal":
                        candyCrushNormal.SetActive(true);
                        candyCrushNormalManager.currentLevel = 2;
                        break;
                    case "Hard":
                        candyCrushHard.SetActive(true);
                        candyCrushHardManager.currentLevel = 3;
                        break;
                }
                break;

            case 2: // CapyRunGame
                switch (difficulty)
                {
                    case "Easy":
                        capyRunEasy.SetActive(true);
                        capyRunEasyManager.currentDifficulty = CapybaraGameManager.Difficulty.Easy;
                        break;
                    case "Normal":
                        capyRunNormal.SetActive(true);
                        capyRunNormalManager.currentDifficulty = CapybaraGameManager.Difficulty.Normal;
                        break;
                    case "Hard":
                        capyRunHard.SetActive(true);
                        capyRunHardManager.currentDifficulty = CapybaraGameManager.Difficulty.Hard;
                        break;
                }
                break;

            case 3: // SpeedyCapy
                switch (difficulty)
                {
                    case "Easy":
                        SpeedyCapyEasy.SetActive(true);
                        break;
                    case "Normal":
                        SpeedyCapyNormal.SetActive(true);
                        break;
                    case "Hard":
                        SpeedyCapyHard.SetActive(true);
                        break;
                }
                break;

                // 추가 게임이 있다면 여기에 case 4: ... 추가
        }
    }
}
