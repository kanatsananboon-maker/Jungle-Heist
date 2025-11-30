using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // ฟังก์ชันเริ่มเกม
    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");  // เปลี่ยนชื่อ "Gameplay" เป็นชื่อ Scene ที่ต้องการเริ่มเล่น
    }

    // ฟังก์ชันออกจากเกม
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();  // ปิดเกม
    }
}
