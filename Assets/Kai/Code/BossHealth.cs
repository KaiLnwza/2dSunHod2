using UnityEngine;
using System;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("HP Settings")]
    public float maxHP = 1000f;
    public float currentHP;

    [Header("Status")]
    public bool isInvincible = false;
    public bool isDead { get; private set; } = false;

    [Header("Death Settings")]
    [Tooltip("หน่วงเวลากี่วินาทีก่อนลบตัวบอสออกจากฉาก (ตั้งให้ตรงกับความยาวอนิเมชั่น Death)")]
    public float destroyDelay = 2.5f;
    [Tooltip("ลากวัตถุในฉากที่ต้องการให้ปิดการทำงานทันทีเมื่อบอสตาย เช่น LavaMoving")]
    public GameObject[] objectsToDisableOnDeath;

    [Header("Hit Visual Feedback")]
    public Color hitFlashColor = new Color(1f, 0.35f, 0.35f, 1f);
    public Color invincibleColor = new Color(0.5f, 0.8f, 1f, 0.8f);
    public float hitFlashDuration = 0.1f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnBossDeath;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    public void SetInvincible(bool state)
    {
        if (isDead) return;

        isInvincible = state;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = state ? invincibleColor : Color.white;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        currentHP = Mathf.Clamp(currentHP - damage, 0f, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0f)
        {
            Die();
            return;
        }

        // เล่นอนิเมชั่นเจ็บและกระพริบสีแดง
        if (anim != null)
        {
            anim.SetTrigger("Hurt");
        }

        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(HitFlashRoutine());
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);

        if (!isDead)
        {
            spriteRenderer.color = isInvincible ? invincibleColor : Color.white;
        }
        flashCoroutine = null;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. คืนสีปกติและสั่งเล่นอนิเมชั่นตาย
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (anim != null)
        {
            anim.ResetTrigger("Hurt");
            anim.SetTrigger("Death");
        }

        // 2. แจ้งเตือนระบบอื่นๆ (เช่น UI หลอดเลือดบอส, ระบบชนะเกม)
        OnBossDeath?.Invoke();

        // 3. ปิด Collider เพื่อไม่ให้โดนตีซ้ำหรือชนผู้เล่น
        if (TryGetComponent<Collider2D>(out var col))
        {
            col.enabled = false;
        }

        // 4. ปิดเส้นเลเซอร์ (ถ้ากำลังยิงค้างอยู่ตอนตายพอดี)
        if (TryGetComponent<LineRenderer>(out var line))
        {
            line.enabled = false;
        }

        // 5. หยุด Coroutine และปิดสคริปต์ควบคุมทั้งหมดบนตัวบอส
        StopAllScriptsOnBoss();

        // 6. ปิดวัตถุเสริมในฉาก เช่น ลาวา
        if (objectsToDisableOnDeath != null)
        {
            foreach (GameObject obj in objectsToDisableOnDeath)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // 7. ลบตัวบอสออกจากฉากหลังเล่นท่าตายจบ
        if (destroyDelay >= 0f)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void StopAllScriptsOnBoss()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            // ปิดทุกสคริปต์บนตัวบอส ยกเว้นตัว BossHealth เอง
            if (script != null && script != this)
            {
                script.StopAllCoroutines();
                script.enabled = false;
            }
        }
    }
}