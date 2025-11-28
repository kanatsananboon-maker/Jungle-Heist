using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 7f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        UpdateAnimation();
    }

    void Move()
    {
        float inputX = Input.GetAxisRaw("Horizontal");  // รับค่าจากปุ่มซ้าย/ขวา

        // กำหนดความเร็วในการเคลื่อนที่ของตัวละคร
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // ถ้ามีการกดปุ่มซ้าย/ขวา พลิกตัวละครตามทิศ
        if (inputX != 0)
            transform.localScale = new Vector3(Mathf.Sign(inputX), 1, 1);
    }

    void UpdateAnimation()
    {
        // ถ้าความเร็วในการเคลื่อนที่ในแนวนอน (x) มากกว่า 0.1f, ให้แสดงอนิเมชั่นวิ่ง
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;  // ตรวจสอบว่าเดินหรือวิ่ง
        anim.SetBool("isRunning", isRunning);  // ส่งค่าการวิ่งให้กับ Animator

        // ถ้าไม่กดปุ่มซ้าย/ขวา ให้ตัวละครไปสู่สถานะ Idle
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)  // ความเร็วในการเคลื่อนที่เล็กน้อย จะเป็น Idle
        {
            anim.SetBool("isRunning", false);  // ยกเลิกการวิ่ง
            anim.SetBool("isIdle", true);  // สถานะ Idle
        }
        else
        {
            anim.SetBool("isIdle", false);  // ถ้ากำลังวิ่งไม่ใช่ Idle
        }
    }

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
