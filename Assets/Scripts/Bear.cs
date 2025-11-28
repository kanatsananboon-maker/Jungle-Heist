using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bear : Enemy
{
    [Header("Bear Movement")]
    // ตัวแปรนี้กำหนดขีดจำกัดการเดินจากจุดเริ่มต้น (Center)
    [SerializeField] private float bearWalkDistance = 3f;
    [SerializeField] private float bearWalkSpeed = 2f;

    private float direction = 1f; // 1 = ขวา, -1 = ซ้าย
    private Vector3 startingPosition; // จุดเริ่มต้น/จุดกึ่งกลาง (Center)
    private Animator anim;

    protected override void Start()
    {
        // **สำคัญ:** เรียก Start ของ EnemyController เพื่อกำหนด rb
        base.Start();

        // หา Component Animator
        anim = GetComponent<Animator>();

        // กำหนดตำแหน่งเริ่มต้น (Center Point) เมื่อเกมเริ่ม
        startingPosition = transform.position;
    }

    // ใช้ FixedUpdate() สำหรับการเคลื่อนที่ที่เกี่ยวข้องกับ Physics
    private void FixedUpdate()
    {
        HandleMovement();
    }

    protected override void HandleMovement()
    {
        // ตรวจสอบว่ามี Rigidbody2D และความเร็วมากกว่า 0 หรือไม่
        if (rb == null || bearWalkSpeed <= 0) return;

        // 1. คำนวณระยะทางที่เดินได้จากจุดเริ่มต้น
        float distanceTravelled = transform.position.x - startingPosition.x;

        // 2. Logic การกลับทิศทาง (Waypoint Check)
        // Point B: ถ้าเดินไปทางขวาถึงขีดจำกัด
        if (distanceTravelled >= bearWalkDistance)
        {
            direction = -1f; // กลับไปทางซ้าย
            Flip();
        }
        // Point A: ถ้าเดินไปทางซ้ายถึงขีดจำกัด
        else if (distanceTravelled <= -bearWalkDistance)
        {
            direction = 1f; // กลับไปทางขวา
            Flip();
        }

        // 3. กำหนดความเร็วในการเคลื่อนที่
        rb.linearVelocity = new Vector2(direction * bearWalkSpeed, rb.linearVelocity.y);

        // 4. ควบคุม Animation ด้วย Bool Parameter (isWalking)
        if (anim != null)
        {
            // ถ้าหมีมีความเร็วในแนวนอน แสดงว่ากำลังเดิน
            bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
            // สั่งให้ Animator เล่น Animation เดิน (Bear-Animation)
            anim.SetBool("isWalking", isWalking);
        }
    }

    void Flip()
    {
        // กลับด้าน Sprite ด้วยการกลับค่า Scale X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // **แสดงขอบเขตการเดิน (Waypoints) ใน Scene View**
    private void OnDrawGizmos()
    {
        // ใช้ตำแหน่ง Transform ปัจจุบันหากยังไม่ได้เริ่มเล่นเกม
        Vector3 centerPosition = Application.isPlaying ?
                                 startingPosition :
                                 transform.position;

        // คำนวณจุด A (ซ้าย) และ จุด B (ขวา)
        Vector3 pointA = centerPosition - new Vector3(bearWalkDistance, 0, 0);
        Vector3 pointB = centerPosition + new Vector3(bearWalkDistance, 0, 0);

        // วาดเส้นและจุดสีน้ำเงินเพื่อแสดงขอบเขต
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pointA, 0.1f);
        Gizmos.DrawWireSphere(pointB, 0.1f);
        Gizmos.DrawLine(pointA, pointB);
    }
}