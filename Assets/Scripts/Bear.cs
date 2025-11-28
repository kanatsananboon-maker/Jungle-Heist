using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// สืบทอดจาก EnemyController (เพื่อคง Logic การชน Player และการกำหนด rb)
public class BearController : Enemy
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
        base.Start(); // **สำคัญ:** เรียก Start ของ EnemyController เพื่อกำหนด rb
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
        // **สำคัญ:** rb.velocity.y ถูกปล่อยให้ Rigidbody 2D และ Gravity จัดการ
        rb.linearVelocity = new Vector2(direction * bearWalkSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        // กลับด้าน Sprite ด้วยการกลับค่า Scale X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // **Animation Logic (ใช้ Speed)**
    private void LateUpdate()
    {
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            if (anim == null) return;
        }

        // Animation จะเล่นเมื่อความเร็วในแนวนอนมากกว่า 0.01 (กำลังเดิน)
        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.speed = isWalking ? 1f : 0f;
    }

    // **แสดงขอบเขตการเดิน (Waypoints) ใน Scene View**
    private void OnDrawGizmos()
    {
        // Gizmo จะแสดงเมื่อไม่ได้กด Play เท่านั้น
        if (!Application.isPlaying && startingPosition != Vector3.zero)
        {
            // คำนวณจุด A (ซ้าย) และ จุด B (ขวา)
            Vector3 pointA = startingPosition - new Vector3(bearWalkDistance, 0, 0);
            Vector3 pointB = startingPosition + new Vector3(bearWalkDistance, 0, 0);

            // วาดเส้นและจุดสีน้ำเงินเพื่อแสดงขอบเขต
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pointA, 0.1f);
            Gizmos.DrawWireSphere(pointB, 0.1f);
            Gizmos.DrawLine(pointA, pointB);
        }
        else if (Application.isPlaying)
        {
            // ถ้ากำลังเล่นเกม ให้ใช้ตำแหน่งปัจจุบันเพื่อแสดง Waypoint
            Vector3 currentCenter = transform.position - new Vector3(transform.position.x - startingPosition.x, 0, 0);
            Vector3 pointA = currentCenter - new Vector3(bearWalkDistance, 0, 0);
            Vector3 pointB = currentCenter + new Vector3(bearWalkDistance, 0, 0);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pointA, 0.1f);
            Gizmos.DrawWireSphere(pointB, 0.1f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }
}