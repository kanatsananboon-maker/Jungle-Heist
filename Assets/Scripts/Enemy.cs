using UnityEngine;

// คลาส Enemy เป็นคลาสแม่สำหรับศัตรูทั้งหมด
public class Enemy : MonoBehaviour
{
    // protected Animator เพื่อให้คลาสลูกสามารถเข้าถึงและใช้งานได้
    protected Animator anim;

    // ใช้ virtual เพื่อให้คลาสลูกสามารถ Override (เขียนทับ) ฟังก์ชันนี้ได้
    protected virtual void Start()
    {
        // ดึง Animator Component มาใช้งาน
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    // ฟังก์ชันสำหรับสั่งให้ศัตรูเล่นอนิเมชั่นบาดเจ็บ
    public virtual void TakeDamage()
    {
        if (anim != null)
        {
            // สมมติว่าใน Animator มี Trigger Parameter ชื่อ "Hurt"
            anim.SetTrigger("Hurt");
        }
        Debug.Log(gameObject.name + " took damage!");
    }

    // ฟังก์ชันสำหรับสั่งให้ศัตรูตาย
    public virtual void Die()
    {
        Debug.Log(gameObject.name + " died!");
        // โค้ดสำหรับจัดการการตาย
    }
}