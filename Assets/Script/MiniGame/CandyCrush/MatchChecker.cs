using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchChecker : MonoBehaviour
{
    public List<Candy> candyList = new List<Candy>();
    public bool containsSpecailCandy = false;

    public void CheckAllMatches()
    {
        candyList.Clear();


        Board board = FindObjectOfType<Board>();
        Candy[,] candyMap = board.candyMap;

        for (int x = 0; x < board.width - 2; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (candyMap[x, y].type == candyMap[x + 1, y].type
                    && candyMap[x, y].type == candyMap[x + 2, y].type)
                {
                    candyMap[x, y].isMatched = true;
                    candyMap[x + 1, y].isMatched = true;
                    candyMap[x + 2, y].isMatched = true;

                    candyList.Add(candyMap[x, y]);
                    candyList.Add(candyMap[x + 1, y]);
                    candyList.Add(candyMap[x + 2, y]);
                }
            }
        }


        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height - 2; y++)
            {
                if (candyMap[x, y].type == candyMap[x, y + 1].type
                    && candyMap[x, y].type == candyMap[x, y + 2].type)
                {
                    candyMap[x, y].isMatched = true;
                    candyMap[x, y + 1].isMatched = true;
                    candyMap[x, y + 2].isMatched = true;


                    candyList.Add(candyMap[x, y]);
                    candyList.Add(candyMap[x, y + 1]);
                    candyList.Add(candyMap[x, y + 2]);
                }
            }
        }

        if (candyList.Count > 0)
        {
            candyList = candyList.Distinct().ToList();
        }

    }

    public void CheckSpecialMatches(Candy firstCandy, Candy secondCandy)
    {
        if (firstCandy.type != CandyType.Special
            && secondCandy.type != CandyType.Special)
        {
            return;
        }

        Board board = FindObjectOfType<Board>();
        Candy[,] candyMap = board.candyMap;
        bool matchAll = false;
        CandyType targetType = CandyType.Special;
        if (firstCandy.type == CandyType.Special
            && secondCandy.type != CandyType.Special)
        {
            targetType = secondCandy.type;
            firstCandy.isMatched = true;
            candyList.Add(firstCandy);
        }
        else if (firstCandy.type != CandyType.Special
            && secondCandy.type == CandyType.Special)
        {
            targetType = firstCandy.type;
            secondCandy.isMatched = true;
            candyList.Add(secondCandy);
        }
        else if (firstCandy.type == CandyType.Special
            && secondCandy.type == CandyType.Special)
        {
            matchAll = true;

        }

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                if (candyMap[x, y].type == targetType || matchAll)
                {
                    candyMap[x, y].isMatched = true;
                    candyList.Add(candyMap[x, y]);
                }
            }
        }

        if (candyList.Count > 0)
        {
            candyList = candyList.Distinct().ToList();
        }

        containsSpecailCandy = true;
    }
}
