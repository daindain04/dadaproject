using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
public class CandyController : MonoBehaviour
{
    private Board board;
    private MatchChecker matchChecker;

    private Vector2 mouseDownPosition;
    private Vector2 mouseUpPosition;

    private Candy firstCandy;
    private Vector2 firstCandyPos;

    private Candy secondCandy;
    private Vector2 secondCandyPos;

    private float moveDuration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {

        board = FindObjectOfType<Board>();
        matchChecker = FindObjectOfType<MatchChecker>();


    }

    // Update is called once per frame
    void Update()
    {
        if (CandyCrushManager.instance.canMoveCandy == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition;


            Vector2 mousePos = Camera.main.ScreenToWorldPoint(mouseDownPosition);


            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                firstCandy = hit.collider.GetComponent<Candy>();
                firstCandyPos = firstCandy.transform.position;

            }
        }



        if (Input.GetMouseButtonUp(0))
        {
            mouseUpPosition = Input.mousePosition;

            float angle = Mathf.Atan2(mouseUpPosition.y - mouseDownPosition.y, mouseUpPosition.x - mouseDownPosition.x) * Mathf.Rad2Deg;
            secondCandy = GetSecondCandy(angle);

            if (firstCandy != null && secondCandy != null)
            {
                CandyCrushManager.instance.canMoveCandy = false;
                CandyCrushManager.instance.DecreaseMove();
                secondCandyPos = secondCandy.transform.position;

                //swap
                //firstCandy.transform.position = secondCandyPos;
                //secondCandy.transform.position = firstCandyPos;
                firstCandy.transform.DOMove(secondCandyPos, moveDuration);
                secondCandy.transform.DOMove(firstCandyPos, moveDuration);



                SwapCandies();
                StartCoroutine("CheckMatchRoutine");
            }
            else
            {
                ResetCandies();
            }

        }
    }

    private Candy GetSecondCandy(float angle)
    {
        if (firstCandy == null)
        {
            return null;
        }


        Candy candy = null;

        if (angle > -45 && angle < 45 && firstCandy.x < board.width - 1)
        {
            //right
            candy = board.candyMap[firstCandy.x + 1, firstCandy.y];
        }
        else if (angle > 45 && angle <= 135 && firstCandy.y < board.height - 1)
        {
            candy = board.candyMap[firstCandy.x, firstCandy.y + 1];
            //up
        }
        else if ((angle > 135 || angle <= -135) && firstCandy.x > 0)
        {
            candy = board.candyMap[firstCandy.x - 1, firstCandy.y];
            //left
        }
        else if (angle > -135 && angle <= -45 && firstCandy.y > 0)
        {
            candy = board.candyMap[firstCandy.x, firstCandy.y - 1];
            //down
        }

        return candy;
    }


    IEnumerator CheckMatchRoutine()
    {
        yield return new WaitForSeconds(moveDuration * 2f);
        matchChecker.CheckAllMatches();
        matchChecker.CheckSpecialMatches(firstCandy, secondCandy);
        if (firstCandy.isMatched == false && secondCandy.isMatched == false)
        {
            firstCandy.transform.DOMove(firstCandyPos, moveDuration * 0.5f);
            secondCandy.transform.DOMove(secondCandyPos, moveDuration * 0.5f);

            SwapCandies();
            yield return new WaitForSeconds(moveDuration);
            CandyCrushManager.instance.canMoveCandy = true;
            CandyCrushManager.instance.CheckGameOver();
        }
        else
        {
            board.StartRemoveCandiesRoutine();
        }

        ResetCandies();
    }


    private void SwapCandies()
    {
        int tempX = firstCandy.x;
        int tempY = firstCandy.y;

        firstCandy.x = secondCandy.x;
        firstCandy.y = secondCandy.y;

        secondCandy.x = tempX;
        secondCandy.y = tempY;

        board.candyMap[firstCandy.x, firstCandy.y] = firstCandy;
        board.candyMap[secondCandy.x, secondCandy.y] = secondCandy;
    }

    private void ResetCandies()
    {
        firstCandy = null;
        firstCandyPos = Vector2.zero;

        secondCandy = null;
        secondCandyPos = Vector2.zero;
    }


}


