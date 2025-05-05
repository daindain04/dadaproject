using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUIManager : MonoBehaviour
{
    [Header("�г� ����")]
    public GameObject miniGameSelectPanel;
    public GameObject difficultySelectPanel;
    public GameObject cardGamePanel;
    public GameObject inGamePanel;

    [Header("���� ��Ʈ�ѷ�")]
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
