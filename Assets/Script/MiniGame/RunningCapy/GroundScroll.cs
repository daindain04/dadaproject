using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGroundGenerator : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float resetDistance = 50f; // 이 거리만큼 이동하면 리셋

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (CapybaraGameManager.instance != null && CapybaraGameManager.instance.isGameOver)
            return;

        // 왼쪽으로 이동
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // 일정 거리 이동하면 원래 위치로 리셋
        if (transform.position.x <= startPosition.x - resetDistance)
        {
            transform.position = startPosition;
        }
    }
}
