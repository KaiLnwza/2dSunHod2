using UnityEngine;

public class GhostProjectile : MonoBehaviour
{
    [Header("ตั้งค่าความเร็วและดาเมจ")]
    public float aimDuration = 0.8f;    
    public float dashSpeed = 14f;       
    public float damage = 15f;
    public float lifeTime = 5f;         

    private Transform player;
    private Vector2 lockedDirection;
    private float timer = 0f;
    private bool isDashing = false;
    private bool hasHitPlayer = false;

    public void Initialize(Transform targetPlayer)
    {
        player = targetPlayer;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isDashing)
        {
            timer += Time.deltaTime;
            if (player != null)
            {
                lockedDirection = (player.position - transform.position).normalized;
                RotateTowards(lockedDirection);
            }

            if (timer >= aimDuration)
            {
                isDashing = true; 
            }
        }
        else
        {
            transform.position += (Vector3)(lockedDirection * dashSpeed * Time.deltaTime);
        }
    }

    void RotateTowards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasHitPlayer && other.CompareTag("Player"))
        {
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            hasHitPlayer = true; 
        }
    }
}