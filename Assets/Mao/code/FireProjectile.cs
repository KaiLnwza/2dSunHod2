using UnityEngine;

// ติดที่วัตถุลูกไฟแต่ละลูก (randomObstacle เพิ่มให้อัตโนมัติ)
public class FireProjectile : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector2 direction = Vector2.up;
    private float speed = 8f;
    private int damage = 10;
    private bool moving = false;

    public Quaternion StartRotation { get { return startRotation; } }

    // จำตำแหน่งและมุมเริ่มต้น แล้วซ่อนไว้
    public void Capture()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        Hide();
    }

    // โผล่ที่ตำแหน่งเดิม แล้วพุ่งตรงตามทิศที่ได้รับ
    public void Launch(Vector2 dir, float spd, int dmg)
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        moving = true;
        gameObject.SetActive(true);
    }

    // ซ่อนและคืนตำแหน่งเดิม
    public void Hide()
    {
        moving = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!moving) return;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!moving) return;
        if (!other.CompareTag("Player")) return;

        // เรียก TakeDamage(int) ในสคริปต์ผู้เล่น (ถ้ามี)
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        Hide();
    }
}