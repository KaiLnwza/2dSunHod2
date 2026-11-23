using System.Collections;
using UnityEngine;

public class AfterimageEffect : MonoBehaviour
{
    [Header("ตั้งค่าร่างเงา")]
    public GameObject ghostPrefab;       // ลาก Prefab ร่างเงามาใส่ตรงนี้
    public float spawnInterval = 0.03f;  // ความถี่ในการสร้าง (ยิ่งน้อย ยิ่งชิดกันเป็นปื้นแบบในรูป)
    public float fadeDuration = 0.5f;    // เวลาที่ร่างเงาจะค่อยๆ จางหายไป
    public bool isRainbow = true;        // เปิด-ปิดโหมดสีรุ้ง

    [Header("สถานะการทำงาน")]
    public bool isSpawning = false;      // ติ๊กถูกใน Inspector หรือสั่งผ่านโค้ดตอนบอสขยับ

    private SpriteRenderer bossSprite;
    private float spawnTimer;
    private float hueValue = 0f;

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isSpawning) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnGhost();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnGhost()
    {
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSprite = ghost.GetComponent<SpriteRenderer>();
        ghostSprite.sprite = bossSprite.sprite; 
        ghostSprite.flipX = bossSprite.flipX;
        ghostSprite.flipY = bossSprite.flipY;

        Color targetColor = Color.white;
        if (isRainbow)
        {
            hueValue += 0.08f;
            if (hueValue > 1f) hueValue -= 1f;
            targetColor = Color.HSVToRGB(hueValue, 0.85f, 1f);
        }

        ghostSprite.color = targetColor;

        StartCoroutine(FadeAndDestroy(ghost, ghostSprite, targetColor));
    }

    IEnumerator FadeAndDestroy(GameObject ghostObj, SpriteRenderer sr, Color startColor)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.8f, 0f, elapsed / fadeDuration); 
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(ghostObj);
    }
}