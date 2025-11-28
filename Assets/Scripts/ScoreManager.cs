using UnityEngine;
// ต้องมี using TMPro; ถ้าคุณใช้ TextMeshPro ใน UI
// using TMPro; 

public class ScoreManager : MonoBehaviour
{
    // Encapsulation: ใช้ private เพื่อห่อหุ้มตัวแปรคะแนน
    private int score = 0;

    // public TextMeshProUGUI scoreText; // ถ้าใช้ TextMeshPro

    // Encapsulation: Public Method สำหรับการอ่านค่าคะแนน (Getter)
    public int GetScore()
    {
        return score;
    }

    // Encapsulation: Public Method สำหรับการเพิ่มคะแนน (Setter/Modifier)
    // โค้ดอื่นๆ จะเรียกใช้ Method นี้เท่านั้น เพื่อเพิ่มคะแนน
    public void AddScore(int pointsToAdd)
    {
        if (pointsToAdd > 0)
        {
            score += pointsToAdd;
            Debug.Log("Score updated to: " + score);
            // UpdateScoreUI(); // เรียก Update UI ถ้ามี
        }
    }

    // หากมี UI
    /*
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score.ToString();
        }
    }
    */
}