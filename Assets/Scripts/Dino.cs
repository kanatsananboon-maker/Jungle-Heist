using UnityEngine;

// Dino สืบทอด (Inherits) มาจาก Enemy
public class Dino : Enemy
{
    // เราไม่จำเป็นต้องเขียน Start() หรือ Update() ซ้ำ
    // เพราะมันจะถูกเรียกจาก Base Class (Enemy) อัตโนมัติ (ผ่าน protected virtual)

    // Method นี้ถูกบังคับให้ implement จาก Abstract method ใน Base Class (Enemy)
    protected override void SetAnimation(bool walking)
    {
        if (anim != null)
        {
            // ใช้ Bool Parameter ชื่อ "InWalking" ที่คุณตั้งไว้
            anim.SetBool("InWalking", walking);
        }
    }

    // ตัวอย่าง: การเพิ่ม Logic เฉพาะของ Dino เมื่อชน Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ... Logic การโจมตี Player
            // collision.gameObject.GetComponent<PlayerController>().DieAndRespawn(); 
        }
    }
}