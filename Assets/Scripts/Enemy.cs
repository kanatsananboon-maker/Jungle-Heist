using UnityEngine;

// กำหนดให้เป็น Abstract เพื่อป้องกันการนำไปใส่ GameObject โดยตรง
public abstract class Enemy : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] protected float patrolSpeed = 2f;
    [SerializeField] protected float waitTime = 1f;

    // จุดเริ่มต้นและจุดสิ้นสุดของการลาดตระเวน (ลาก Transform จาก Scene มาใส่)
    public Transform pointA;
    public Transform pointB;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform currentTarget;
    protected float timer;
    protected bool isWalking = true;

    // Abstract Method ที่ต้องถูก implement ในคลาสลูก
    protected abstract void SetAnimation(bool walking);

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (pointA == null || pointB == null)
        {
            Debug.LogError("Enemy points A and B are not assigned. Please assign them in the Inspector.");
            enabled = false;
            return;
        }

        currentTarget = pointB;
        timer = waitTime;

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected virtual void Update()
    {
        SetAnimation(isWalking); // เรียกใช้ Animation จากคลาสลูก

        if (isWalking)
        {
            PatrolMove();
        }
        else
        {
            HandleWaitTime();
        }
    }

    protected void PatrolMove()
    {
        Vector2 targetPosition = currentTarget.position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * patrolSpeed, rb.linearVelocity.y);
        }

        // 3. พลิกตัวตามทิศทางการเดิน
        if (moveDirection.x > 0.01f) // เดินขวา
        {
            Flip(1);
        }
        else if (moveDirection.x < -0.01f) // เดินซ้าย
        {
            Flip(-1);
        }

        // 4. ตรวจสอบว่าถึงจุดหมายแล้วหรือไม่
        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            isWalking = false;
        }
    }

    protected void HandleWaitTime()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // สลับเป้าหมาย
            currentTarget = (currentTarget == pointA) ? pointB : pointA;

            // เริ่มเดินอีกครั้ง
            isWalking = true;
            timer = waitTime;
        }
    }

    protected void Flip(int direction)
    {
        Vector3 scale = transform.localScale;

        if ((direction > 0 && scale.x < 0) || (direction < 0 && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}