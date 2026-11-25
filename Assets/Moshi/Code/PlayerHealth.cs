using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;
    public float invincibilityDuration = 1f;

    [Header("Auto Heal on Hit (ตีครบ 4 ครั้ง รีเลือด 1 ดวง)")]
    public int hitsRequiredToHeal = 4;
    public int currentHitCount = 0;

    [Header("Heart UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Animation")]
    public Animator animator;

    private bool isInvincible = false;
    private bool isDead = false;
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        if (hearts == null || hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
            }
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจาก PlayerMovement ทุกครั้งที่ฟันโดนบอส
    public void RegisterBossHit()
    {
        if (isDead) return;

        currentHitCount++;
        Debug.Log($"ตีบอสโดนสะสม: {currentHitCount}/{hitsRequiredToHeal} ครั้ง");

        if (currentHitCount >= hitsRequiredToHeal)
        {
            currentHitCount = 0; // <-- บรรทัดนี้บังคับรีเซ็ตกลับเป็น 0 เพื่อเริ่มนับรอบใหม่
            Heal(1);             // <-- บวกเลือด +1 HP
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDashing = (playerMovement != null && playerMovement.IsDashing());
        if (isDead || isInvincible || isDashing) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // --- [เพิ่มเสียงโดนตี] ---
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.getHitSound);

        Debug.Log($"<color=red>Player โดนตี!</color> เลือดเหลือ {currentHealth}/{maxHealth}");
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(1);
    }

    public void Heal(int amount = 1)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log($"<color=green>รีเลือด +1 HP!</color> เลือดปัจจุบัน: {currentHealth}/{maxHealth}");
        UpdateHeartsUI();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("<color=red>Player ตายแล้ว!</color>");

        // --- [เพิ่มเสียงแพ้ + หยุดเพลง BGM] ---
        if (SoundManager.Instance != null) SoundManager.Instance.PlayEndGame(SoundManager.Instance.loseSound);

        if (animator != null) animator.SetTrigger("Die");
        if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (playerMovement != null) playerMovement.enabled = false;

        // แจ้ง GameManager ให้รอท่าตายจบแล้วขึ้นฉากแพ้ + หยุดเกม
        GameManager.instance?.TriggerLose();
    }
}