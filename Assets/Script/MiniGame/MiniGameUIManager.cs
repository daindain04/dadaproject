using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIManager : MonoBehaviour
{
    public GameObject gameChoosePanel;
    public GameObject cardDifficultyPanel;
    public GameObject inGamePanel;

    public CardGameManager gameController;
    public GameSettings easySetting;
    public GameSettings normalSetting;
    public GameSettings hardSetting;

    public void OnCardGameButtonClicked()
    {
        gameChoosePanel.SetActive(false);
        cardDifficultyPanel.SetActive(true);
    }

    public void OnEasyButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(easySetting);
    }

    public void OnNormalButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(normalSetting);
    }

    public void OnHardButtonClicked()
    {
        cardDifficultyPanel.SetActive(false);
        inGamePanel.SetActive(true);

        gameController.InitializeGame(hardSetting);
    }
}
