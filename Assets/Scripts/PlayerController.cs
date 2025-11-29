using UnityEngine;

// 1. Abstract class และ Abstract method 
public abstract class CharacterControllerBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float jumpForce = 10f;

    protected abstract void HandleMovement();
    protected abstract void HandleAnimation();
}

// 2. Inheritance
public class PlayerController : CharacterControllerBase
{
    // ตัวแปรสำหรับ Inspector
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Collider หลัก
    public BoxCollider2D standingCollider;
    public BoxCollider2D crouchCollider;

    // **NEW: สำหรับการเกิดใหม่ (Respawn)**
    [Header("Respawn")] // หัวข้อใน Inspector
    public Transform respawnPoint; // วัตถุที่ระบุจุดเกิดใหม่ (คุณต้องสร้างใน Scene)
    private Vector3 initialRespawnPosition;

    // สถานะ
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // **NEW: บันทึกตำแหน่งเริ่มต้นเป็นจุดเกิดใหม่**
        if (respawnPoint != null)
        {
            initialRespawnPosition = respawnPoint.position;
        }
        else
        {
            // ถ้าไม่ได้กำหนด respawnPoint ให้ใช้ตำแหน่งเริ่มต้นของ Player
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

    // Polymorphism: Method Overloading
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

    // **NEW: Method สำหรับการตายและการเกิดใหม่ (ถูกเรียกจาก DeathZone.cs)**
    public void DieAndRespawn()
    {
        // 1. นำตัวละครกลับไปยังจุดเกิดใหม่
        rb.linearVelocity = Vector2.zero; // หยุดการเคลื่อนไหว
        transform.position = initialRespawnPosition;

        // *ตรงนี้คือตำแหน่งที่คุณจะใส่ Logic ของการเล่นเสียงตาย, ลดพลังชีวิต, หรือเปลี่ยน Scene*
    }
}