using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BearController : Enemy // สืบทอดจาก EnemyController
{
    [Header("Bear Movement")]
    [SerializeField] private float bearWalkSpeed = 2f;
    [SerializeField] private float bearWalkDistance = 3f;

    private Animator anim;
    private Vector3 startingPosition;
    private float direction = 1f;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>(); // กำหนด Animator Component
        startingPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();
        HandleAnimation(); // **สำคัญ: เรียกใช้ Animation**
    }

    protected override void HandleMovement()
    {
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

        rb.linearVelocity = new Vector2(direction * bearWalkSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // **NEW: Method สำหรับจัดการ Animation (ใช้ Speed แทน Bool)**
    void HandleAnimation()
    {
        if (anim != null)
        {
            // ตรวจสอบความเร็วในการเคลื่อนที่
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

            if (isWalking)
            {
                // ถ้าหมีเดินอยู่ ให้ Animation เล่นด้วยความเร็วปกติ (1.0)
                anim.speed = 1f;
            }
            else
            {
                // ถ้าหมีหยุดนิ่ง ให้หยุด Animation ด้วยการตั้งค่า Speed เป็น 0
                anim.speed = 0f;
            }

            // หมายเหตุ: โค้ดนี้จะใช้กับ State ปัจจุบันใน Animator เท่านั้น (ซึ่งคือ Walk)
            // เมื่อ anim.speed = 0f, Sprite จะค้างอยู่ที่เฟรมนั้น
        }
    }

    // คุณสามารถ Override Die() ตรงนี้ได้
    // ...
}