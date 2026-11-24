using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;
    public float invincibilityDuration = 1f;

    [Header("Heart UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Animation")]
    public Animator animator; // ช่องใส่ Animator ของ SpriteBody

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
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isDead || isInvincible || (playerMovement != null && playerMovement.IsDashing()))
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(1);
    }

    public void Heal(int amount = 1)
    {
        if (isDead) return;
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    // เพิ่มฟังก์ชันนี้ต่อท้าย TakeDamage(int damage = 1) ในไฟล์ PlayerHealth.cs
    public void TakeDamage(float damage)
    {
        // แปลงดาเมจจากบอส (เช่น 15f) ให้ลดเลือด Player 1 ดวง
        TakeDamage(1);
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

        // 1. สั่งเล่นอนิเมชั่นตาย
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 2. หยุดการเคลื่อนที่และปิดสคริปต์ควบคุมตัวละคร
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }
}