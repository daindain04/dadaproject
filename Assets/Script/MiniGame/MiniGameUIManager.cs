using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class MiniGameUIManager : MonoBehaviour
{


    [Header("ī�� ���� ����")]
    public GameObject cardEasyPanel;
    public GameObject cardNormalPanel;
    public GameObject cardHardPanel;
    public CardGameController cardEasyManager;
    public CardGameController cardNormalManager;
    public CardGameController cardHardManager;

    [Header("3��ġ ���� ����")]
    public GameObject candyCrushEasy;
    public GameObject candyCrushNormal;
    public GameObject candyCrushHard;
    public CandyCrushManager candyCrushEasyManager;
    public CandyCrushManager candyCrushNormalManager;
    public CandyCrushManager candyCrushHardManager;

    [Header("�޸��� ���� ����")]
    public GameObject capyRunEasy;
    public GameObject capyRunNormal;
    public GameObject capyRunHard;
    public CapybaraGameManager capyRunEasyManager;
    public CapybaraGameManager capyRunNormalManager;
    public CapybaraGameManager capyRunHardManager;

    [Header("���ǵ�ī�� ����")]
    public GameObject SpeedyCapyEasy;
    public GameObject SpeedyCapyNormal;
    public GameObject SpeedyCapyHard;






    //--------------------------------------------------


    [Header("��ü UI ĵ����")]
    public Canvas mainUICanvas;


    [Header("���� ���� �г�")]
    public GameObject gameChoosePanel;

    [Header("���� ��ư��")]
    public Button[] gameButtons;
    public GameObject[] selectedLines;

    [Header("���̵� ���� UI")]
    public RectTransform[] difficultyPanels; // �� ���Ӻ� ���̵� �г� (��ġ �̵���)
    public float slideDuration = 0.4f;

    [Header("���̵� ���� ��ư�� ���� �̹���")]
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
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -645f); // �Ʒ��� ����
    }

    public void OnGameButtonClicked(int index)
    {
        if (currentGameIndex == index)
            return;

        // ���� �г� �����
        if (currentActivePanel != null)
            StartCoroutine(SlidePanel(currentActivePanel, false));

        currentGameIndex = index;

        // ��ư ���� �ʱ�ȭ �� ���� ǥ��
        for (int i = 0; i < gameButtons.Length; i++)
        {
            gameButtons[i].image.color = (i == index) ? Color.white : new Color32(119, 119, 119, 255);
            selectedLines[i].SetActive(i == index);
        }

        // �� �г� ���̱�
        currentActivePanel = difficultyPanels[index];
        StartCoroutine(SlidePanel(currentActivePanel, true));
    }

    private IEnumerator SlidePanel(RectTransform panel, bool slideUp)
    {
        float elapsed = 0f;
        Vector2 start = panel.anchoredPosition;
        Vector2 end = slideUp
    ? new Vector2(start.x, -441f)  // �ö���� ��ġ
    : new Vector2(start.x, -645f); // �������� ��ġ


        while (elapsed < slideDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panel.anchoredPosition = end;
    }

    // ���콺 ���� �� ���� �̹��� ǥ��
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

    // ���̵� ��ư Ŭ�� �� �� ���� ����
    public void OnSelectDifficulty(string difficulty)
    {
        // ��ü UI ĵ���� ��Ȱ��ȭ
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

                // �߰� ������ �ִٸ� ���⿡ case 4: ... �߰�
        }
    }
}
