using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // ตรวจสอบความถูกต้องของ Collider: ต้องเป็น Trigger
    void Start()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null || !myCollider.isTrigger)
        {
            Debug.LogError("DeathZone Collider must be attached and set to Is Trigger!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าวัตถุที่ชนคือ Player หรือไม่
        if (other.CompareTag("Player"))
        {
            // เรียก PlayerController เพื่อจัดการการตายและการเกิดใหม่
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.DieAndRespawn(); // เรียก Method ที่เราเพิ่มใน PlayerController
            }
        }
    }
}