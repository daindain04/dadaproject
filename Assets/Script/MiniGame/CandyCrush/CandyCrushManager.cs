using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class CandyCrushManager : MonoBehaviour
{
    //�̱��� 
    public static CandyCrushManager instance = null;

    public int currentLevel = 1;
    public int remainMove = 0;
    public int[] candyGoal;

    [HideInInspector]
    public bool canMoveCandy = true;
    [HideInInspector]
    public bool isGameOver = false;

    public TextMeshProUGUI moveText;
    public TextMeshProUGUI[] candyTexts;

    public GameObject successPanel;
    public GameObject failPanel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SetCandyGoal();
        SetMaxMove();
    }

    void Update()
    {
        UpdateUI();
    }

    public int GetBoardWidth()
    {
        if (currentLevel == 1)
        {
            return 7;
        }
        else if (currentLevel == 2)
        {
            return 8;
        }
        else
        {
            return 9;
        }
    }

    public int GetBoardHeight()
    {
        if (currentLevel == 1)
        {
            return 7;
        }
        else if (currentLevel == 2)
        {
            return 8;
        }
        else
        {
            return 9;
        }
    }

    private void SetMaxMove()
    {
        if (currentLevel == 1)
        {
            remainMove = 10;
        }
        else if (currentLevel == 2)
        {
            remainMove = 15;
        }
        else
        {
            remainMove = 20;
        }
    }

    public void DecreaseMove()
    {
        remainMove -= 1;
    }

    private void SetCandyGoal()
    {
        if (currentLevel == 1)
        {
            candyGoal = new int[] { 10, 10, 0, 0, 0 };
        }
        else if (currentLevel == 2)
        {
            candyGoal = new int[] { 0, 0, 30, 30, 0 };
        }
        else
        {
            candyGoal = new int[] { 60, 0, 0, 0, 60 };
        }
    }

    public void DecreaseBlue()
    {
        DecreaseCandyAt(0);
    }

    public void DecreaseGreen()
    {
        DecreaseCandyAt(1);
    }

    public void DecreasePurple()
    {
        DecreaseCandyAt(2);
    }

    public void DecreasePink()
    {
        DecreaseCandyAt(3);
    }

    public void DecreaseOrange()
    {
        DecreaseCandyAt(4);
    }

    private void DecreaseCandyAt(int index)
    {
        candyGoal[index] -= 1;
        if (candyGoal[index] < 0)
        {
            candyGoal[index] = 0;
        }
    }

    public void CheckGameOver()
    {
        isGameOver = true;
        foreach (int goal in candyGoal)
        {
            if (goal != 0)
            {
                isGameOver = false;
                break;
            }
        }
        if (isGameOver)
        {
            canMoveCandy = false;
            CandySuccess();
        }
        else if (remainMove == 0)
        {
            isGameOver = true;
            canMoveCandy = false;
            failPanel.SetActive(true);
            Debug.Log("Failed");
        }
    }

    private void CandySuccess()
    {
        successPanel.SetActive(true);

        // MoneyManager�� ���� ���� ����
        switch (currentLevel)
        {
            case 1: // ���̵� ��
                MoneyManager.Instance.AddCoins(200);
                MoneyManager.Instance.AddGems(1);
                MoneyManager.Instance.AddExperience(10);
                break;
            case 2: // ���̵� ��
                MoneyManager.Instance.AddCoins(250);
                MoneyManager.Instance.AddGems(2);
                MoneyManager.Instance.AddExperience(20);
                break;
            case 3: // ���̵� ��
                MoneyManager.Instance.AddCoins(300);
                MoneyManager.Instance.AddGems(3);
                MoneyManager.Instance.AddExperience(30);
                break;
        }

        // ���� UI ����
        successPanel.SetActive(true);
        Debug.Log("Success");
    }

    private void UpdateUI()
    {
        moveText.text = "" + remainMove;

        // ��� �ؽ�Ʈ �ʱ� ��Ȱ��ȭ
        foreach (TextMeshProUGUI t in candyTexts)
        {
            t.gameObject.SetActive(false);
        }

        // ���̵��� ���� ������ ���� Ȱ��ȭ
        if (currentLevel == 1) // ��
        {
            candyTexts[0].gameObject.SetActive(true); // Blue
            candyTexts[1].gameObject.SetActive(true); // Green

            candyTexts[0].text = "" + candyGoal[0];
            candyTexts[1].text = "" + candyGoal[1];
        }
        else if (currentLevel == 2) // ��
        {
            candyTexts[2].gameObject.SetActive(true); // Purple
            candyTexts[3].gameObject.SetActive(true); // Pink

            candyTexts[2].text = "" + candyGoal[2];
            candyTexts[3].text = "" + candyGoal[3];
        }
        else if (currentLevel == 3) // ��
        {
            candyTexts[0].gameObject.SetActive(true); // Blue
            candyTexts[4].gameObject.SetActive(true); // Orange

            candyTexts[0].text = "" + candyGoal[0];
            candyTexts[4].text = "" + candyGoal[4];
        }
    }

    public void OnClickHome()
    {
        SceneManager.LoadScene("Main"); // ���� �� �̸�
    }

    public void OnClickRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���� �� �ٽ� �ε�
    }
}
