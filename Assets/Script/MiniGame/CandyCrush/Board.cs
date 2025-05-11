using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject candyTile;


    [SerializeField]
    private Candy[] candies;
    [SerializeField]
    private Candy specialCandy;

    public Candy[,] candyMap;

    public int width;
    public int height;


    // Start is called before the first frame update
    void Start()
    {
        SetSize();
        InitCandyMap();
    }

    private void SetSize()
    {
        width = CandyCrushManager.instance.GetBoardWidth();
        height = CandyCrushManager.instance.GetBoardHeight();
    }

    private void InitCandyMap()
    {
        candyMap = new Candy[width, height];

        Vector2 centerPos = GetCenterPosition();


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);
                Instantiate(candyTile, pos, Quaternion.identity);


                CreateRandomCandy(x, y, pos, true);
            }
        }
    }

    private Vector2 GetCenterPosition()
    {
        return new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
    }


    private Candy CreateRandomCandy(int x, int y, Vector2 pos, bool checkMatch)
    {
        int index = Random.Range(0, candies.Length);
        if (checkMatch)
        {
            //int retryCount = 0;
            while (MatchHorizontally(x, y, index) || MatchVertically(x, y, index))
            {
                index = Random.Range(0, candies.Length);
                //Debug.Log("Retry Count :" + retryCount);
            }
        }
        Candy candy = Instantiate(candies[index], pos, Quaternion.identity);
        candy.Init(x, y);
        candyMap[x, y] = candy;
        return candy;

    }

    private Candy CreateSpecialCandy(int x, int y, Vector2 pos)
    {
        Candy candy = Instantiate(specialCandy, pos, Quaternion.identity);

        candy.Init(x, y);
        candyMap[x, y] = candy;
        return candy;
    }


    private bool MatchHorizontally(int x, int y, int index)
    {
        if (x > 1)
        {
            if (candyMap[x - 1, y].type == candies[index].type
                && candyMap[x - 2, y].type == candies[index].type)
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchVertically(int x, int y, int index)
    {
        if (y > 1)
        {
            if (candyMap[x, y - 1].type == candies[index].type
                && candyMap[x, y - 2].type == candies[index].type)
            {
                return true;
            }
        }
        return false;
    }

    public void StartRemoveCandiesRoutine()
    {
        StartCoroutine(RemoveCandiesRoutine());

    }

    IEnumerator RemoveCandiesRoutine()
    {
        MatchChecker matchChecker = FindObjectOfType<MatchChecker>();
        bool addSpecialCandy = false;
        for (int i = 0; i < 100; i++)
        {
            if (matchChecker.candyList.Count > 3 && matchChecker.containsSpecailCandy == false)
            {
                addSpecialCandy = true;
            }
            else
            {
                addSpecialCandy = false;
            }

            RemoveMatchedCandies(matchChecker);
            yield return new WaitForSeconds(0.8f);
            DropCandies();
            yield return new WaitForSeconds(0.8f);
            FillCandies(addSpecialCandy);
            yield return new WaitForSeconds(0.8f);
            matchChecker.CheckAllMatches();

            if (matchChecker.candyList.Count == 0)
            {
                break;
            }

        }

        CandyCrushManager.instance.canMoveCandy = true;
        CandyCrushManager.instance.CheckGameOver();

    }

    private void DropCandies()
    {
        int emptySpace = 0;
        Vector2 centerPos = GetCenterPosition();
        float moveDuration = 0.5f;

        for (int x = 0; x < width; x++)
        {
            emptySpace = 0;
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    emptySpace++;
                }
                else if (emptySpace > 0)
                {
                    Candy candy = candyMap[x, y];
                    candyMap[x, y] = null;
                    candyMap[x, y - emptySpace] = candy;
                    candy.y -= emptySpace;

                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y - emptySpace);
                    candy.transform.DOMove(pos, moveDuration).SetEase(Ease.OutBounce);
                }

            }
        }
    }

    private void FillCandies(bool addSpecialCandy)
    {
        float moveDuration = 0.25f;
        Vector2 centerPos = GetCenterPosition();
        int emptySpaceCount = GetEmptySpaceCount();
        int specialCandyIndex = Random.Range(0, emptySpaceCount);
        int currentCandyIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    Vector2 initPos = new Vector2(x - centerPos.x, y - centerPos.y + height);
                    Candy candy = null;
                    if (addSpecialCandy == true && currentCandyIndex == specialCandyIndex)
                    {
                        candy = CreateSpecialCandy(x, y, initPos);
                    }
                    else
                    {
                        candy = CreateRandomCandy(x, y, initPos, false);
                    }
                    Vector2 pos = new Vector2(x - centerPos.x, y - centerPos.y);

                    candy.transform.DOMove(pos, moveDuration);

                    currentCandyIndex++;
                }


            }
        }
    }

    private int GetEmptySpaceCount()
    {
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (candyMap[x, y] == null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void RemoveMatchedCandies(MatchChecker matchChecker)
    {
        foreach (Candy candy in matchChecker.candyList)
        {
            candyMap[candy.x, candy.y] = null;


            if (candy.type == CandyType.Blue)
            {
                CandyCrushManager.instance.DecreaseBlue();
            }
            else if (candy.type == CandyType.Green)
            {
                CandyCrushManager.instance.DecreaseGreen();
            }
            else if (candy.type == CandyType.Purple)
            {
                CandyCrushManager.instance.DecreasePurple();
            }
            else if (candy.type == CandyType.Pink)
            {
                CandyCrushManager.instance.DecreasePink();
            }
            else if (candy.type == CandyType.Orange)
            {
                CandyCrushManager.instance.DecreaseOrange();
            }

            candy.Remove();
        }
        matchChecker.candyList.Clear();
        matchChecker.containsSpecailCandy = false;
    }
}
