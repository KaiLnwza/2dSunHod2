using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BossHealth), typeof(BossMovement), typeof(Animator))]
public class BossBrain : MonoBehaviour
{
    [Header("Debug / Test Mode")]
    [Tooltip("0 = Random, 1 = Summon, 2 = Lava, 3 = Bomb, 4 = Ghost")]
    public int debugForceSkill = 0; 

    [Header("Cooldown Base Attack")]
    public float minNormalCooldown = 5f;
    public float maxNormalCooldown = 8f;

    [Header("Cooldown Lava")]
    public float minLavaCooldown = 2f;
    public float maxLavaCooldown = 4f;

    [Header("Return to Center")]
    public float returnSpeed = 8f;

    [HideInInspector] public bool isLavaActive = false;
    [HideInInspector] public bool isPortalActive = false;

    private BossHealth health;
    private BossMovement movement;
    private Animator anim;

    private Skill_Lava skillLava;
    private Skill_Bomb skillBomb;

    private bool isUsingSkill = false; 

    void Awake()
    {
        health = GetComponent<BossHealth>();
        movement = GetComponent<BossMovement>();
        anim = GetComponent<Animator>();

        skillLava = GetComponent<Skill_Lava>();
        skillBomb = GetComponent<Skill_Bomb>();
    }

    void Start()
    {
        StartCoroutine(BossStateLoop());
    }

    void LateUpdate()
    {
        if (isUsingSkill && !health.isDead)
        {
            Transform centerTarget = movement.GetPointOne();
            if (centerTarget != null)
            {
                transform.position = new Vector3(centerTarget.position.x, centerTarget.position.y, transform.position.z);
            }
        }
    }

    IEnumerator BossStateLoop()
    {
        while (!health.isDead)
        {
            isUsingSkill = false;
            movement.ResetPathToStart();
            movement.SetMovementActive(true);
            movement.ChangePattern(BossMovement.MovePattern.CustomPathLoop);

            float waitTime = isLavaActive
                ? Random.Range(minLavaCooldown, maxLavaCooldown)
                : Random.Range(minNormalCooldown, maxNormalCooldown);

            yield return new WaitForSeconds(waitTime);

            if (health.isDead) yield break;

            movement.SetMovementActive(false);
            yield return StartCoroutine(ReturnToPointOne());

            isUsingSkill = true;
            yield return StartCoroutine(ExecuteRandomSkill());
            isUsingSkill = false;
        }
    }

    IEnumerator ReturnToPointOne()
    {
        Transform centerTarget = movement.GetPointOne();
        if (centerTarget == null) yield break;

        Vector3 targetPos = new Vector3(centerTarget.position.x, centerTarget.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            if (health.isDead) yield break;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    IEnumerator ExecuteRandomSkill()
    {
        int skillIndex;

        if (debugForceSkill >= 1 && debugForceSkill <= 4)
        {
            skillIndex = debugForceSkill;
        }
        else
        {
            skillIndex = Random.Range(1, 5);
            while (isLavaActive && skillIndex == 2)
            {
                skillIndex = Random.Range(1, 5);
            }
        }

        switch (skillIndex)
        {
            case 1:
                Debug.Log("1Skill Summon");
                anim.SetTrigger("Buff");
                isPortalActive = true;
                yield return new WaitForSeconds(2f);
                isPortalActive = false;
                break;

            case 2:
                Debug.Log("2Skill lavaFloor");
                anim.SetTrigger("Buff");
                if (skillLava != null)
                {
                    isLavaActive = true;
                    yield return StartCoroutine(skillLava.ExecuteSkill());
                    isLavaActive = false;
                }
                break;

            case 3:
                Debug.Log("3SKill Bome");
                anim.SetTrigger("Attack1");
                if (skillBomb != null)
                {
                    yield return StartCoroutine(skillBomb.ExecuteSkill());
                }
                break;

            case 4:
                Debug.Log("4Skill Ghost");
                anim.SetTrigger("Skill");
                yield return new WaitForSeconds(2f);
                break;
        }
    }
}