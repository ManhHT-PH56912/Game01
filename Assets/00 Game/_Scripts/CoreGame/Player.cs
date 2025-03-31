using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private int jumpCount = 0;
    private const int MaxJumps = 2;
    private bool isGrounded = false;
    private bool canJump = false;

    [SerializeField] GameObject gameOverUI; // Reference to the Game Over UI
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGroundStatus();
        UpdateAnimations();
        HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // Lật hướng nhân vật khi di chuyển
        if (moveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1f, 1f);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpCount++;

        if (jumpCount != MaxJumps)
        {
            canJump = false;
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canJump = true;
            jumpCount = 0; // Reset lại số lần nhảy khi chạm đất
        }
        StatusDie();
    }

    private void UpdateAnimations()
    {
        float moveX = Input.GetAxis("Horizontal");
        // Set animation idle
        if (moveX == 0 && isGrounded)
        {
            animator.Play("Idle");
        }
        // Set animation chạy
        animator.SetBool("Run", moveX != 0);

        // Set animation nhảy
        if (rb.velocity.y > 0 && !isGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);
        }
    }

    public void StatusDie()
    {
        if (rb.velocity.y < -10f)
        {
            rb.velocity = Vector2.zero; // Dừng chuyển động của nhân vật
            Destroy(gameObject, 1f); // Thời gian chờ trước khi hủy đối tượng
            Time.timeScale = 0; // Dừng thời gian trong game
            gameOverUI.SetActive(true); // Hiện Game Over UI
        }
    }
}
