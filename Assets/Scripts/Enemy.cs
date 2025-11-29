using UnityEngine;
using System.Collections; // เพื่อใช้ Coroutine

public abstract class Enemy : MonoBehaviour
{
    // **ต้องมี Abstract Method นี้**
    protected abstract void SetAnimation(bool walking);

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolSpeed = 2f;
    [SerializeField] protected float waitTime = 2f;
    [SerializeField] protected Transform pointA; // จุดเริ่มต้น
    [SerializeField] protected Transform pointB; // จุดสิ้นสุด

    // **ตัวแปรสำหรับ Ground/Wall Check (ใหม่)**
    [Header("Collision Check")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 0.2f;
    [SerializeField] protected LayerMask groundLayer;

    // ตัวแปรภายใน
    protected Transform currentTarget;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected bool isWalking = true;

    protected virtual void Start()
    {
        // ตรวจสอบและตั้งค่า Component ที่จำเป็น
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Point A or Point B is not set in the inspector for " + gameObject.name);
            enabled = false; // ปิด Script ถ้าไม่มีจุด Patrol
            return;
        }

        // ตั้งเป้าหมายเริ่มต้น
        currentTarget = pointA;
        transform.position = currentTarget.position; // เริ่มที่จุด A
    }

    protected virtual void Update()
    {
        if (isWalking)
        {
            PatrolMove();
        }
        else
        {
            // ถ้าหยุดเดิน จะเริ่มนับเวลาถอยหลังเพื่อเดินต่อ
            StartCoroutine(HandleWaitTime());
        }
    }

    protected void PatrolMove()
    {
        // 1. คำนวณทิศทางไปยังเป้าหมาย
        Vector2 targetPosition = currentTarget.position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        // 2. **LOGIC ใหม่: ตรวจสอบกำแพงและขอบเหว**
        // ใช้ LocalScale.x เพื่อให้ Raycast ยิงไปในทิศทางที่ศัตรูกำลังหัน (ซ้ายหรือขวา)
        float direction = Mathf.Sign(transform.localScale.x);

        // Raycast ตรวจสอบพื้นด้านล่าง (เพื่อดูขอบเหว)
        bool isAtEdge = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        // Raycast ตรวจสอบกำแพงด้านหน้า (เพื่อดูสิ่งกีดขวาง)
        bool isHittingWall = Physics2D.Raycast(groundCheck.position, Vector2.right * direction, 0.2f, groundLayer);

        // 3. พลิกตัวตามทิศทางการเดิน
        if (moveDirection.x > 0.01f) // เดินขวา
        {
            Flip(1);
        }
        else if (moveDirection.x < -0.01f) // เดินซ้าย
        {
            Flip(-1);
        }

        // 4. ถ้าถึงขอบเหว หรือชนกำแพง ให้สลับเป้าหมายทันที
        if (isAtEdge || isHittingWall)
        {
            // บังคับสลับเป้าหมายและหยุดชั่วคราว
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            if (rb != null) rb.linearVelocity = Vector2.zero;
            isWalking = false;
            return; // หยุดการเคลื่อนที่ในเฟรมนี้
        }

        // 5. เคลื่อนที่ตามปกติ (เฉพาะเมื่อไม่ตกเหว/ชนกำแพง)
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * patrolSpeed, rb.linearVelocity.y);
        }

        // 6. ตรวจสอบว่าถึงจุดหมาย Patrol แล้วหรือไม่ (Logic เดิม)
        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            isWalking = false;
        }
    }

    protected IEnumerator HandleWaitTime()
    {
        // หยุดเดินและรอตามเวลาที่กำหนด
        if (rb != null) rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);

        // สลับเป้าหมายหลังจากรอ
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        isWalking = true;
    }

    protected void Flip(int direction)
    {
        // พลิกตัวศัตรูโดยการเปลี่ยน Scale.x
        if (transform.localScale.x != direction)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction;
            transform.localScale = scale;
        }
    }

    // **แสดง Raycast ใน Scene View เพื่อการ Debug**
    protected virtual void OnDrawGizmos()
    {
        if (pointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
        }

        if (pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // แสดง Ground/Wall Check (Gizmos ใหม่)
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            // Raycast ตรวจสอบพื้นด้านล่าง (ขอบเหว)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

            // Raycast ตรวจสอบกำแพงด้านหน้า (ตามทิศทางที่ Dino หัน)
            float direction = (transform.localScale.x > 0) ? 1f : -1f;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.right * direction * 0.2f);
        }
    }
}