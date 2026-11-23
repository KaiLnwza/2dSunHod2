using System.Collections;
using UnityEngine;

public class BombObject : MonoBehaviour
{
    [Header("ตั้งค่าเวลาและดาเมจ")]
    public float warningDuration = 1.2f; 
    public int damage = 15;
    public LayerMask playerLayer;        

    [Header("เอฟเฟกต์ตอนระเบิด")]
    public GameObject explosionVFX;

    private SpriteRenderer sr;
    private float explosionRadius;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        explosionRadius = transform.localScale.x * 0.5f;

        StartCoroutine(WarningAndExplode());
    }

    IEnumerator WarningAndExplode()
    {
        float elapsed = 0f;
        Color warnColor = new Color(1f, 0.2f, 0.2f, 0.45f);

        while (elapsed < warningDuration)
        {
            elapsed += Time.deltaTime;
            float pulse = Mathf.PingPong(elapsed * 8f, 0.4f);
            if (sr != null)
            {
                sr.color = new Color(warnColor.r, warnColor.g, warnColor.b, 0.3f + pulse);
            }
            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 0.2f);
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, explosionRadius, playerLayer);
        if (hitPlayer != null)
        {
            Debug.Log("ผู้เล่นโดนระเบิด! -" + damage);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    }
}