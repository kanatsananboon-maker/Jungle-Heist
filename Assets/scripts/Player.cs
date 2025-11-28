using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 3f; // 👈 NEW: ความเร็วในการปีน

    // สำหรับ Ground Check และ Climb
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public LayerMask climbLayer; // 👈 NEW: Layer สำหรับต้นปาล์ม/เถาวัลย์
    public float groundCheckRadius = 0.2f;

    // สำหรับ Crouch
    public BoxCollider2D standingCollider;
    public BoxCollider2D crouchCollider;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isClimbing = false; // 👈 NEW: สถานะปีนป่าย

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // ตรวจสอบ Collider สำหรับ Crouch
        if (standingCollider != null && crouchCollider != null)
        {
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // ตรวจสอบการชนกับวัตถุปีนได้ (ใช้ Collider ที่ยืนอยู่)
        CheckForClimbable(); // 👈 NEW

        HandleCrouch();
        HandleClimb(); // 👈 NEW

        // 3. การกระโดด (Jump Logic)
        if (!isClimbing && !isCrouching && isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Move();
        UpdateAnimation();
    }

    // ----------------------------------------------------
    // NEW: เมธอดจัดการการปีนป่าย
    void HandleClimb()
    {
        // ถ้ากำลังปีนอยู่
        if (isClimbing)
        {
            // ปิด Gravity ชั่วคราว
            rb.gravityScale = 0f;

            // รับค่า Input ในแนวดิ่ง (W/S หรือ Up/Down)
            float inputY = Input.GetAxisRaw("Vertical");

            // กำหนดความเร็วในการปีน
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputY * climbSpeed);

            // ถ้ากระโดดขณะปีนอยู่ (Jump off the climbable object)
            if (Input.GetButtonDown("Jump"))
            {
                isClimbing = false;
                rb.gravityScale = 3f; // คืน Gravity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // กระโดดออก
            }
        }
    }

    // ----------------------------------------------------

    void Move()
    {
        // ห้ามเคลื่อนที่แนวนอนถ้ากำลังปีนอยู่ หรือกำลังหมอบ
        if (isClimbing || isCrouching)
        {
            if (!isClimbing) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // หยุดแนวนอนถ้าหมอบ
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
        if (isClimbing) // ห้ามหมอบขณะปีน
        {
            isCrouching = false;
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
            return;
        }

        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching)
        {
            standingCollider.enabled = false;
            crouchCollider.enabled = true;
        }
        else
        {
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    void UpdateAnimation()
    {
        // 1. อนิเมชั่นปีนป่าย
        anim.SetBool("isClimbing", isClimbing); // 👈 NEW

        // 2. อนิเมชั่นหมอบ
        anim.SetBool("isCrouching", isCrouching);

        // ถ้ากำลังปีนอยู่ ไม่ต้องแสดงวิ่ง กระโดด หรือหมอบ
        if (isClimbing)
        {
            anim.speed = Mathf.Abs(rb.linearVelocity.y) > 0.1f ? 1f : 0f; // เล่นอนิเมชั่นถ้ามีการขยับ
            return;
        }
        else
        {
            anim.speed = 1f; // คืนความเร็วอนิเมชั่นปกติ
        }

        // ถ้ากำลังหมอบอยู่ ไม่ต้องแสดงวิ่งหรือกระโดด
        if (isCrouching) return;

        // 3. อนิเมชั่นวิ่ง/Idle
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        // 4. อนิเมชั่นกระโดด/ตก
        anim.SetBool("isJumping", !isGrounded);
    }

    // ----------------------------------------------------
    // NEW: ตรวจสอบการเข้า/ออกจากวัตถุที่ปีนได้
    void CheckForClimbable()
    {
        // ใช้ Collider ของตัวละครเอง ตรวจสอบว่าชนกับ Layer ของวัตถุปีนได้หรือไม่
        Collider2D hit = Physics2D.OverlapBox(standingCollider.bounds.center, standingCollider.bounds.size, 0, climbLayer);

        if (hit != null && Input.GetKey(KeyCode.W)) // ถ้าชนวัตถุปีนได้ และกดปุ่มขึ้น
        {
            isClimbing = true;
        }
        else if (isClimbing && hit == null) // ถ้ากำลังปีนอยู่ แต่หลุดจากวัตถุปีนได้
        {
            isClimbing = false;
            rb.gravityScale = 3f; // คืน Gravity
        }
        else if (isClimbing && isGrounded && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            // ถ้าอยู่บนพื้นดินและไม่ได้กดปุ่มปีน (W/S) ให้หยุดปีน
            isClimbing = false;
            rb.gravityScale = 3f; // คืน Gravity
        }
        else if (isClimbing && isGrounded && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            // ถ้าใช้ Arrow Keys แทน W/S
            isClimbing = false;
            rb.gravityScale = 3f; // คืน Gravity
        }

        if (!isClimbing && rb.gravityScale == 0) // ป้องกัน Gravity ค้าง
        {
            rb.gravityScale = 3f; // คืนค่า Gravity Scale เดิม (หรือค่าที่คุณตั้งไว้)
        }
    }

    // ป้องกัน Gravity ค้างเมื่อเลิกเล่นเกม
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.gravityScale = 3f; // คืนค่า Gravity Scale ให้เป็นปกติ
        }
    }
}