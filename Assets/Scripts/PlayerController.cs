using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ตัวแปรสำหรับ Movement และ Jump
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    // สำหรับจำกัดขอบเขตการเดิน (Boundary) - กันตัวละครทะลุขอบ
    [Header("World Boundaries")]
    [SerializeField] private float minXBoundary = -10f; // ขอบซ้ายสุดที่ Player จะไปได้
    [SerializeField] private float maxXBoundary = 10f;  // ขอบขวาสุดที่ Player จะไปได้

    // ตัวแปรสำหรับตรวจสอบพื้น (แก้ปัญหาลอยฟ้า)
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.3f; // เพิ่มรัศมีเล็กน้อยแก้ปัญหาเดินติด/ลอย
    [SerializeField] private LayerMask groundLayer; // Layer ที่เป็นพื้นดิน (ต้องตั้งค่าใน Inspector)

    // สำหรับการจัดการ Collider ตอนหมอบ
    [Header("Collider Settings")]
    [SerializeField] private Vector2 standingColliderSize = new Vector2(1f, 2f);
    [SerializeField] private Vector2 crouchColliderSize = new Vector2(1f, 1f);

    // สำหรับ Respawn
    [Header("Player Management")]
    [SerializeField] private Vector3 respawnPoint = new Vector3(0, 0, 0); // ตั้งค่าจุดเกิดใหม่เริ่มต้น

    [Header("Debug")]
    public bool isGroundedStatus; // สำหรับดูสถานะใน Inspector

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Animator anim;

    private float horizontalInput;
    private bool isGrounded;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        if (rb == null || playerCollider == null)
        {
            Debug.LogError("Player is missing Rigidbody2D or Collider2D components!");
        }

        // กำหนดขนาด Collider เริ่มต้น
        if (playerCollider is BoxCollider2D boxCollider)
        {
            boxCollider.size = standingColliderSize;
        }

        // ตรึงเฉพาะการหมุนแกน Z เท่านั้น
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        CheckGround();
        HandleInput();
        HandleAnimation();

        // อัพเดทสถานะ Debug
        isGroundedStatus = isGrounded;
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyBoundaryConstraint(); // ใช้การจำกัดขอบเขต
    }

    private void HandleInput()
    {
        // รับ Input การเคลื่อนที่แนวนอน (ซ้าย/ขวา)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // รับ Input การกระโดด (Jump)
        if (isGrounded && !isCrouching && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        HandleCrouchInput();
    }

    private void HandleMovement()
    {
        // 1. ควบคุมการเคลื่อนที่ซ้าย/ขวา
        if (!isCrouching)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (isCrouching && isGrounded)
        {
            // ถ้าหมอบอยู่บนพื้น ให้หยุดเคลื่อนที่เอง
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 2. พลิกตัวละครตามทิศทาง
        if (horizontalInput > 0)
        {
            Flip(1); // หันขวา
        }
        else if (horizontalInput < 0)
        {
            Flip(-1); // หันซ้าย
        }
    }

    private void ApplyBoundaryConstraint()
    {
        Vector3 position = transform.position;

        // จำกัดขอบเขตแกน X ให้อยู่ระหว่าง minXBoundary และ maxXBoundary
        position.x = Mathf.Clamp(position.x, minXBoundary, maxXBoundary);

        transform.position = position;
    }

    private void Flip(int direction)
    {
        Vector3 scale = transform.localScale;

        // พลิกตัวเมื่อทิศทางการเคลื่อนที่ต่างจากทิศทางที่หันอยู่
        if ((direction > 0 && scale.x < 0) || (direction < 0 && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void CheckGround()
    {
        // ใช้ OverlapCircle ตรวจสอบการสัมผัสพื้นด้วย LayerMask ที่ถูกต้อง
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isCrouching && !isGrounded)
        {
            SetCrouching(false);
        }
    }

    private void HandleCrouchInput()
    {
        if (isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            SetCrouching(true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow) || !Input.GetKey(KeyCode.DownArrow))
        {
            if (isCrouching)
            {
                SetCrouching(false);
            }
        }
    }

    private void SetCrouching(bool isCurrentlyCrouching)
    {
        if (isCrouching == isCurrentlyCrouching) return;

        isCrouching = isCurrentlyCrouching;

        if (playerCollider is BoxCollider2D boxCollider)
        {
            boxCollider.size = isCrouching ? crouchColliderSize : standingColliderSize;
            // ปรับ Offset ของ Collider ให้เข้ากับรูปร่างที่หมอบ
            float newOffsetY = (isCrouching ? crouchColliderSize.y : standingColliderSize.y) / 2f;
            boxCollider.offset = new Vector2(boxCollider.offset.x, newOffsetY);
        }

        if (anim != null)
        {
            anim.SetBool("isCrouching", isCrouching);
        }
    }

    private void HandleAnimation()
    {
        // ป้องกัน NullReferenceException ถ้า Animator เป็น null
        if (anim == null) return;

        // Animation Run/Walk
        bool isRunning = isGrounded && !isCrouching && Mathf.Abs(horizontalInput) > 0.01f;
        anim.SetBool("isRunning", isRunning);

        // Animation Jump/Fall
        anim.SetBool("isJumping", !isGrounded);

        // Note: isCrouching ถูกตั้งค่าใน SetCrouching() แล้ว
    }

    // **Method สำหรับ Respawn (แก้ Error ใน DeathZone.cs)**
    public void DieAndRespawn()
    {
        Debug.Log("Player has died and is attempting to respawn.");
        rb.linearVelocity = Vector2.zero; // หยุดการเคลื่อนที่
        transform.position = respawnPoint; // ย้ายกลับจุดเกิดใหม่
    }

    // **แสดง Ground Check และ Boundary ใน Scene View**
    private void OnDrawGizmos()
    {
        // วาด Ground Check point
        if (groundCheckPoint != null)
        {
            // แสดงเป็นสีเขียวเมื่ออยู่บนพื้น และสีแดงเมื่อลอย
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }

        // วาด Boundary
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minXBoundary, transform.position.y - 10, 0), new Vector3(minXBoundary, transform.position.y + 10, 0));
        Gizmos.DrawLine(new Vector3(maxXBoundary, transform.position.y - 10, 0), new Vector3(maxXBoundary, transform.position.y + 10, 0));
    }
}