using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BearController : Enemy // สืบทอดจาก EnemyController
{
    [Header("Bear Movement")]
    [SerializeField] private float bearWalkSpeed = 2f;
    [SerializeField] private float bearWalkDistance = 3f;

    // **NEW: ตัวแปรสำหรับ Animation**
    private Animator anim;

    private Vector3 startingPosition;
    private float direction = 1f;

    protected override void Start()
    {
        base.Start(); // เรียก Start ของ EnemyController ก่อน (ตั้งค่า Rigidbody และ Health)

        // **NEW: กำหนด Animator Component**
        anim = GetComponent<Animator>();

        startingPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update(); // เรียก Update ของ EnemyController ก่อน
        HandleAnimation(); // **NEW: เรียกใช้ Animation**
    }

    // 4. Polymorphism: Implement Abstract Method HandleMovement()
    protected override void HandleMovement()
    {
        // **Logic การเดินซ้าย-ขวา (จำกัดขอบเขต)**
        // โค้ดนี้ทำให้หมีเดินไป-กลับตาม walkDistance รอบจุดเกิด (startingPosition)
        float distanceTravelled = transform.position.x - startingPosition.x;

        if (distanceTravelled > bearWalkDistance)
        {
            direction = -1f;
            Flip();
        }
        else if (distanceTravelled < -bearWalkDistance)
        {
            direction = 1f;
            Flip();
        }

        // **NEW: ทำให้หมีเดินไป-กลับเฉพาะบนพื้น**
        // เราใช้ rb.velocity.y เพื่อให้ Rigidbody 2D จัดการแรงโน้มถ่วง (Gravity) เอง
        rb.linearVelocity = new Vector2(direction * bearWalkSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        // พลิก Sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // **NEW: Method สำหรับจัดการ Animation**
    void HandleAnimation()
    {
        if (anim != null)
        {
            // ใช้ค่าความเร็วในแนวนอนเพื่อกำหนดว่ากำลังวิ่งหรือไม่
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

            // ตั้งค่า Parameter ใน Animator Controller
            anim.SetBool("isWalking", isWalking); // คุณต้องสร้าง Parameter ชื่อ "isWalking" เป็นชนิด Bool ใน Animator
        }
    }

    // **NEW: Override Die เพื่อให้เล่น Animation ตายได้**
    protected override void Die()
    {
        // *คุณสามารถเรียกใช้ anim.SetTrigger("Die") และใช้ Destroy(gameObject, delay) แทน*
        base.Die(); // ทำลาย Game Object
    }
}