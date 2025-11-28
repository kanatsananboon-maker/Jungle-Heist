using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float climbSpeed = 3f;

    // สำหรับ Ground Check
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public LayerMask climbLayer; // 👈 ยังจำเป็นต้องใช้ LayerMask นี้
    public float groundCheckRadius = 0.2f;

    // สำหรับ Crouch
    public BoxCollider2D standingCollider;
    public BoxCollider2D crouchCollider;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isClimbing = false;

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
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        HandleCrouch();
        HandleClimb(); // 👈 จัดการการปีนป่ายที่นี่

        // 3. การกระโดด (Jump Logic)
        if (!isClimbing && !isCrouching && isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Move();
        UpdateAnimation();
    }

    // ----------------------------------------------------
    // NEW: ใช้ OnTriggerStay2D ตรวจสอบว่าชนกับวัตถุปีนได้
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ตรวจสอบว่า Collider ที่ชนมี Layer ตรงกับ Climb LayerMask หรือไม่
        if (((1 << collision.gameObject.layer) & climbLayer) != 0)
        {
            // ถ้าชนกับวัตถุปีนได้และกดปุ่มขึ้น/ลง
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                isClimbing = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // เมื่อออกจากวัตถุปีนได้ ให้หยุดปีนและคืน Gravity
        if (((1 << collision.gameObject.layer) & climbLayer) != 0)
        {
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 3f; // คืน Gravity
            }
        }
    }
    // ----------------------------------------------------

    void HandleClimb()
    {
        // ถ้ากำลังปีนอยู่
        if (isClimbing)
        {
            // ปิด Gravity ชั่วคราว
            rb.gravityScale = 0f;

            // รับค่า Input ในแนวดิ่ง
            float inputY = Input.GetAxisRaw("Vertical");

            // กำหนดความเร็วในการปีน (ห้ามให้ความเร็ว X เป็น 0 เพราะจะทำให้หยุดการปีนเมื่อปล่อย W/S)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputY * climbSpeed);

            // ถ้ากระโดดขณะปีนอยู่
            if (Input.GetButtonDown("Jump"))
            {
                isClimbing = false;
                rb.gravityScale = 3f; // คืน Gravity
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // กระโดดออก
            }

            // ถ้าไม่ได้กดปุ่ม W/S/Up/Down ให้หยุดการเคลื่อนไหวในแนวตั้ง
            if (Mathf.Abs(inputY) < 0.01f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
        }
        else if (rb.gravityScale == 0 && !isClimbing)
        {
            rb.gravityScale = 3f; // ป้องกัน Gravity ค้าง
        }
    }

    void Move()
    {
        // ห้ามเคลื่อนที่แนวนอนถ้ากำลังปีนอยู่
        if (isClimbing)
        {
            // ถ้าปีนอยู่ ให้หยุดการควบคุม X และกลับไปสู่ HandleClimb
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // ห้ามเคลื่อนที่แนวนอนถ้ากำลังหมอบ
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

    // ... (ส่วน HandleCrouch และ UpdateAnimation เดิม)
    void HandleCrouch()
    {
        if (isClimbing) { isCrouching = false; return; }

        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (standingCollider != null && crouchCollider != null)
        {
            standingCollider.enabled = !isCrouching;
            crouchCollider.enabled = isCrouching;
        }
    }

    void UpdateAnimation()
    {
        anim.SetBool("isClimbing", isClimbing);
        anim.SetBool("isCrouching", isCrouching);

        if (isClimbing)
        {
            anim.speed = Mathf.Abs(rb.linearVelocity.y) > 0.1f ? 1f : 0f;
            return;
        }
        else
        {
            anim.speed = 1f;
        }

        if (isCrouching) return;

        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        anim.SetBool("isJumping", !isGrounded);
    }
}