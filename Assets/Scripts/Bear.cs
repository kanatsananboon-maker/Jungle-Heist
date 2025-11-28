using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// สืบทอดจาก EnemyController (เพื่อคง Logic การชน Player)
public class Bear : Enemy
{
    [Header("Bear Movement")]
    [SerializeField] private float bearWalkSpeed = 2f;

    // สำหรับการตรวจจับขอบแพลตฟอร์มเท่านั้น
    [SerializeField] private float edgeCheckDistance = 0.5f; // ระยะยิง Raycast ลงพื้น
    [SerializeField] private LayerMask groundLayer;

    private float direction = 1f; // 1 = ขวา, -1 = ซ้าย
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>(); // หา Animator
    }

    // ใช้ FixedUpdate() สำหรับการเคลื่อนที่ที่เกี่ยวข้องกับ Physics
    private void FixedUpdate()
    {
        HandleMovement();
    }

    protected override void HandleMovement()
    {
        if (rb == null || bearWalkSpeed <= 0) return;

        // 1. **Edge Check**: ยิง Raycast ลงพื้นเพื่อตรวจจับขอบ

        // ตำแหน่งเริ่มต้น Raycast: ด้านหน้าหมีเล็กน้อยและต่ำลงเล็กน้อย
        Vector2 rayStart = rb.position + new Vector2(direction * 0.4f, 0);

        // ยิง Raycast ลงพื้น
        // ถ้า Raycast ไม่เจอ Collider ของ Ground Layer แสดงว่ากำลังจะตกขอบ
        RaycastHit2D hitEdge = Physics2D.Raycast(rayStart, Vector2.down, edgeCheckDistance, groundLayer);

        // 2. **Logic การกลับทิศทาง**
        // กลับทิศทางถ้า: Raycast ลงพื้นไม่เจออะไร (กำลังจะตกขอบ)
        if (hitEdge.collider == null)
        {
            direction *= -1; // กลับทิศทาง
            Flip();
        }

        // 3. กำหนดความเร็วในการเคลื่อนที่
        rb.linearVelocity = new Vector2(direction * bearWalkSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
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

        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        anim.speed = isWalking ? 1f : 0f;
    }

    // **แสดง Raycast ใน Scene View** (สำหรับปรับแต่ง)
    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 rayStart = rb.position + new Vector2(direction * 0.4f, 0);

            // Edge Check (แนวดิ่ง)
            Gizmos.DrawLine(rayStart, rayStart + Vector2.down * edgeCheckDistance);
        }
    }
}