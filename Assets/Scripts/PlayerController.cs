using UnityEngine;

// 1. Abstract class และ Abstract method 
public abstract class CharacterControllerBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float jumpForce = 10f;

    // **NEW: Base Health (Encapsulation)**
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;

    protected abstract void HandleMovement();
    protected abstract void HandleAnimation();

    // **NEW: Virtual Method สำหรับการรับดาเมจ (Inheritance/Polymorphism)**
    public virtual void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // **NEW: Virtual Method สำหรับการตาย**
    public virtual void Die()
    {
        // ตรงนี้จะถูก Override ด้วย DieAndRespawn() ใน PlayerController
    }
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
    public Transform respawnPoint; // วัตถุที่ระบุจุดเกิดใหม่
    private Vector3 initialRespawnPosition;

    // **NEW: Health and Invulnerability Logic**
    [Header("Health & Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 1.5f;
    private float invulnerabilityTimer;

    // สถานะ
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // **NEW: กำหนดพลังชีวิตเริ่มต้น**
        currentHealth = maxHealth;
        invulnerabilityTimer = 0f;

        // **NEW: บันทึกตำแหน่งเริ่มต้นเป็นจุดเกิดใหม่**
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

        // **NEW: นับเวลาถอยหลังสำหรับช่วงอมตะ**
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

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

    // **NEW: Override TakeDamage - สำหรับจัดการช่วงอมตะเฉพาะผู้เล่น**
    public override void TakeDamage(int damageAmount)
    {
        if (invulnerabilityTimer > 0)
        {
            return; // ยังอยู่ในช่วงอมตะ
        }

        // เริ่มช่วงอมตะก่อนเรียก logic พื้นฐาน
        invulnerabilityTimer = invulnerabilityDuration;

        // เรียก logic ลดพลังชีวิตและตรวจสอบการตายจาก CharacterControllerBase
        base.TakeDamage(damageAmount);
    }

    // **NEW: Override Die() - เรียก DieAndRespawn**
    public override void Die()
    {
        DieAndRespawn();
    }

    // **NEW: Method สำหรับการตายและการเกิดใหม่ (ถูกเรียกจาก Die())**
    public void DieAndRespawn()
    {
        // 1. นำตัวละครกลับไปยังจุดเกิดใหม่
        rb.linearVelocity = Vector2.zero; // หยุดการเคลื่อนไหว
        transform.position = initialRespawnPosition;

        // **สำคัญ:** รีเซ็ตพลังชีวิตเมื่อเกิดใหม่
        currentHealth = maxHealth;
    }
}