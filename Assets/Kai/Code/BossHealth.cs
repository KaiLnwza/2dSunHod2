using UnityEngine;
using System;

public class BossHealth : MonoBehaviour
{
    [Header("Hp")]
    public float maxHP = 1000f;
    public float currentHP;

    [Header("Status")]
    public bool isInvincible = false; 
    public bool isDead { get; private set; } = false;

    public event Action<float, float> OnHealthChanged;
    public event Action OnBossDeath;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    public void SetInvincible(bool state)
    {
        isInvincible = state;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = state ? new Color(0.5f, 0.8f, 1f, 0.8f) : Color.white;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvincible) return;

        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (anim != null) anim.SetTrigger("Hurt");

        if (currentHP <= 0)
        {
            isDead = true;
            if (anim != null) anim.SetTrigger("Death");
            OnBossDeath?.Invoke();
            GetComponent<Collider2D>().enabled = false;
        }
    }
}