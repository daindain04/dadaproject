using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCrushManager : MonoBehaviour
{

    public static CandyCrushManager instance = null;

    public int currentLevel = 1;
    public int remainMove = 0;
    public int[] candyGoal;

    [HideInInspector]
    public bool canMoveCandy = true;
    [HideInInspector]
    public bool isGameOver = false;


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
            remainMove = 20;
        }
        else if (currentLevel == 2)
        {
            remainMove = 15;
        }
        else
        {
            remainMove = 10;
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
            Debug.Log("Failed");
        }
    }

    private void CandySuccess()
    {
        Debug.Log("Success");
    }

}
