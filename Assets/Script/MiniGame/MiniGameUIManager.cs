using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIManager : MonoBehaviour
{
    public GameObject gameChoosePanel;
    public GameObject cardDifficultyPanel;
    public GameObject BlockMatchDifficultyPanel;
    public GameObject inGamePanel;

    public CardGameManager gameController;
    public GameSettings CardEasySetting;
    public GameSettings CardNormalSetting;
    public GameSettings CardHardSetting;

    public GameObject candyCrushEasy;
    public GameObject candyCrushNormal;
    public GameObject candyCrushHard;

    public CandyCrushManager candyCrushEasyManager;
    public CandyCrushManager candyCrushNormalManager;
    public CandyCrushManager candyCrushHardManager;

    // 카드게임 관련---------------------------------------
    public void OnCardGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        cardDifficultyPanel.SetActive(true);
    }

    public void OnCardEasyButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(CardEasySetting);
    }

    public void OnCardNormalButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(CardNormalSetting);
    }

    public void OnCardHardButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(CardHardSetting);
    }


    //3매치 게임 관련 --------------------------------------------
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

    public void OnBlockMatchNomalButtonClicked()
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
}
