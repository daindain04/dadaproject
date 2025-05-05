using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIManager : MonoBehaviour
{
    [Header("패널 연결")]
    public GameObject miniGameSelectPanel;
    public GameObject difficultySelectPanel;
    public GameObject cardGamePanel;
    public GameObject inGamePanel;

    [Header("게임 컨트롤러")]
    public CardGameUIManager cardGameManager;

    public void OnClickCardGame()
    {
        miniGameSelectPanel.SetActive(false);
        difficultySelectPanel.SetActive(true);
    }

    public void OnClickEasy()
    {
        difficultySelectPanel.SetActive(false);
        cardGamePanel.SetActive(true);
        inGamePanel.SetActive(true);
        cardGameManager.StartNewGame();
    }
}
