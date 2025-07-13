using UnityEngine;

public class MiniGameUIManager : MonoBehaviour
{
    [Header("���� �г�")]
    public GameObject gameChoosePanel;

    // ����������������������������������������������������������������������������������������
    [Header("ī�� ���� ����")]
    public GameObject cardDifficultyPanel;
    public GameObject cardEasyPanel;
    public GameObject cardNormalPanel;
    public GameObject cardHardPanel;
    public CardGameController cardEasyManager;
    public CardGameController cardNormalManager;
    public CardGameController cardHardManager;
    // ����������������������������������������������������������������������������������������

    [Header("3��ġ ���� ����")]
    public GameObject BlockMatchDifficultyPanel;
    public GameObject candyCrushEasy;
    public GameObject candyCrushNormal;
    public GameObject candyCrushHard;
    public CandyCrushManager candyCrushEasyManager;
    public CandyCrushManager candyCrushNormalManager;
    public CandyCrushManager candyCrushHardManager;

    [Header("�޸��� ���� ����")]
    public GameObject capyDifficultyPanel;
    public GameObject capyRunEasy;
    public GameObject capyRunNormal;
    public GameObject capyRunHard;
    public CapybaraGameManager capyRunEasyManager;
    public CapybaraGameManager capyRunNormalManager;
    public CapybaraGameManager capyRunHardManager;

    [Header("���ǵ�ī�� ����")]
    public GameObject SpeedyDifficultyPanel;
    public GameObject SpeedyCapyEasy;
    public GameObject SpeedyCapyNormal;
    public GameObject SpeedyCapyHard;

    // ================= ī����� �̺�Ʈ =================
    public void OnCardGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        cardDifficultyPanel.SetActive(true);
    }

    public void OnCardEasyButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        cardEasyPanel.SetActive(true);
        cardEasyManager.InitializeGame();
    }

    public void OnCardNormalButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        cardNormalPanel.SetActive(true);
        cardNormalManager.InitializeGame();
    }

    public void OnCardHardButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        cardHardPanel.SetActive(true);
        cardHardManager.InitializeGame();
    }

    // ================ 3��ġ ���� �̺�Ʈ ================
    public void OnMatchGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        BlockMatchDifficultyPanel.SetActive(true);
    }

    public void OnBlockMatchEasyButtonClicked()
    {
        BlockMatchDifficultyPanel.SetActive(false);
        candyCrushEasy.SetActive(true);
        candyCrushEasyManager.currentLevel = 1;
    }

    public void OnBlockMatchNormalButtonClicked()
    {
        BlockMatchDifficultyPanel.SetActive(false);
        candyCrushNormal.SetActive(true);
        candyCrushNormalManager.currentLevel = 2;
    }

    public void OnBlockMatchHardButtonClicked()
    {
        BlockMatchDifficultyPanel.SetActive(false);
        candyCrushHard.SetActive(true);
        candyCrushHardManager.currentLevel = 3;
    }

    // =============== �޸��� ���� �̺�Ʈ ================
    public void OnCapyRunGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        capyDifficultyPanel.SetActive(true);
    }

    public void OnCapyRunEasyButtonClicked()
    {
        capyDifficultyPanel.SetActive(false);
        capyRunEasy.SetActive(true);
        capyRunEasyManager.currentDifficulty = CapybaraGameManager.Difficulty.Easy;
    }

    public void OnCapyRunNormalButtonClicked()
    {
        capyDifficultyPanel.SetActive(false);
        capyRunNormal.SetActive(true);
        capyRunNormalManager.currentDifficulty = CapybaraGameManager.Difficulty.Normal;
    }

    public void OnCapyRunHardButtonClicked()
    {
        capyDifficultyPanel.SetActive(false);
        capyRunHard.SetActive(true);
        capyRunHardManager.currentDifficulty = CapybaraGameManager.Difficulty.Hard;
    }

    // =============== ���ǵ�ī�� ���� �̺�Ʈ ================
    public void OnSpeedyCapyGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        SpeedyDifficultyPanel.SetActive(true);
    }

    public void OnSpeedyCapyEasyButtonClicked()
    {
        SpeedyDifficultyPanel.SetActive(false);
        SpeedyCapyEasy.SetActive(true);
    }

    public void OnSpeedyCapyNormalButtonClicked()
    {
        SpeedyDifficultyPanel.SetActive(false);
        SpeedyCapyNormal.SetActive(true);
    }

    public void OnSpeedyCapyHardButtonClicked()
    {
        SpeedyDifficultyPanel.SetActive(false);
        SpeedyCapyHard.SetActive(true);
    }
}
