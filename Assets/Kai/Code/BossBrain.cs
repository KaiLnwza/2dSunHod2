using UnityEngine;

[RequireComponent(typeof(BossHealth), typeof(BossMovement), typeof(Animator))]
public class BossBrain : MonoBehaviour
{
    [Header("Cooldown Base Attack")]
    public float minNormalCooldown = 5f;
    public float maxNormalCooldown = 8f;

    [Header("Cooldown Lava")]
    public float minLavaCooldown = 2f;
    public float maxLavaCooldown = 4f;

    [HideInInspector] public bool isLavaActive = false;
    [HideInInspector] public bool isPortalActive = false; 

    private BossHealth health;
    private BossMovement movement;
    private Animator anim;
    private float actionTimer;

    void Awake()
    {
        health = GetComponent<BossHealth>();
        movement = GetComponent<BossMovement>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        movement.ChangePattern(BossMovement.MovePattern.CustomPathLoop);
        ResetCooldown();
    }

    void Update()
    {
        if (health.isDead || isPortalActive) return;

        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0f)
        {
            ChooseMainSkill();
            ResetCooldown();
        }
    }

    void ResetCooldown()
    {
        if (isLavaActive)
            actionTimer = Random.Range(minLavaCooldown, maxLavaCooldown);
        else
            actionTimer = Random.Range(minNormalCooldown, maxNormalCooldown);
    }

    void ChooseMainSkill()
    {
        int skillIndex = isLavaActive ? Random.Range(1, 5) : Random.Range(1, 5);
        while (isLavaActive && skillIndex == 2)
        {
            skillIndex = Random.Range(1, 5);
        }

        switch (skillIndex)
        {
            case 1:
                Debug.Log("1Skill Summon");
                anim.SetTrigger("Buff");
                break;
            case 2:
                Debug.Log("2Skill lavaFloor");
                anim.SetTrigger("Buff");
                break;
            case 3:
                Debug.Log("3SKill Bome");
                anim.SetTrigger("Attack1");
                break;
            case 4:
                Debug.Log("4Skill Ghost");
                anim.SetTrigger("Skill");
                break;
        }
    }
}