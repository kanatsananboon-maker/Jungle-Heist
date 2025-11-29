using UnityEngine;

// 1. Abstract class และ Abstract method 
public abstract class CharacterControllerBase : MonoBehaviour
{
    // Encapsulation
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float jumpForce = 10f;

    protected abstract void HandleMovement();
    protected abstract void HandleAnimation();
}

// 2. Inheritance
public class PlayerController : CharacterControllerBase
{
    // Collider และ Ground Check
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    public BoxCollider2D standingCollider;
    public BoxCollider2D crouchCollider;

    [Header("Respawn")]
    public Transform respawnPoint;
    private Vector3 initialRespawnPosition;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // บันทึกตำแหน่ง Respawn
        if (respawnPoint != null)
        {
            initialRespawnPosition = respawnPoint.position;
        }
        else
        {
            initialRespawnPosition = transform.position;
        }

        if (standingCollider != null && crouchCollider != null)
        {
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        HandleCrouch();
        HandleMovement();
        HandleAnimation();
    }

    private void HandleJump()
    {
        if (!isCrouching && isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    protected override void HandleMovement()
    {
        HandleJump();

        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = inputX * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        if (inputX != 0)
            transform.localScale = new Vector3(Mathf.Sign(inputX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void HandleCrouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (standingCollider != null && crouchCollider != null)
        {
            standingCollider.enabled = !isCrouching;
            crouchCollider.enabled = isCrouching;
        }
    }

    protected override void HandleAnimation()
    {
        anim.SetBool("isCrouching", isCrouching);
        anim.speed = 1f;

        if (isCrouching) return;

        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        anim.SetBool("isJumping", !isGrounded);
    }

    // Method สำหรับการตายและการเกิดใหม่
    public void DieAndRespawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = initialRespawnPosition;
    }
}