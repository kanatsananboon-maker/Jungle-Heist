using UnityEngine;

// สืบทอดจาก Enemy เพื่อใช้ Animator และฟังก์ชัน TakeDamage
public class Eagle : Enemy
{
    [Header("Eagle Configuration")]
    public float attackRange = 8f;        // ระยะตรวจจับผู้เล่น
    public float diveSpeed = 5f;          // ความเร็วในการพุ่ง
    public float patrolSpeed = 1.5f;      // ความเร็วในการบินวน/กลับ
    public Transform player;              // กำหนด Player ใน Inspector หรือหาจาก Tag

    private Vector3 initialPatrolPoint;   // จุดเริ่มต้น/จุดบินวน
    private bool isAttacking = false;     // สถานะการโจมตี

    // สถานะที่เราจะใช้ในการควบคุม
    private enum EagleState { Patrol, Dive, Return }
    private EagleState currentState = EagleState.Patrol;

    protected override void Start()
    {
        base.Start(); // เรียก Start() ของ Enemy เพื่อตั้งค่า anim

        // ถ้าไม่ได้กำหนด player ใน Inspector ให้ลองค้นหาจาก Tag "Player"
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
        initialPatrolPoint = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EagleState.Patrol:
                HandlePatrolState(distanceToPlayer);
                break;

            case EagleState.Dive:
                HandleDiveState(distanceToPlayer);
                break;

            case EagleState.Return:
                HandleReturnState();
                break;
        }
    }

    // --- State Handlers ---

    private void HandlePatrolState(float distanceToPlayer)
    {
        // 1. บินวน/ลอยตัว (Patrol Animation - IsAttacking = false)
        anim.SetBool("IsAttacking", false);

        // เคลื่อนที่ขึ้นลงเบาๆ ที่จุดเริ่มต้น
        float newY = initialPatrolPoint.y + Mathf.Sin(Time.time * 2f) * 0.5f;
        Vector3 targetPosition = new Vector3(initialPatrolPoint.x, newY, initialPatrolPoint.z);

        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, Time.deltaTime * patrolSpeed
        );

        // 2. ตรวจจับผู้เล่น
        if (distanceToPlayer < attackRange)
        {
            // เปลี่ยนสถานะเป็น Dive เพื่อโจมตี
            currentState = EagleState.Dive;
            anim.SetBool("IsAttacking", true); // เริ่มอนิเมชั่นพุ่ง
        }
    }

    private void HandleDiveState(float distanceToPlayer)
    {
        // 1. พุ่งเข้าหาผู้เล่น
        transform.position = Vector3.MoveTowards(
            transform.position, player.position, Time.deltaTime * diveSpeed
        );

        // 2. เงื่อนไขการหยุดพุ่ง
        bool isCloseToTarget = Vector2.Distance(transform.position, player.position) < 0.5f;
        bool playerOutOfRange = distanceToPlayer > attackRange * 1.5f; // ให้ระยะไกลกว่าปกติเล็กน้อย

        if (isCloseToTarget || playerOutOfRange)
        {
            // เปลี่ยนสถานะเป็น Return เพื่อบินกลับ
            currentState = EagleState.Return;
        }
    }

    private void HandleReturnState()
    {
        // 1. บินกลับไปที่จุดเริ่มต้น (initialPatrolPoint)
        transform.position = Vector3.MoveTowards(
            transform.position, initialPatrolPoint, Time.deltaTime * patrolSpeed
        );

        // 2. เมื่อถึงจุดเริ่มต้นแล้ว
        if (Vector2.Distance(transform.position, initialPatrolPoint) < 0.1f)
        {
            // กลับสู่สถานะ Patrol
            currentState = EagleState.Patrol;
        }
    }
}