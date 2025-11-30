using UnityEngine;

// EagleController สืบทอดจาก Enemy
public class Eagle : Enemy
{
    // *** ตั้งค่าใน Inspector ***
    [Header("Eagle Configuration")]
    public float attackRange = 8f;        // ระยะตรวจจับผู้เล่น
    public float diveSpeed = 5f;          // ความเร็วในการพุ่ง
    public float patrolSpeed = 1.5f;      // ความเร็วในการบินวน/กลับ
    public Transform player;              // กำหนด Player ใน Inspector หรือหาจาก Tag

    [Header("Damage Settings")]
    public int attackDamage = 1;          // ดาเมจที่เหยี่ยวทำต่อการชน 1 ครั้ง

    // *** ตัวแปรภายใน ***
    private Vector3 initialPatrolPoint;   // จุดเริ่มต้น/จุดบินวน

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

        // กำหนดจุดเริ่มต้นการบินวน
        initialPatrolPoint = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        // ใช้ Vector3.Distance เพื่อรองรับการทำงานทั้ง 2D และ 3D
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
        if (anim != null) anim.SetBool("IsAttacking", false);

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
            if (anim != null) anim.SetBool("IsAttacking", true); // เริ่มอนิเมชั่นพุ่ง
        }
    }

    private void HandleDiveState(float distanceToPlayer)
    {
        // 1. พุ่งเข้าหาผู้เล่น
        transform.position = Vector3.MoveTowards(
            transform.position, player.position, Time.deltaTime * diveSpeed
        );

        // 2. เงื่อนไขการหยุดพุ่ง
        bool isCloseToTarget = Vector3.Distance(transform.position, player.position) < 0.5f;
        bool playerOutOfRange = distanceToPlayer > attackRange * 1.5f;

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
        if (Vector3.Distance(transform.position, initialPatrolPoint) < 0.1f)
        {
            // กลับสู่สถานะ Patrol
            currentState = EagleState.Patrol;
        }
    }

    // **NEW: ตรวจจับการชนเพื่อทำดาเมจ**
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. ตรวจสอบว่าเหยี่ยวกำลังอยู่ในสถานะโจมตี (Dive) หรือไม่
        if (currentState == EagleState.Dive)
        {
            // 2. ลองดึง Component PlayerController จากวัตถุที่ชน
            PlayerController playerController = other.GetComponent<PlayerController>();

            // 3. ถ้าเจอ PlayerController แสดงว่าชนผู้เล่น
            if (playerController != null)
            {
                // ทำดาเมจผู้เล่น
                playerController.TakeDamage(attackDamage);

                // หลังชนแล้ว ให้เหยี่ยวบินกลับทันที
                currentState = EagleState.Return;
            }
        }
    }
}