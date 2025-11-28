using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // ตัวแปรพื้นฐานที่ศัตรูทุกตัวต้องมี
    [Header("Enemy Base Stats")]
    [SerializeField] protected int maxHealth = 1;
    protected int currentHealth;

    [Header("Stomp Interaction")]
    [SerializeField] protected float stompForce = 5f;
    [SerializeField] protected float stompCheckOffset = 0.5f;

    // **Protected Rigidbody 2D**
    protected Rigidbody2D rb;

    // Abstract Method: การเคลื่อนที่ (บังคับให้ Subclass ต้อง Implement)
    protected abstract void HandleMovement();

    // Virtual Method: Start
    protected virtual void Start()
    {
        // **สำคัญ: บังคับให้หา Rigidbody 2D ตรงนี้**
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Enemy is missing Rigidbody2D component! Movement will fail.");
        }

        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        // เราจะไม่เรียก HandleMovement() ใน Update/FixedUpdate ของ Base Class
        // ให้ Subclass (BearController) เป็นผู้เรียกเองเพื่อความยืดหยุ่น
    }

    // ... (TakeDamage, Die และ OnCollisionEnter2D เหมือนเดิม)
    // ... (ส่วนที่เหลือของโค้ด EnemyController.cs)

    // หมายเหตุ: โค้ดส่วน OnCollisionEnter2D ต้องใช้ rb เพื่อหาตำแหน่ง
}