using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ใช้ "วัตถุไฟที่วางไว้ในฉาก" เป็นลูกไฟเลย ไม่ต้องใช้ Prefab
// ตอนเริ่มเกม ทุกลูกจะถูกซ่อน ถึงคิวสีไหนก็โผล่ที่ตำแหน่งเดิมแล้วพุ่งตามมุม z ของตัวเอง
public class randomObstacle : MonoBehaviour
{
    [Header("Settings time")]
    public float gameDuration = 210f;   // 3 นาที 30 วิ
    public float spawnInterval = 30f;   // ยิงทุก 30 วิ
    public float spawnTimer = 5f;       // ไฟอยู่ 5 วิ แล้วหาย

    [Header("Settings Fire")]
    public float fireSpeed = 8f;
    public int fireDamage = 10;
    [Tooltip("ทิศของรูปตอนหมุน z = 0 (ใช้กับทุก Header)")]
    public Vector2 baseDirection = Vector2.up;

    [Header("Random Settings (จำนวน Header ที่สุ่มในสีเดียวกัน)")]
    public int minRandomFire = 1;
    public int maxRandomFire = 2;

    [Header("Purple")]
    public GameObject[] firepurple1;
    public bool reversePurple1 = true;   // ติ๊ก = กลับทิศพุ่งของ Header นี้

    [Header("Red")]
    public GameObject[] firered1;
    public bool reverseRed1 = true;
    public GameObject[] firered2;
    public bool reverseRed2 = true;
    public GameObject[] firered3;       // เผื่อ Header แดงที่เพิ่มใหม่ (ว่างได้)
    public bool reverseRed3 = true;
    public GameObject[] firered4;
    public bool reverseRed4 = true;

    [Header("Blue")]
    public GameObject[] fireblue1;
    public bool reverseBlue1 = false;
    public GameObject[] fireblue2;
    public bool reverseBlue2 = false;
    public GameObject[] fireblue3;
    public bool reverseBlue3 = false;
    public GameObject[] fireblue4;
    public bool reverseBlue4 = false;

    [Header("Green")]
    public GameObject[] firegreen1;
    public bool reverseGreen1 = false;
    public GameObject[] firegreen2;
    public bool reverseGreen2 = false;

    // Header 1 ชุด = ลูกไฟหลายดวง + id (ใช้หาค่า reverse ของชุดนั้น)
    private class FireHeader
    {
        public GameObject[] fires;
        public int id;
        public FireHeader(GameObject[] fires, int id)
        {
            this.fires = fires;
            this.id = id;
        }
    }

    private bool gameEnded = false;
    private int lastColor = -1;

    private readonly List<FireProjectile> activeFires = new List<FireProjectile>();

    // ชุดสี: 0 = ม่วง, 1 = แดง, 2 = ฟ้า
    private List<FireHeader[]> colorGroups;

    private bool IsReversed(int id)
    {
        switch (id)
        {
            case 0: return reversePurple1;
            case 1: return reverseRed1;
            case 2: return reverseRed2;
            case 3: return reverseBlue1;
            case 4: return reverseBlue2;
            case 5: return reverseBlue3;
            case 6: return reverseBlue4;
            case 7: return reverseRed3;
            case 8: return reverseRed4;
            case 9: return reverseGreen1;
            case 10: return reverseGreen2;
        }
        return false;
    }

    private void Awake()
    {
        colorGroups = new List<FireHeader[]>
        {
            new FireHeader[] { new FireHeader(firepurple1, 0) },
            new FireHeader[]
            {
                new FireHeader(firered1, 1),
                new FireHeader(firered2, 2),
                new FireHeader(firered3, 7),
                new FireHeader(firered4, 8)
            },
            new FireHeader[]
            {
                new FireHeader(fireblue1, 3),
                new FireHeader(fireblue2, 4),
                new FireHeader(fireblue3, 5),
                new FireHeader(fireblue4, 6)
            },
            new FireHeader[]
            {
                new FireHeader(firegreen1, 9),
                new FireHeader(firegreen2, 10)
            },
        };

        // เตรียมลูกไฟทุกลูก แล้วซ่อนไว้ก่อน
        string[] headerNames = { "Firepurple1", "Firered1", "Firered2", "Fireblue1", "Fireblue2", "Fireblue3", "Fireblue4", "Firered3", "Firered4", "Firegreen1", "Firegreen2" };
        foreach (FireHeader[] group in colorGroups)
        {
            foreach (FireHeader h in group)
            {
                int n = (h.fires == null) ? 0 : h.fires.Length;
                Debug.Log("[randomObstacle] " + headerNames[h.id] + " มีลูกไฟ " + n + " ตัว" + (n == 0 ? "  <-- ว่าง! ต้องลากวัตถุไฟใส่ช่องนี้" : ""));
                PrepareHeader(h.fires);
            }
        }
    }

    void PrepareHeader(GameObject[] fires)
    {
        if (fires == null) return;

        foreach (GameObject fire in fires)
        {
            if (fire == null) continue;

            // ถ้าลูกไฟตัวไหนมีปัญหา ให้ข้ามตัวนั้นแล้วบอกชื่อใน Console (ไม่ให้ตัวอื่นพังตามไปด้วย)
            try
            {
                PrepareOne(fire);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[randomObstacle] เตรียมลูกไฟ '" + fire.name + "' ไม่สำเร็จ: " + e.Message, fire);
            }
        }
    }

