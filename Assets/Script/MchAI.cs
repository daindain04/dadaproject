using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MchAI : MonoBehaviour
{
    public float speed = 1f; //  õõ�� �̵�
    public float changeDirectionTime = 3f; // ���� ��ȯ �ð�
    public float idleTime = 2f; // ���� ��� �ð�
    public LayerMask obstacleLayer; 

    private Vector2 movementDirection;
    private float timer;
    private float idleTimer;
    private bool isIdle = false; // ��� ���� ����
    private Rigidbody2D rb;
    private Animator animator;
    private float centerY; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeDirectionTime;
        idleTimer = idleTime;

        // ȭ�� �߾� Y ��ǥ ��� (���� ������Ʈ�� ��ġ�� ��������)
        centerY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).y;

        PickNewDirection(); // �ʱ� ���� ����
    }

    void Update()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                isIdle = false; // �̵� �簳
                idleTimer = idleTime;
                PickNewDirection();
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (Random.value < 0.3f) // 30% Ȯ���� ���
                {
                    isIdle = true;
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    PickNewDirection(); // ���ο� ���� ����
                }
                timer = changeDirectionTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isIdle) // ��� ���°� �ƴ� ���� �̵� 
        {
            Vector2 newPosition = rb.position + movementDirection * speed * Time.fixedDeltaTime;

            //  ��ֹ� �浹 ���� (Debug �߰�)
            Collider2D obstacleHit = Physics2D.OverlapCircle(newPosition, 0.3f, obstacleLayer);
            if (obstacleHit != null)
            {
                Debug.Log(" ��ֹ� �浹 ����! ������Ʈ: " + obstacleHit.gameObject.name);
                PickNewDirection(); // ��ֹ� ���� �� ���� ����
            }
            else if (newPosition.y <= centerY) // ȭ�� �߾� ���� �ö��� ���ϰ� ����
            {
                rb.MovePosition(newPosition);
                animator.SetBool("isWalking", true);
            }
            else
            {
                PickNewDirection(); // �߾Ӽ��� ������ �ϸ� ���ο� ���� ����
            }

            //  ĳ���� ���� ���� (����/������)
            FlipCharacter();
        }
    }

    void PickNewDirection()
    {
        movementDirection = Random.insideUnitCircle.normalized; // ���� ���� ����

        //  ��ֹ� ���� (Raycast�� �̸� Ȯ��)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movementDirection, 1f, obstacleLayer);
        if (hit.collider != null)
        {
            movementDirection = -movementDirection; // ��ֹ��� ������ �ݴ� �������� ����
        }

        // ���� ���� ������ ȭ�� �߾Ӽ��� �Ѿ�� �����̶�� �ٽ� ����
        if (transform.position.y + movementDirection.y > centerY)
        {
            movementDirection.y = -Mathf.Abs(movementDirection.y);
        }

        Debug.Log("�� �̵� ����: " + movementDirection);

        //  ���� ���� �� ĳ���� ���� ����
        FlipCharacter();
    }

    //  ĳ���Ͱ� �̵� ���⿡ ���� �¿� ����
    void FlipCharacter()
    {
        if (movementDirection.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // �������� �ٶ�
        }
        else if (movementDirection.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // ������ �ٶ�
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            Debug.Log("Trigger �浹 ����! ������Ʈ: " + collision.gameObject.name);
            PickNewDirection(); // �浹 ���� �� ���� ����
        }
    }
}
