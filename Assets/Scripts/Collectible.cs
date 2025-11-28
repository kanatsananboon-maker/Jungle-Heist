using UnityEngine;

// Abstract Class สำหรับไอเท็มที่เก็บได้ทั้งหมด
public abstract class CollectibleBase : MonoBehaviour
{
    // Encapsulation: ให้ Subclass เข้าถึงได้
    [SerializeField] protected int scoreValue = 1; // คะแนนที่จะได้จากการเก็บ

    // Abstract Method: Subclass ต้องกำหนดพฤติกรรมการเก็บเอง
    protected abstract void OnCollect(GameObject collector);

    // ถูกเรียกเมื่อวัตถุที่เป็น Trigger ชนกัน (เพชรควรเป็น Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าวัตถุที่ชนคือ Player หรือไม่
        if (other.CompareTag("Player"))
        {
            OnCollect(other.gameObject);
        }
    }
}

// Subclass สำหรับเพชร (Item ชนิดแรก)
public class Collectible : CollectibleBase
{
    // Polymorphism: Implement OnCollect ตามพฤติกรรมของเพชร
    protected override void OnCollect(GameObject collector)
    {
        // 1. หา ScoreManager และเพิ่มคะแนน
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(scoreValue);
        }

        // 2. ทำลายเพชร (เก็บได้แล้ว)
        Destroy(gameObject);

        // *คุณสามารถเพิ่มเสียงเก็บไอเท็ม หรือ Particle Effect ตรงนี้ได้*
    }
}