using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Bear : Enemy
{
    [Header("Bear Movement")]
    [SerializeField] private float bearWalkSpeed = 2f;

    // **MODIFIED: สำหรับการตรวจจับขอบแพลตฟอร์มเท่านั้น**
    [SerializeField] private float checkDistance = 0.05f; // ระยะยิง Raycast ลงพื้น (สั้นมาก)
    [SerializeField] private float horizontalCheckOffset = 0.45f; // ระยะยื่น Raycast ออกจากตัวหมี
    [SerializeField] private LayerMask groundLayer;

    private float direction = 1f;
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    protected override void HandleMovement()
    {
        if (rb == null || bearWalkSpeed <= 0) return;

        // 1. **Edge Check**: ตรวจจับว่าขอบของ Collider ของหมี (ด้านหน้า) มีพื้นอยู่ใต้เท้าหรือไม่

        // ตำแหน่งเริ่มต้น Raycast: ด้านหน้าหมีเล็กน้อย (ใช้ horizontalCheckOffset) และอยู่ต่ำกว่าจุดศูนย์กลางเล็กน้อย (-0.1f)
        Vector2 rayStart = rb.position + new Vector2(direction * horizontalCheckOffset, -0.1f);

        // ยิง Raycast ลงพื้นด้วยระยะทางที่สั้นมาก (checkDistance)
        // เพื่อให้แน่ใจว่ามันตรวจสอบ "ใต้เท้า" ที่ขอบพอดี
        RaycastHit2D hitEdge = Physics2D.Raycast(rayStart, Vector2.down, checkDistance, groundLayer);

        // 2. **Logic การกลับทิศทาง**
        // กลับทิศทางถ้า: Raycast ลงพื้นไม่เจออะไร (hitEdge.collider == null)
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

    // แสดง Raycast ใน Scene View (สำหรับปรับแต่ง)
    private void OnDrawGizmos()
    {
        // ตรวจสอบ rb ไม่ให้เป็น null ก่อน
        if (Application.isPlaying && rb != null)
        {
            Gizmos.color = Color.yellow;
            // ตำแหน่งเริ่มต้น Raycast ที่โค้ดใช้
            Vector2 rayStart = rb.position + new Vector2(direction * horizontalCheckOffset, -0.1f);

            // Edge Check (แนวดิ่ง)
            Gizmos.DrawLine(rayStart, rayStart + Vector2.down * checkDistance);
        }
    }
}