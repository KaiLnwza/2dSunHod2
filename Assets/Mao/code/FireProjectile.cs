using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector2 direction = Vector2.up;
    private float speed = 8f;
    private int damage = 1;
    private bool moving = false;

    public Vector3 StartPosition { get { return startPosition; } }
    public Quaternion StartRotation { get { return startRotation; } }

    public void Capture()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        Hide();
    }

    // โผล่ที่ตำแหน่งเดิม หมุนหัวลูกไฟหันตามทิศทาง แล้วพุ่งไป
    public void Launch(Vector2 dir, float spd, int dmg, Vector2 spriteForwardDir)
    {
        transform.position = startPosition;
        direction = dir.normalized;
        speed = spd;
        damage = dmg;

        // หมุนหัวลูกไฟให้หันไปทาง Player ตามทิศหัวลูกไฟตั้งต้น (baseDirection)
        if (direction != Vector2.zero)
        {
            transform.rotation = Quaternion.FromToRotation(spriteForwardDir, direction);
        }

        moving = true;
        gameObject.SetActive(true);
    }

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

        // ลดเลือด Player 1 ดวง (ผ่าน PlayerHealth.TakeDamage(int))
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        Hide();
    }
}