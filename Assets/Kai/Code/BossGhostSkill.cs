using UnityEngine;
using System.Collections;

public class BossGhostSkill : MonoBehaviour
{
    [Header("ตั้งค่า Prefab และเป้าหมาย")]
    public GameObject ghostPrefab;
    public Transform player;

    [Header("รูปแบบการปล่อย (3 ตัว x 3 รอบ)")]
    public int totalWaves = 3;
    public int ghostsPerWave = 3;
    public float timeBetweenWaves = 1.8f;
    public float spawnRadius = 2.2f;

    private Animator anim;
    private BossHealth bossHealth;

    void Awake()
    {
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<BossHealth>();
    }

    public IEnumerator ExecuteSkill()
    {
        if (ghostPrefab == null || player == null) yield break;

        for (int wave = 0; wave < totalWaves; wave++)
        {
            if (bossHealth != null && bossHealth.isDead) yield break;

            if (anim != null) anim.SetTrigger("Skill");

            // --- [เพิ่มเสียงปล่อยผี] ดังรอบละ 1 ครั้งตอนเสกผีออกมา ---
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX(SoundManager.Instance.bossGhostSound);

            for (int i = 0; i < ghostsPerWave; i++)
            {
                float angle = Mathf.Lerp(30f, 150f, ghostsPerWave == 1 ? 0.5f : (float)i / (ghostsPerWave - 1));
                float rad = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * spawnRadius;

                GameObject ghostObj = Instantiate(ghostPrefab, transform.position + offset, Quaternion.identity);
                GhostProjectile ghostScript = ghostObj.GetComponent<GhostProjectile>();
                if (ghostScript != null)
                {
                    ghostScript.Initialize(player);
                }
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}