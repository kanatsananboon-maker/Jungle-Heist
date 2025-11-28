using UnityEngine;

// 1. Abstract class และ Abstract method (Polymorphism: Method Overriding)
public abstract class CharacterControllerBase : MonoBehaviour
{
    // Encapsulation: ตัวแปรทั้งหมดใช้ Encapsulation
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float jumpForce = 10f;

    protected abstract void HandleMovement();
    protected abstract void HandleAnimation();
}

// 2. Inheritance
public class Player : CharacterControllerBase
{
    // ตัวแปรสำหรับ Inspector
    public Transform groundCheckPoint;
    public LayerMask groundLayer;

    // **NEW: ทำให้ groundCheckRadius เป็น [SerializeField] เพื่อให้ตั้งค่าใน Inspector ได้**
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Collider หลัก
    public BoxCollider2D standingCollider;
    public BoxCollider2D crouchCollider;

    // สถานะ
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;

    // ลบ isClimbing และ isInClimbZone ออกทั้งหมด

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (standingCollider != null && crouchCollider != null)
        {
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    void Update()
    {
        // 1. ตรวจสอบพื้น
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 2. จัดการท่าหมอบ
        HandleCrouch();

        // 3. จัดการการเคลื่อนที่และการกระโดด (ใช้ Abstract Method)
        HandleMovement();

        // 4. จัดการอนิเมชั่น (ใช้ Abstract Method)
        HandleAnimation();
    }

    // 4. Polymorphism: Method Overloading (ใช้ชื่อเดียวกันแต่มี Parameter ต่างกัน)
    private void HandleJump()
    {
        // ห้ามกระโดดขณะหมอบ
        if (!isCrouching && isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // 3. (ต่อ) จัดการการเคลื่อนที่ทั้งหมด
    protected override void HandleMovement()
    {
        HandleJump();

        // ล็อกการเคลื่อนที่แนวนอนถ้ากำลังหมอบ
        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = inputX * moveSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // พลิกตัวละคร
        if (inputX != 0)
            transform.localScale = new Vector3(Mathf.Sign(inputX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void HandleCrouch()
    {
        //isClimbing ถูกลบออกไปแล้ว
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (standingCollider != null && crouchCollider != null)
        {
            standingCollider.enabled = !isCrouching;
            crouchCollider.enabled = isCrouching;
        }
    }

    // 4. Polymorphism: Method Overriding (ต่อ)
    protected override void HandleAnimation()
    {
        // ลบ isClimbing ออกจาก Animator
        // anim.SetBool("isClimbing", isClimbing); 

        anim.SetBool("isCrouching", isCrouching);

        // ไม่ต้องมี Logic จัดการ isClimbing
        anim.speed = 1f;

        if (isCrouching) return;

        // อนิเมชั่นวิ่ง/Idle
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        // อนิเมชั่นกระโดด/ตก
        anim.SetBool("isJumping", !isGrounded);
    }
}