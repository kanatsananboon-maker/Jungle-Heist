using UnityEngine;

public class Eagle : Enemy // สืบทอดจาก Enemy
{
    // *** ตั้งค่าใน Inspector ***
    [Header("Attack Settings")]
    public float patrolHeight = 4f;     // ความสูงที่เหยี่ยวจะบินวนอยู่เฉยๆ
    public float attackRange = 8f;      // ระยะห่างที่ผู้เล่นเข้าใกล้แล้วเหยี่ยวจะโจมตี
    public float diveSpeed = 5f;        // ความเร็วในการพุ่งโจมตี
    public float returnSpeed = 3f;      // ความเร็วในการบินกลับ

    // *** ตัวแปรภายใน ***
    private Transform player;           // Reference ถึงตำแหน่งผู้เล่น
    private Vector3 initialPosition;    // ตำแหน่งเริ่มต้นของเหยี่ยว
    private bool isDiving = false;      // สถานะการพุ่งโจมตี

    // Override Start() ของคลาสแม่
    protected override void Start()
    {
        base.Start(); // เรียก Start() ของคลาสแม่ (เพื่อตั้งค่า anim)

        // 1. หาวัตถุผู้เล่น (สมมติว่า Player มี Tag เป็น "Player")
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 2. กำหนดตำแหน่งเริ่มต้น (ใช้เป็นตำแหน่งสำหรับบินวน)
        initialPosition = transform.position;
        // ปรับความสูงเริ่มต้นให้เป็นความสูงที่เราต้องการให้มันบินวน
        initialPosition.y = patrolHeight;
    }

    void Update()
    {
        if (player == null) return;

        // คำนวณระยะห่างระหว่างเหยี่ยวกับผู้เล่น (ใช้ Vector2.Distance สำหรับเกม 2D)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- 1. ตรวจสอบเงื่อนไขการโจมตี ---
        if (distanceToPlayer < attackRange && !isDiving)
        {
            // ผู้เล่นเข้าสู่ระยะโจมตี: เริ่มพุ่งโจมตี
            StartAttack();
        }
        else if (isDiving)
        {
            // --- 2. สถานะการพุ่งโจมตี ---
            DiveAttack();
        }
        else
        {
            // --- 3. สถานะบินวนอยู่เฉยๆ (Idle Fly) ---
            Patrol();
        }
    }

    // --- LOGIC การบินและการโจมตี ---

    private void Patrol()
    {
        // บินวนอยู่แถวตำแหน่งเริ่มต้นและความสูงที่กำหนด
        // ใช้ Mathf.Sin เพื่อทำให้มันเคลื่อนที่ขึ้นลงเล็กน้อยให้ดูเหมือนกำลังบิน
        float newY = initialPosition.y + Mathf.Sin(Time.time * 2f) * 0.5f;
        Vector3 targetPosition = new Vector3(initialPosition.x, newY, initialPosition.z);

        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, Time.deltaTime * 1f
        );

        EndAttack(); // ตรวจสอบให้แน่ใจว่าอนิเมชั่น "IsAttacking" เป็น False
    }

    public void StartAttack()
    {
        isDiving = true;

        // สั่ง Animator เล่นอนิเมชั่นพุ่งโจมตี (Dive Attack)
        if (anim != null)
        {
            anim.SetBool("IsAttacking", true);
        }
    }

    private void DiveAttack()
    {
        // เคลื่อนที่พุ่งไปยังตำแหน่งของผู้เล่น
        transform.position = Vector3.MoveTowards(
            transform.position, player.position, Time.deltaTime * diveSpeed
        );

        // ถ้าเหยี่ยวไปถึงตำแหน่งผู้เล่น (หรือใกล้มากๆ) ให้มันบินกลับ
        if (Vector2.Distance(transform.position, player.position) < 0.5f)
        {
            StopDiveAndReturn();
        }
    }

    // ฟังก์ชันสำหรับสั่งให้เหยี่ยวหยุดโจมตีและเริ่มบินกลับ
    private void StopDiveAndReturn()
    {
        isDiving = false;
        // เมื่อหยุดโจมตี ตัวเหยี่ยวจะกลับไปสู่ logic ในฟังก์ชัน Update() ที่จะเรียก Patrol()
    }


    // ฟังก์ชันนี้ถูกใช้ใน Patrol()
    public void EndAttack()
    {
        if (anim != null)
        {
            anim.SetBool("IsAttacking", false);
        }
    }
}