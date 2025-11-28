using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // เปลี่ยน moveSpeed เป็นความเร็วเดียวที่ใช้ในการเคลื่อนที่
    public float moveSpeed = 5f;
    // ลบ runSpeed ออกไปเพื่อลดความซ้ำซ้อนในตอนนี้

    private Rigidbody2D rb;
    private Animator anim;

    // ลบ isGrounded ออกไปก่อนเพื่อความเรียบง่ายในการแก้ไขปัญหาหลัก
    // (สามารถเพิ่มกลับมาได้เมื่อต้องการ Jump)

    void Start()
    {
        // ตรวจสอบให้แน่ใจว่าตัวละครมี Rigidbody2D และ Animator ติดอยู่
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // ตรวจสอบค่า Rigidbody เพื่อให้มั่นใจว่า Gravity Scale ไม่เป็น 0
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from Player!");
        }
        if (anim == null)
        {
            Debug.LogError("Animator component missing from Player!");
        }
    }

    void Update()
    {
        Move();
        UpdateAnimation();
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // รับค่าจากปุ่มซ้าย/ขวา

        // คำนวณความเร็วเป้าหมายในแกน X
        float targetVelocityX = inputX * moveSpeed;

        // *** การแก้ไขที่สำคัญ: ใช้ rb.velocity เพื่อควบคุมการเคลื่อนที่ 
        // โดยคงค่าความเร็วในแกน Y ไว้ (สำหรับการกระโดด/แรงโน้มถ่วง) ***
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // พลิกตัวละครตามทิศทางการเคลื่อนที่
        if (inputX != 0)
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1, 1);
    }

    void UpdateAnimation()
    {
        // *** การแก้ไขที่สำคัญ: ตรวจสอบว่าความเร็วในแนวนอน (velocity.x) มากกว่าค่า Threshold หรือไม่ ***
        // ถ้ามากกว่า 0.01f แสดงว่ากำลังวิ่ง/เดิน 
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

        // ส่งค่าการวิ่ง/ไม่วิ่งให้กับ Animator Parameter "isRunning"
        // ถ้า isRunning เป็น true: Idle -> Run
        // ถ้า isRunning เป็น false: Run -> Idle
        anim.SetBool("isRunning", isRunning);

        // เราจะไม่ใช้ "isIdle" Parameter เพราะสามารถใช้ isRunning = false เพื่อเปลี่ยนกลับไป Idle ได้เลย
    }
}

    // ลบ OnCollisionEnter2D และ OnCollisionExit