    void PrepareOne(GameObject fire)
    {
        // ต้องมี Collider2D (Trigger) + Rigidbody2D (Kinematic) เพื่อให้ชนผู้เล่นได้
        Collider2D col = fire.GetComponent<Collider2D>();
        if (col == null) col = fire.AddComponent<CircleCollider2D>();
        if (col != null) col.isTrigger = true;

        Rigidbody2D rb = fire.GetComponent<Rigidbody2D>();
        if (rb == null) rb = fire.AddComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        FireProjectile proj = fire.GetComponent<FireProjectile>();
        if (proj == null) proj = fire.AddComponent<FireProjectile>();

        proj.Capture();   // จำตำแหน่ง/มุมเริ่มต้น แล้วซ่อน
    }

    private void Start()
    {
        Debug.Log("[randomObstacle] เริ่มจับเวลา รอยิงรอบแรกใน " + spawnInterval + " วิ");
        StartCoroutine(GameRoutine());
    }

    // ทดสอบสุ่มสี: เล่นเกมอยู่ > คลิกขวาที่ชื่อสคริปต์ randomObstacle > Test Spawn
    [ContextMenu("Test Spawn")]
    void TestSpawn()
    {
        ClearFires();
        SpawnColor(colorGroups[PickRandomColor()], -1);
    }

    // ทดสอบทุก Header ของสีฟ้าพร้อมกัน (ดูทิศพุ่งทีเดียว)
    [ContextMenu("Test Spawn All Blue")]
    void TestSpawnAllBlue() { TestAll(2); }

    [ContextMenu("Test Spawn All Red")]
    void TestSpawnAllRed() { TestAll(1); }

    [ContextMenu("Test Spawn All Purple")]
    void TestSpawnAllPurple() { TestAll(0); }

    [ContextMenu("Test Spawn All Green")]
    void TestSpawnAllGreen() { TestAll(3); }

    void TestAll(int colorIndex)
    {
        ClearFires();
        foreach (FireHeader h in colorGroups[colorIndex])
            LaunchHeader(h);
    }

    // ควบคุมเวลาของเกม
    IEnumerator GameRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime + spawnInterval <= gameDuration)
        {
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;

            SpawnColor(colorGroups[PickRandomColor()], -1);

            yield return new WaitForSeconds(spawnTimer);
            ClearFires();

            float remaining = spawnInterval - spawnTimer;
            if (elapsedTime + remaining > gameDuration)
                remaining = Mathf.Max(0f, gameDuration - elapsedTime - spawnTimer);

            yield return new WaitForSeconds(remaining);
            elapsedTime += spawnTimer + remaining;
        }

        ClearFires();
        gameEnded = true;
        Debug.Log("Game Ended!");
    }

    bool HasFires(FireHeader[] group)
    {
        foreach (FireHeader h in group)
        {
            if (h != null && h.fires != null && h.fires.Length > 0)
                return true;
        }
        return false;
    }

    int PickRandomColor()
    {
        int index;
        int tries = 0;
        do
        {
            index = Random.Range(0, colorGroups.Count);
            tries++;
        }
        // ไม่สุ่มสีเดิมซ้ำ และข้ามสีที่ไม่มีลูกไฟเลย (กันรอบที่ไม่ยิงอะไร)
        while (tries < 50 && colorGroups.Count > 1 && (index == lastColor || !HasFires(colorGroups[index])));

        lastColor = index;
        return index;
    }

    // สุ่มเลือก Header ในสีที่ได้ แล้วปล่อยไฟพร้อมกันทุกลูก (พารามิเตอร์ที่สองไม่ได้ใช้ ไว้เผื่อขยาย)
    void SpawnColor(FireHeader[] headers, int unused)
    {
        List<FireHeader> available = new List<FireHeader>();
        foreach (FireHeader h in headers)
        {
            if (h != null && h.fires != null && h.fires.Length > 0)
                available.Add(h);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("[randomObstacle] สีนี้ไม่มีลูกไฟใน Header เลย (อาร์เรย์ว่าง) ต้องลากวัตถุไฟใส่ช่อง Firexxx");
            return;
        }

        int min = Mathf.Clamp(minRandomFire, 1, available.Count);
        int max = Mathf.Clamp(maxRandomFire, min, available.Count);
        int count = Random.Range(min, max + 1);

        for (int i = 0; i < available.Count; i++)
        {
            int r = Random.Range(i, available.Count);
            FireHeader tmp = available[i];
            available[i] = available[r];
            available[r] = tmp;
        }

        Debug.Log("[randomObstacle] ยิง! สุ่มได้ " + count + " Header จาก " + available.Count);

        for (int i = 0; i < count; i++)
            LaunchHeader(available[i]);
    }

    // ปล่อยลูกไฟทุกลูกใน Header พร้อมกัน
    void LaunchHeader(FireHeader header)
    {
        float sign = IsReversed(header.id) ? -1f : 1f;

        foreach (GameObject fire in header.fires)
        {
            if (fire == null) continue;

            FireProjectile proj = fire.GetComponent<FireProjectile>();
            if (proj == null) continue;

            // ทิศ = หมุนตามมุม z เริ่มต้นของลูกไฟ (กลับทิศถ้าติ๊ก reverse)
            Vector2 dir = (proj.StartRotation * baseDirection) * sign;

            Debug.Log("[randomObstacle] " + fire.name + " z=" + proj.StartRotation.eulerAngles.z.ToString("F1")
                      + " ทิศพุ่ง=" + dir.normalized + (sign < 0 ? " (reverse)" : ""));

            proj.Launch(dir, fireSpeed, fireDamage);
            activeFires.Add(proj);
        }
    }

    // ซ่อนลูกไฟทั้งหมด และคืนกลับตำแหน่งเดิม
    void ClearFires()
    {
        foreach (FireProjectile fire in activeFires)
        {
            if (fire != null)
                fire.Hide();
        }
        activeFires.Clear();
    }
}