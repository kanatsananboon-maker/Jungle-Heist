using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    // สำหรับ Ground Check และ Crouch
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    // ** NEW: สำหรับ Crouch **
    public BoxCollider2D standingCollider;  // Collider ปกติ (ต้องลากมาใส่ใน Inspector)
    public BoxCollider2D crouchCollider;   // Collider สำหรับหมอบ (ต้องลากมาใส่ใน Inspector)

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded = false;
    private bool isCrouching = false; // 👈 NEW

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // ตรวจสอบว่า Collider ถูกตั้งค่าแล้ว
        if (standingCollider == null || crouchCollider == null)
        {
            // ตรวจสอบ Box Collider 2D ใน Player
            BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
            if (colliders.Length >= 2)
            {
                // ถ้ามี 2 อัน ให้ตั้งค่า default เอาเอง
                standingCollider = colliders[0];
                crouchCollider = colliders[1];
            }
            else if (colliders.Length == 1)
            {
                // ถ้ามีอันเดียว ให้ใช้เป็น Standing และสร้าง Crouch 
                standingCollider = colliders[0];
                // ** คุณควรสร้าง Box Collider 2D อันที่ 2 สำหรับ Crouch ใน Editor **
            }
            else
            {
                Debug.LogError("Player is missing required Box Collider 2D components for standing and crouching!");
            }
        }

        // เริ่มต้นด้วยสถานะยืน
        crouchCollider.enabled = false;
        standingCollider.enabled = true;
    }

    void Update()
    {
        // 1. ตรวจสอบพื้น (Ground Check)
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 2. การหมอบ (Crouch Logic) - ตรวจสอบก่อน Jump
        HandleCrouch();

        // 3. การกระโดด (Jump Logic)
        if (!isCrouching && isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Move();
        UpdateAnimation();
    }

    void Move()
    {
        // ถ้ากำลังหมอบอยู่ จะขยับไม่ได้ (หรือขยับได้ช้าลง ถ้าคุณต้องการ)
        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return; // ออกจากฟังก์ชัน Move เพื่อไม่ให้มีการเคลื่อนที่ในแนวนอน
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
        // ตรวจสอบการกดปุ่มหมอบ (ใช้ปุ่ม "Vertical" เมื่อมีค่าติดลบ หรือปุ่มที่กำหนดเอง)
        // นิยมใช้ Input.GetKey(KeyCode.LeftControl) หรือ Input.GetAxisRaw("Vertical") < 0
        isCrouching = Input.GetKey(KeyCode.LeftControl); // 👈 ใช้ Ctrl ซ้ายเป็นปุ่มหมอบ

        if (isCrouching)
        {
            // เปลี่ยน Collider เป็นสถานะหมอบ
            standingCollider.enabled = false;
            crouchCollider.enabled = true;
        }
        else
        {
            // กลับไปสถานะยืน
            crouchCollider.enabled = false;
            standingCollider.enabled = true;
        }
    }

    void UpdateAnimation()
    {
        // 1. อนิเมชั่นหมอบ
        anim.SetBool("isCrouching", isCrouching); // 👈 NEW

        // ถ้ากำลังหมอบอยู่ ไม่ต้องแสดงวิ่งหรือกระโดด
        if (isCrouching) return;

        // 2. อนิเมชั่นวิ่ง/Idle
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        // 3. อนิเมชั่นกระโดด/ตก
        anim.SetBool("isJumping", !isGrounded);
    }
}