using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public int damageAmount = 1; // ดาเมจที่ทำได้ (ค่าเริ่มต้น 1 ดวง)

    // ทำงานตลอดเวลาที่ Player สัมผัสหรือยืนอยู่ในกล่องนี้
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}