using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGroundGenerator : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public float resetDistance = 50f; // �� �Ÿ���ŭ �̵��ϸ� ����

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (CapybaraGameManager.instance != null && CapybaraGameManager.instance.isGameOver)
            return;

        // �������� �̵�
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // ���� �Ÿ� �̵��ϸ� ���� ��ġ�� ����
        if (transform.position.x <= startPosition.x - resetDistance)
        {
            transform.position = startPosition;
        }
    }
}
