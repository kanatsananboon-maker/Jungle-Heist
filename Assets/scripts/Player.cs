using UnityEngine;

public class Player : MonoBehaviour
{
    // กำหนดความเร็วในการเคลื่อนที่
    public float moveSpeed = 5f;
    // ลบ runSpeed ออกไปเพื่อความเรียบง่ายในการควบคุม (ตอนนี้ใช้ moveSpeed อย่างเดียว)

    private Rigidbody2D rb;
    private Animator anim;

    // สามารถเพิ่ม isGrounded กลับมาได้ถ้าต้องการทำ Jump
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // ** (ข้อควรระวังเกี่ยวกับ Scale) **
        // ตรวจสอบว่าไม่มีโค้ดบรรทัดใดๆ ที่มีการตั้งค่า transform.localScale = new Vector3(1, 1, 1);
        // ใน Start() หรือ Awake() เพราะจะทำให้ Scale ที่ตั้งค่าไว้ใน Inspector ถูก Reset
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

        // *** การแก้ไขปัญหาการเคลื่อนที่: ใช้ rb.velocity เพื่อให้ถูกต้อง ***
        // โดยคงค่าความเร็วในแกน Y ไว้ (สำหรับการกระโดด/แรงโน้มถ่วง)
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // พลิกตัวละครตามทิศทางการเคลื่อนที่
        if (inputX != 0)
            // โค้ดนี้จะเปลี่ยนแค่แกน X เพื่อ Flip ตัวละคร (จาก 1 เป็น -1 หรือกลับกัน) 
            // โดยคงค่า Scale ในแกน Y และ Z ไว้
            transform.localScale = new Vector3(Mathf.Sign(inputX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void UpdateAnimation()
    {
        // *** การแก้ไขปัญหาอนิเมชั่นค้าง: ตรวจสอบความเร็วในแนวนอน ***
        // ถ้าความเร็วในแกน X มากกว่า 0.01f แสดงว่ากำลังวิ่ง/เดิน
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

        // ส่งค่าการวิ่ง/ไม่วิ่งให้กับ Animator Parameter "isRunning"
        // ถ้า isRunning = true: Idle -> Run
        // ถ้า isRunning = false: Run -> Idle
        anim.SetBool("isRunning", isRunning);
    }

    // สามารถเพิ่ม Collision Check กลับมาได้
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}