using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Bomb : MonoBehaviour
{
    [Header("ตั้งค่าสกิลระเบิด")]
    public GameObject bombPrefab;          
    public Transform spawnPointsContainer;
    public float skillDuration = 10f;      
    public float delayBetweenBombs = 0.2f; 

    private List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        if (spawnPointsContainer != null)
        {
            foreach (Transform child in spawnPointsContainer)
            {
                spawnPoints.Add(child);
            }
        }
    }

    public IEnumerator ExecuteSkill()
    {
        if (bombPrefab == null || spawnPoints.Count == 0) yield break;

        float skillTimer = 0f;

        while (skillTimer < skillDuration)
        {
            Transform targetPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject currentBomb = Instantiate(bombPrefab, targetPoint.position, Quaternion.identity);

            while (currentBomb != null)
            {
                skillTimer += Time.deltaTime;
                yield return null;
            }

            if (skillTimer < skillDuration)
            {
                yield return new WaitForSeconds(delayBetweenBombs);
                skillTimer += delayBetweenBombs;
            }
        }
    }
}