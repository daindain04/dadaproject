using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capybara : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private BoxCollider2D boxCollider;

    [SerializeField]
    private float jumpForce = 10f;
    private bool isGrounded = false;

    // slide
    private bool isSliding = false;
    private float slideStartTime;
    private float slideDuration = 0.8f;
    private float slidePosY = -2.86f;
    private float initPosY;

    private Vector2 slideBoxColliderOffset = new Vector2(0.18f, -0.27f);
    private Vector2 slideBoxColliderSize = new Vector2(2.27f, 1.47f);
    private Vector2 initBoxColliderOffset;
    private Vector2 initBoxColliderSize;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CapybaraGameManager.instance.isGameOver == false) {
            if (Input.GetKey(KeyCode.UpArrow) && isGrounded) {
                Jump();
            } else if (Input.GetKey(KeyCode.DownArrow) && isGrounded) {
                SlideBegin();
            }

            if (isSliding && Time.time - slideStartTime >= slideDuration) {
                SlideEnd();
            }
        }
    }

    private void Jump() {
        isGrounded = false;
        rigidBody.velocity = Vector2.up * jumpForce;
        animator.SetBool("isJumping", true);
    }

    private void SlideBegin()
    {
        isGrounded = false;
        isSliding = true;
        slideStartTime = Time.time;

        initPosY = transform.position.y;
        transform.position = new Vector3(transform.position.x, slidePosY, transform.position.z);

        initBoxColliderOffset = boxCollider.offset;
        initBoxColliderSize = boxCollider.size;

        boxCollider.offset = slideBoxColliderOffset;
        boxCollider.size = slideBoxColliderSize;

        animator.SetBool("isSliding", true);

        //  슬라이딩 횟수 차감 (GameManager에게 알림)
        CapybaraGameManager.instance.HandleSlideAction();
    }

    private void SlideEnd() {
        isGrounded = true;
        isSliding = false;
        transform.position = new Vector3(transform.position.x, initPosY, transform.position.z);
        boxCollider.offset = initBoxColliderOffset;
        boxCollider.size = initBoxColliderSize;
        animator.SetBool("isSliding", false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Ground") {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    public void SetGameOver() {
        Jump();

        animator.enabled = false;
        boxCollider.enabled = false;
        Destroy(gameObject, 2f);
    }
}
