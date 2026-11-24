using UnityEngine;

public class Skill_lava_Damage : MonoBehaviour
{
    public int damagePerTick = 10;     
    public float damageInterval = 0.5f; 
    private float damageTimer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage(other);
            damageTimer = damageInterval;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                DealDamage(other);
                damageTimer = damageInterval;
            }
        }
    }

    void DealDamage(Collider2D player)
    {
        Debug.Log("ผู้เล่นโดนลาวาลวก -" + damagePerTick);
        player.GetComponent<PlayerHealth>()?.TakeDamage(1); 
    }
}