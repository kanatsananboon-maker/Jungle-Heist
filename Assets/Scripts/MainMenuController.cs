using UnityEngine;
using UnityEngine.SceneManagement; // <<-- ต้องมี SceneManagement

public class MainMenuController : MonoBehaviour
{
    // [ตั้งค่าใน Inspector] ให้ใส่ชื่อ Scene ของเกมหลัก (ตาม Build Settings)
    // จากภาพที่เคยส่งมา ชื่อ Scene เกมของคุณคือ SampleScene
    [SerializeField] private string levelSceneName = "SampleScene";


    // ------------------------------------------
    // 1. ฟังก์ชันสำหรับการเริ่มเล่นเกม
    // ------------------------------------------
    public void StartGame()
    {
        // โหลด Scene เกมหลัก
        SceneManager.LoadScene(levelSceneName);
    }

    // ------------------------------------------
    // 2. ฟังก์ชันสำหรับการออกจากเกม
    // ------------------------------------------
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

        // สำหรับใช้งานจริงหลัง Build (บน PC, Mac, ฯลฯ)
        Application.Quit();

        // สำหรับการทดสอบใน Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}