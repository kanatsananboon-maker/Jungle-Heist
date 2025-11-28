using UnityEngine;

// 1. Abstract Class: Base Class สำหรับศัตรูทั้งหมด
// ต้องสืบทอดจาก MonoBehaviour เพื่อให้ใช้งานใน Unity ได้
public abstract class Enemy : MonoBehaviour
{
    // Encapsulation: ตัวแปรพื้นฐานที่ศัตรูทุกตัวต้องมี
    [Header("Enemy Base Stats")]
    [SerializeField] protected int maxHealth = 1; // พลังชีวิตเริ่มต้น 1
    protected int currentHealth;

    [Header("Stomp Interaction")]
    [SerializeField] protected float stompForce = 5f; // แรงผลัก Player เมื่อเหยียบ
    [SerializeField] protected float stompCheckOffset = 0.5f; // ชดเชยตำแหน่งในการเช็คเหยียบ

    protected Rigidbody2D rb;

    // Abstract Method: การเคลื่อนที่ (บังคับให้ Subclass ต้อง Implement)
    protected abstract void HandleMovement();

    // Virtual Method: การรับความเสียหาย (Subclass สามารถ Override ได้)
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        HandleMovement();
    }

    // Method สำหรับการรับความเสียหาย
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage. Health remaining: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method สำหรับการตาย (Subclass สามารถ Override เพื่อเพิ่มลูกเล่นได้)
    protected virtual void Die()
    {
        Destroy(gameObject);
        Debug.Log(gameObject.name + " destroyed!");
    }

    // 2. การตรวจจับการชน (Collision Logic - Polymorphism)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player == null) return;

            Vector2 enemyCenter = rb.position;
            // เช็คว่า Player กระโดดเหยียบจากด้านบนจริง
            bool isStomped = collision.GetContact(0).point.y > (enemyCenter.y + stompCheckOffset);

            if (isStomped)
            {
                // ถ้าเหยียบจากด้านบน:
                TakeDamage(1);

                // กระเด้ง Player ขึ้นไป
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompForce);
                }
            }
            else
            {
                // ถ้าชนจากด้านข้าง:
                player.DieAndRespawn();
            }
        }
    }
}