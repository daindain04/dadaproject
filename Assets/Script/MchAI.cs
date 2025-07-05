using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MchAI : MonoBehaviour
{
    public float speed = 1f; //  천천히 이동
    public float changeDirectionTime = 3f; // 방향 전환 시간
    public float idleTime = 2f; // 랜덤 대기 시간
    public LayerMask obstacleLayer; 

    private Vector2 movementDirection;
    private float timer;
    private float idleTimer;
    private bool isIdle = false; // 대기 상태 여부
    private Rigidbody2D rb;
    private Animator animator;
    private float centerY; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeDirectionTime;
        idleTimer = idleTime;

        // 화면 중앙 Y 좌표 계산 (현재 오브젝트의 위치를 기준으로)
        centerY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).y;

        PickNewDirection(); // 초기 방향 설정
    }

    void Update()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                isIdle = false; // 이동 재개
                idleTimer = idleTime;
                PickNewDirection();
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (Random.value < 0.3f) // 30% 확률로 대기
                {
                    isIdle = true;
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    PickNewDirection(); // 새로운 방향 설정
                }
                timer = changeDirectionTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isIdle) // 대기 상태가 아닐 때만 이동 
        {
            Vector2 newPosition = rb.position + movementDirection * speed * Time.fixedDeltaTime;

            //  장애물 충돌 감지 (Debug 추가)
            Collider2D obstacleHit = Physics2D.OverlapCircle(newPosition, 0.3f, obstacleLayer);
            if (obstacleHit != null)
            {
                Debug.Log(" 장애물 충돌 감지! 오브젝트: " + obstacleHit.gameObject.name);
                PickNewDirection(); // 장애물 감지 시 방향 변경
            }
            else if (newPosition.y <= centerY) // 화면 중앙 위로 올라가지 못하게 제한
            {
                rb.MovePosition(newPosition);
                animator.SetBool("isWalking", true);
            }
            else
            {
                PickNewDirection(); // 중앙선을 넘으려 하면 새로운 방향 설정
            }

            //  캐릭터 방향 변경 (왼쪽/오른쪽)
            FlipCharacter();
        }
    }

    void PickNewDirection()
    {
        movementDirection = Random.insideUnitCircle.normalized; // 랜덤 방향 설정

        //  장애물 감지 (Raycast로 미리 확인)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movementDirection, 1f, obstacleLayer);
        if (hit.collider != null)
        {
            movementDirection = -movementDirection; // 장애물이 있으면 반대 방향으로 변경
        }

        // 만약 현재 방향이 화면 중앙선을 넘어가는 방향이라면 다시 선택
        if (transform.position.y + movementDirection.y > centerY)
        {
            movementDirection.y = -Mathf.Abs(movementDirection.y);
        }

        Debug.Log("새 이동 방향: " + movementDirection);

        //  방향 변경 후 캐릭터 방향 조정
        FlipCharacter();
    }

    //  캐릭터가 이동 방향에 따라 좌우 반전
    void FlipCharacter()
    {
        if (movementDirection.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 오른쪽을 바라봄
        }
        else if (movementDirection.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // 왼쪽을 바라봄
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            Debug.Log("Trigger 충돌 감지! 오브젝트: " + collision.gameObject.name);
            PickNewDirection(); // 충돌 감지 후 방향 변경
        }
    }
}
