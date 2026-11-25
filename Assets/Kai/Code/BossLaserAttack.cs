using UnityEngine;
using System.Collections;

public class BossLaserAttack : MonoBehaviour
{
    [Header("Attack Point")]
    public Transform laserOrigin;
    public Transform player;
    public LineRenderer lineRenderer;
    private BossHealth bossHealth;

    [Header("Charge")]
    public float minFireInterval = 4f;
    public float maxFireInterval = 6f;
    public float aimDuration = 1.5f;
    public float lockDuration = 0.35f;
    public float fireDuration = 0.4f;

    [Header("Damage")]
    public float laserLength = 25f;
    public float aimWidth = 0.05f;
    public float fireWidth = 0.6f;
    public Color aimColor = new Color(1f, 0f, 0f, 0.45f);
    public Color lockColor = new Color(1f, 0.8f, 0f, 0.9f);
    public Color fireColor = new Color(1f, 0.1f, 0.1f, 1f);
    public float damage = 15f;
    public LayerMask targetLayer;

    private float cooldownTimer;
    private bool isFiring = false;

    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        if (lineRenderer != null) lineRenderer.enabled = false;
        cooldownTimer = Random.Range(minFireInterval, maxFireInterval);
    }

    void Update()
    {
        if ((bossHealth != null && bossHealth.isDead) || isFiring || player == null) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            StartCoroutine(LaserSequence());
            cooldownTimer = Random.Range(minFireInterval, maxFireInterval);
        }
    }

    IEnumerator LaserSequence()
    {
        isFiring = true;
        lineRenderer.enabled = true;
        Vector2 lockedDirection = Vector2.right;

        lineRenderer.startWidth = aimWidth;
        lineRenderer.endWidth = aimWidth;
        lineRenderer.startColor = aimColor;
        lineRenderer.endColor = aimColor;

        float timer = 0f;
        while (timer < aimDuration)
        {
            if (bossHealth != null && bossHealth.isDead) { CancelLaser(); yield break; }

            lockedDirection = (player.position - laserOrigin.position).normalized;
            UpdateLaserVisual(laserOrigin.position, lockedDirection);

            timer += Time.deltaTime;
            yield return null;
        }

        lineRenderer.startColor = lockColor;
        lineRenderer.endColor = lockColor;

        timer = 0f;
        while (timer < lockDuration)
        {
            if (bossHealth != null && bossHealth.isDead) { CancelLaser(); yield break; }

            UpdateLaserVisual(laserOrigin.position, lockedDirection);

            timer += Time.deltaTime;
            yield return null;
        }

        lineRenderer.startWidth = fireWidth;
        lineRenderer.endWidth = fireWidth;
        lineRenderer.startColor = fireColor;
        lineRenderer.endColor = fireColor;

        // --- [เพิ่มเสียงยิงเลเซอร์] ดังตรงจังหวะลำแสงใหญ่ยิงออกพอดี ---
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.bossLaserSound);

        bool hasHitPlayer = false;
        timer = 0f;
        while (timer < fireDuration)
        {
            if (bossHealth != null && bossHealth.isDead) { CancelLaser(); yield break; }

            Vector2 startPos = laserOrigin.position;
            UpdateLaserVisual(startPos, lockedDirection);

            if (!hasHitPlayer)
            {
                RaycastHit2D hit = Physics2D.CircleCast(startPos, fireWidth * 0.4f, lockedDirection, laserLength, targetLayer);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    hit.collider.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                    hasHitPlayer = true;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        CancelLaser();
    }

    void UpdateLaserVisual(Vector2 startPos, Vector2 direction)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos + (direction * laserLength));
    }

    void CancelLaser()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        isFiring = false;
    }
}